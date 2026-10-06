using System.IO.Compression;
using System.Security.Cryptography;
using NONIVERUS.BinaryScope.Models;
using NONIVERUS.BinaryScope.Policy;

namespace NONIVERUS.BinaryScope.Analysis;

public sealed class AabAnalyzer
{
    public AabAnalysis Analyze(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("AAB path is required.", nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("AAB not found.", path);
        if (!string.Equals(Path.GetExtension(path), ".aab", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("BinaryScope accepts Android App Bundle (.aab) files only.");

        var fileInfo = new FileInfo(path);
        var bundleHash = HashFile(path);

        using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var zip = new ZipArchive(file, ZipArchiveMode.Read, leaveOpen: false);

        var dexEntries = zip.Entries
            .Where(IsDexEntry)
            .OrderBy(e => e.FullName, StringComparer.Ordinal)
            .ToArray();

        var dexFiles = new List<DexFileAnalysis>(dexEntries.Length);
        var uniqueMethodReferences = new HashSet<string>(StringComparer.Ordinal);

        foreach (var entry in dexEntries)
        {
            using var stream = entry.Open();
            using var ms = new MemoryStream(capacity: entry.Length > int.MaxValue ? 0 : (int)entry.Length);
            stream.CopyTo(ms);
            var bytes = ms.ToArray();

            using var dexStream = new MemoryStream(bytes, writable: false);
            var header = DexHeaderReader.Read(dexStream);
            if (!header.HeaderValid)
                throw new InvalidDataException($"Invalid DEX header in {entry.FullName}: {header.Warning}");

            var deep = DexDeepMetricsReader.Read(bytes);
            uniqueMethodReferences.UnionWith(deep.MethodReferences);

            var hash = Convert.ToHexString(SHA256.HashData(bytes));
            dexFiles.Add(new DexFileAnalysis(
                entry.FullName,
                GetModule(entry.FullName),
                entry.Length,
                entry.CompressedLength,
                hash,
                header,
                deep.DefinedMethods,
                deep.MethodsWithCode));
        }

        var moduleManifestEntries = zip.Entries
            .Where(IsModuleManifestEntry)
            .Select(e => e.FullName)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        var modulesFromManifest = moduleManifestEntries
            .Select(FirstSegment)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        // A valid AAB module has <module>/manifest/AndroidManifest.xml.
        // Fallback to modules containing DEX only when no module manifest can be proven,
        // and surface that fact in evidence instead of silently claiming proof.
        var modulesProvenByManifest = modulesFromManifest.Length > 0;
        var modules = modulesProvenByManifest
            ? modulesFromManifest
            : dexFiles.Select(d => d.Module)
                .Where(s => !string.Equals(s, "unknown", StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(s => s, StringComparer.Ordinal)
                .ToArray();

        var metadataEntries = zip.Entries
            .Where(e => e.FullName.StartsWith("BUNDLE-METADATA/", StringComparison.OrdinalIgnoreCase))
            .Select(e => e.FullName)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        var mappings = metadataEntries
            .Where(e => e.Contains("proguard", StringComparison.OrdinalIgnoreCase) ||
                        e.EndsWith("mapping.txt", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var nativeLibraries = zip.Entries
            .Where(e => IsNativeLibraryEntry(e.FullName))
            .Select(e => e.FullName)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        var binary = BinaryContributorAnalyzer.Analyze(zip.Entries);

        var totalDex = dexFiles.Sum(d => d.UncompressedBytes);
        var totalCompressed = dexFiles.Sum(d => d.CompressedBytes);
        var totalMethods = dexFiles.Aggregate<DexFileAnalysis, ulong>(0, (sum, d) => sum + d.Header.MethodIds);
        var totalClasses = dexFiles.Aggregate<DexFileAnalysis, ulong>(0, (sum, d) => sum + d.Header.ClassDefs);
        var totalDefinedMethods = dexFiles.Aggregate<DexFileAnalysis, ulong>(0, (sum, d) => sum + d.DefinedMethods);
        var totalMethodsWithCode = dexFiles.Aggregate<DexFileAnalysis, ulong>(0, (sum, d) => sum + d.MethodsWithCode);

        return new AabAnalysis(
            Path.GetFullPath(path),
            Path.GetFileName(path),
            fileInfo.Length,
            bundleHash,
            DateTimeOffset.UtcNow,
            modules,
            dexFiles,
            totalDex,
            totalCompressed,
            totalMethods,
            checked((ulong)uniqueMethodReferences.Count),
            totalClasses,
            totalDefinedMethods,
            totalMethodsWithCode,
            binary.NativeEntries,
            binary.Contributors,
            binary.Categories,
            binary.Abis,
            binary.TotalUncompressedBytes,
            binary.TotalCompressedBytes,
            "UNPROVEN FROM AAB ALONE — packaged binary contributors do not reconstruct the exact NuGet dependency graph.",
            new AabEvidence(
                metadataEntries.Length > 0,
                mappings.Length > 0,
                mappings,
                metadataEntries,
                nativeLibraries,
                moduleManifestEntries,
                modulesProvenByManifest),
            GoogleDexPolicy.Evaluate(totalDex));
    }

    private static bool IsDexEntry(ZipArchiveEntry entry)
    {
        var name = entry.FullName.Replace('\\', '/');
        var segments = name.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length >= 3 &&
               string.Equals(segments[^2], "dex", StringComparison.OrdinalIgnoreCase) &&
               segments[^1].StartsWith("classes", StringComparison.OrdinalIgnoreCase) &&
               segments[^1].EndsWith(".dex", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsModuleManifestEntry(ZipArchiveEntry entry)
    {
        var name = entry.FullName.Replace('\\', '/');
        var segments = name.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length == 3 &&
               string.Equals(segments[1], "manifest", StringComparison.OrdinalIgnoreCase) &&
               string.Equals(segments[2], "AndroidManifest.xml", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsNativeLibraryEntry(string name)
    {
        var normalized = name.Replace('\\', '/');
        return normalized.Contains("/lib/", StringComparison.OrdinalIgnoreCase) &&
               normalized.EndsWith(".so", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetModule(string entryName) => FirstSegment(entryName) ?? "unknown";

    private static string? FirstSegment(string entryName)
    {
        var normalized = entryName.Replace('\\', '/');
        var slash = normalized.IndexOf('/');
        return slash <= 0 ? null : normalized[..slash];
    }

    private static string HashFile(string path)
    {
        using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}
