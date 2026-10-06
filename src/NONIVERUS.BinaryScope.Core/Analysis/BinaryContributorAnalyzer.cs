using System.IO.Compression;
using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Analysis;

public static class BinaryContributorAnalyzer
{
    public static BinaryContributorAnalysis Analyze(IEnumerable<ZipArchiveEntry> entries)
    {
        var nativeEntries = entries
            .Where(e => TryParseNativeEntry(e, out _))
            .Select(e =>
            {
                _ = TryParseNativeEntry(e, out var parsed);
                var category = Classify(parsed.FileName);
                return new NativeBinaryEntryAnalysis(
                    e.FullName,
                    parsed.Module,
                    parsed.Abi,
                    parsed.FileName,
                    GetContributorName(parsed.FileName),
                    category,
                    e.Length,
                    e.CompressedLength);
            })
            .OrderBy(e => e.EntryName, StringComparer.Ordinal)
            .ToArray();

        var contributors = nativeEntries
            .GroupBy(
                e => (ContributorName: e.ContributorName, Category: e.Category),
                e => e)
            .Select(g =>
            {
                var abis = g.Select(e => e.Abi)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(s => s, StringComparer.Ordinal)
                    .ToArray();

                return new BinaryContributorSummary(
                    g.Key.ContributorName,
                    g.Key.Category,
                    g.Sum(e => e.UncompressedBytes),
                    g.Sum(e => e.CompressedBytes),
                    g.Count(),
                    abis.Length,
                    abis);
            })
            .OrderByDescending(c => c.UncompressedBytes)
            .ThenBy(c => c.ContributorName, StringComparer.Ordinal)
            .ToArray();

        var categories = contributors
            .GroupBy(c => c.Category, StringComparer.Ordinal)
            .Select(g => new BinaryCategorySummary(
                g.Key,
                g.Sum(c => c.UncompressedBytes),
                g.Sum(c => c.CompressedBytes),
                g.Count(),
                g.Sum(c => c.EntryCount)))
            .OrderByDescending(c => c.UncompressedBytes)
            .ThenBy(c => c.Category, StringComparer.Ordinal)
            .ToArray();

        var abisAll = nativeEntries
            .Select(e => e.Abi)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(s => s, StringComparer.Ordinal)
            .ToArray();

        return new BinaryContributorAnalysis(
            nativeEntries,
            contributors,
            categories,
            abisAll,
            nativeEntries.Sum(e => e.UncompressedBytes),
            nativeEntries.Sum(e => e.CompressedBytes));
    }

    private static bool TryParseNativeEntry(ZipArchiveEntry entry, out ParsedNativeEntry parsed)
    {
        var normalized = entry.FullName.Replace('\\', '/');
        var segments = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length >= 4 &&
            string.Equals(segments[^3], "lib", StringComparison.OrdinalIgnoreCase) &&
            segments[^1].EndsWith(".so", StringComparison.OrdinalIgnoreCase))
        {
            parsed = new ParsedNativeEntry(
                segments[0],
                segments[^2],
                segments[^1]);
            return true;
        }

        parsed = default;
        return false;
    }

    private static string GetContributorName(string fileName)
    {
        var name = fileName;

        if (name.StartsWith("libaot-", StringComparison.OrdinalIgnoreCase))
            name = name["libaot-".Length..];

        if (name.EndsWith(".dll.so", StringComparison.OrdinalIgnoreCase))
            name = name[..^3]; // Keep the .dll suffix, remove .so only.
        else if (name.EndsWith(".so", StringComparison.OrdinalIgnoreCase))
            name = name[..^3];

        return name;
    }

    private static string Classify(string fileName)
    {
        if (Contains(fileName, "BillingClient"))
            return "Google Play Billing";

        if (Contains(fileName, "Health.Connect"))
            return "Health Connect";

        if (Contains(fileName, "SQLitePCLRaw") ||
            Contains(fileName, "Microsoft.Data.Sqlite") ||
            string.Equals(fileName, "libe_sqlite3.so", StringComparison.OrdinalIgnoreCase))
            return "SQLite / Data";

        if (Contains(fileName, "Microsoft.Maui"))
            return ".NET MAUI";

        if (Contains(fileName, "Xamarin.AndroidX"))
            return "AndroidX";

        if (Contains(fileName, "Xamarin.Google"))
            return "Google libraries";

        if (Contains(fileName, "Xamarin.Kotlin") || Contains(fileName, "Xamarin.KotlinX"))
            return "Kotlin / KotlinX";

        if (Contains(fileName, "Microsoft.Extensions"))
            return "Microsoft Extensions";

        if (Contains(fileName, "NV.") || Contains(fileName, "NONIVERUS"))
            return "App code";

        if (Contains(fileName, "System.") ||
            Contains(fileName, "Mono.Android") ||
            Contains(fileName, "Java.Interop") ||
            Contains(fileName, "netstandard") ||
            fileName.StartsWith("libSystem.", StringComparison.OrdinalIgnoreCase) ||
            fileName.StartsWith("libmono", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fileName, "libmonodroid.so", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fileName, "libassembly-store.so", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fileName, "libarc.bin.so", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fileName, "libxamarin-app.so", StringComparison.OrdinalIgnoreCase))
            return ".NET / Android runtime";

        if (Contains(fileName, "Microsoft.Android.Resource.Designer"))
            return "Android resource glue";

        return "Native / Other";
    }

    private static bool Contains(string value, string token) =>
        value.Contains(token, StringComparison.OrdinalIgnoreCase);

    private readonly record struct ParsedNativeEntry(
        string Module,
        string Abi,
        string FileName);
}

public sealed record BinaryContributorAnalysis(
    IReadOnlyList<NativeBinaryEntryAnalysis> NativeEntries,
    IReadOnlyList<BinaryContributorSummary> Contributors,
    IReadOnlyList<BinaryCategorySummary> Categories,
    IReadOnlyList<string> Abis,
    long TotalUncompressedBytes,
    long TotalCompressedBytes);
