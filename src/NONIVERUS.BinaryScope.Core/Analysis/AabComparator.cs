using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Analysis;

public static class AabComparator
{
    public static AabComparison Compare(AabAnalysis before, AabAnalysis after)
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);

        var metrics = new[]
        {
            Metric("AAB bytes", before.BundleBytes, after.BundleBytes),
            Metric("DEX files", before.DexFiles.Count, after.DexFiles.Count),
            Metric("DEX raw bytes", before.TotalDexBytes, after.TotalDexBytes),
            Metric("DEX compressed bytes", before.TotalDexCompressedBytes, after.TotalDexCompressedBytes),
            Metric("Method IDs (header sum)", checked((long)before.TotalMethodIds), checked((long)after.TotalMethodIds)),
            Metric("Unique method references", checked((long)before.UniqueMethodReferences), checked((long)after.UniqueMethodReferences)),
            Metric("Class definitions", checked((long)before.TotalClassDefs), checked((long)after.TotalClassDefs)),
            Metric("Defined methods", checked((long)before.TotalDefinedMethods), checked((long)after.TotalDefinedMethods)),
            Metric("Methods with code", checked((long)before.TotalMethodsWithCode), checked((long)after.TotalMethodsWithCode)),
            Metric("Native/AOT .so entries", before.NativeBinaryEntries.Count, after.NativeBinaryEntries.Count),
            Metric("Native/AOT raw bytes", before.TotalNativeBinaryBytes, after.TotalNativeBinaryBytes),
            Metric("Native/AOT compressed bytes", before.TotalNativeBinaryCompressedBytes, after.TotalNativeBinaryCompressedBytes),
            Metric("Logical binary contributors", before.BinaryContributors.Count, after.BinaryContributors.Count)
        };

        var beforeContributors = before.BinaryContributors.ToDictionary(Key, StringComparer.Ordinal);
        var afterContributors = after.BinaryContributors.ToDictionary(Key, StringComparer.Ordinal);

        var contributorKeys = beforeContributors.Keys
            .Union(afterContributors.Keys, StringComparer.Ordinal)
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToArray();

        var contributorChanges = new List<BinaryContributorDelta>();

        foreach (var key in contributorKeys)
        {
            beforeContributors.TryGetValue(key, out var b);
            afterContributors.TryGetValue(key, out var a);

            var change = b is null
                ? "ADDED"
                : a is null
                    ? "REMOVED"
                    : b.UncompressedBytes != a.UncompressedBytes ||
                      b.CompressedBytes != a.CompressedBytes ||
                      b.AbiCount != a.AbiCount
                        ? "CHANGED"
                        : "UNCHANGED";

            if (change == "UNCHANGED")
                continue;

            var name = a?.ContributorName ?? b!.ContributorName;
            var category = a?.Category ?? b!.Category;

            contributorChanges.Add(new BinaryContributorDelta(
                name,
                category,
                b?.UncompressedBytes ?? 0,
                a?.UncompressedBytes ?? 0,
                (a?.UncompressedBytes ?? 0) - (b?.UncompressedBytes ?? 0),
                b?.CompressedBytes ?? 0,
                a?.CompressedBytes ?? 0,
                (a?.CompressedBytes ?? 0) - (b?.CompressedBytes ?? 0),
                b?.AbiCount ?? 0,
                a?.AbiCount ?? 0,
                change));
        }

        contributorChanges = contributorChanges
            .OrderByDescending(c => Math.Abs(c.DeltaRawBytes))
            .ThenBy(c => c.ContributorName, StringComparer.Ordinal)
            .ToList();

        var beforeCategories = before.BinaryCategories.ToDictionary(c => c.Category, StringComparer.Ordinal);
        var afterCategories = after.BinaryCategories.ToDictionary(c => c.Category, StringComparer.Ordinal);

        var categoryNames = beforeCategories.Keys
            .Union(afterCategories.Keys, StringComparer.Ordinal)
            .OrderBy(k => k, StringComparer.Ordinal)
            .ToArray();

        var categoryChanges = new List<BinaryCategoryDelta>();

        foreach (var category in categoryNames)
        {
            beforeCategories.TryGetValue(category, out var b);
            afterCategories.TryGetValue(category, out var a);

            var rawDelta = (a?.UncompressedBytes ?? 0) - (b?.UncompressedBytes ?? 0);
            var compressedDelta = (a?.CompressedBytes ?? 0) - (b?.CompressedBytes ?? 0);
            var contributorDelta = (a?.ContributorCount ?? 0) - (b?.ContributorCount ?? 0);

            var change = b is null
                ? "ADDED"
                : a is null
                    ? "REMOVED"
                    : rawDelta != 0 || compressedDelta != 0 || contributorDelta != 0
                        ? "CHANGED"
                        : "UNCHANGED";

            if (change == "UNCHANGED")
                continue;

            categoryChanges.Add(new BinaryCategoryDelta(
                category,
                b?.UncompressedBytes ?? 0,
                a?.UncompressedBytes ?? 0,
                rawDelta,
                b?.CompressedBytes ?? 0,
                a?.CompressedBytes ?? 0,
                compressedDelta,
                b?.ContributorCount ?? 0,
                a?.ContributorCount ?? 0,
                contributorDelta,
                change));
        }

        categoryChanges = categoryChanges
            .OrderByDescending(c => Math.Abs(c.DeltaRawBytes))
            .ThenBy(c => c.Category, StringComparer.Ordinal)
            .ToList();

        var beforeAbis = before.NativeAbis.ToHashSet(StringComparer.Ordinal);
        var afterAbis = after.NativeAbis.ToHashSet(StringComparer.Ordinal);

        var addedAbis = afterAbis.Except(beforeAbis, StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var removedAbis = beforeAbis.Except(afterAbis, StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();

        var comparison = new AabComparison(
            before.SourcePath,
            before.FileName,
            before.BundleSha256,
            after.SourcePath,
            after.FileName,
            after.BundleSha256,
            DateTimeOffset.UtcNow,
            metrics,
            contributorChanges,
            categoryChanges,
            addedAbis,
            removedAbis,
            "UNPROVEN FROM AAB ALONE — comparison is based on packaged binary contributors, not an exact NuGet dependency graph.");

        return comparison with
        {
            Intelligence = ComparisonIntelligence.Evaluate(before, after, comparison)
        };
    }

    private static ComparisonMetric Metric(string name, long before, long after)
    {
        var delta = after - before;
        double? percent = before == 0
            ? after == 0 ? 0d : null
            : delta / (double)before;

        return new ComparisonMetric(name, before, after, delta, percent);
    }

    private static string Key(BinaryContributorSummary contributor) =>
        contributor.Category + "\u001F" + contributor.ContributorName;
}
