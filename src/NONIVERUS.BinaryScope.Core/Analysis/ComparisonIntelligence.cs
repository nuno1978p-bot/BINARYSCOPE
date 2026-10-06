using NONIVERUS.BinaryScope.Models;
using NONIVERUS.BinaryScope.Policy;

namespace NONIVERUS.BinaryScope.Analysis;

public static class ComparisonIntelligence
{
    public const decimal DexGrowthReviewPercent = 5m;
    public const decimal AabGrowthReviewPercent = 10m;
    public const decimal NativeGrowthReviewPercent = 10m;

    public static ComparisonIntelligenceResult Evaluate(
        AabAnalysis before,
        AabAnalysis after,
        AabComparison comparison)
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);
        ArgumentNullException.ThrowIfNull(comparison);

        var addedContributors = comparison.ContributorChanges.Count(c => c.Change == "ADDED");
        var removedContributors = comparison.ContributorChanges.Count(c => c.Change == "REMOVED");

        return EvaluateSignals(new ComparisonSignals(
            before.TotalDexBytes,
            after.TotalDexBytes,
            before.BundleBytes,
            after.BundleBytes,
            before.TotalNativeBinaryBytes,
            after.TotalNativeBinaryBytes,
            before.DexFiles.Count,
            after.DexFiles.Count,
            comparison.AddedAbis,
            comparison.RemovedAbis,
            addedContributors,
            removedContributors));
    }

    public static ComparisonIntelligenceResult EvaluateSignals(ComparisonSignals signals)
    {
        ArgumentNullException.ThrowIfNull(signals);

        var policy = new ComparisonGatePolicySnapshot(
            GoogleDexPolicy.AppThresholdBytes,
            GoogleDexPolicy.NoniverusAdvisoryBytes,
            DexGrowthReviewPercent,
            AabGrowthReviewPercent,
            NativeGrowthReviewPercent);

        var findings = new List<ComparisonFinding>();
        var improvements = new List<ComparisonFinding>();

        // Hard regression gate: this comparison newly crosses the configured Google DEX applicability boundary.
        if (signals.BeforeDexBytes <= GoogleDexPolicy.AppThresholdBytes &&
            signals.AfterDexBytes > GoogleDexPolicy.AppThresholdBytes)
        {
            findings.Add(new ComparisonFinding(
                "REGRESSION",
                "DEX_GOOGLE_THRESHOLD_CROSSED",
                "DEX crossed the configured Google threshold",
                $"DEX raw increased from {signals.BeforeDexBytes:N0} to {signals.AfterDexBytes:N0} bytes and is now above {GoogleDexPolicy.AppThresholdBytes:N0} bytes."));
        }
        else if (signals.BeforeDexBytes > GoogleDexPolicy.AppThresholdBytes &&
                 signals.AfterDexBytes <= GoogleDexPolicy.AppThresholdBytes)
        {
            improvements.Add(new ComparisonFinding(
                "IMPROVEMENT",
                "DEX_GOOGLE_THRESHOLD_RECOVERED",
                "DEX moved below the configured Google threshold",
                $"DEX raw decreased from {signals.BeforeDexBytes:N0} to {signals.AfterDexBytes:N0} bytes."));
        }

        // Internal early-warning transition. Do not duplicate it when the hard Google threshold was crossed.
        if (signals.BeforeDexBytes < GoogleDexPolicy.NoniverusAdvisoryBytes &&
            signals.AfterDexBytes >= GoogleDexPolicy.NoniverusAdvisoryBytes &&
            signals.AfterDexBytes <= GoogleDexPolicy.AppThresholdBytes)
        {
            findings.Add(new ComparisonFinding(
                "REVIEW",
                "DEX_NONIVERUS_ADVISORY_CROSSED",
                "DEX crossed the NONIVERUS advisory boundary",
                $"DEX raw moved from below {GoogleDexPolicy.NoniverusAdvisoryBytes:N0} bytes to {signals.AfterDexBytes:N0} bytes."));
        }
        else if (signals.BeforeDexBytes >= GoogleDexPolicy.NoniverusAdvisoryBytes &&
                 signals.AfterDexBytes < GoogleDexPolicy.NoniverusAdvisoryBytes)
        {
            improvements.Add(new ComparisonFinding(
                "IMPROVEMENT",
                "DEX_NONIVERUS_ADVISORY_RECOVERED",
                "DEX moved below the NONIVERUS advisory boundary",
                $"DEX raw decreased to {signals.AfterDexBytes:N0} bytes."));
        }

        AddGrowthReview(
            findings,
            "DEX_GROWTH_REVIEW",
            "DEX raw growth requires review",
            signals.BeforeDexBytes,
            signals.AfterDexBytes,
            DexGrowthReviewPercent);

        AddGrowthReview(
            findings,
            "AAB_GROWTH_REVIEW",
            "AAB size growth requires review",
            signals.BeforeAabBytes,
            signals.AfterAabBytes,
            AabGrowthReviewPercent);

        AddGrowthReview(
            findings,
            "NATIVE_GROWTH_REVIEW",
            "Native/AOT footprint growth requires review",
            signals.BeforeNativeBytes,
            signals.AfterNativeBytes,
            NativeGrowthReviewPercent);

        if (signals.AfterDexFiles > signals.BeforeDexFiles)
        {
            findings.Add(new ComparisonFinding(
                "REVIEW",
                "DEX_FILE_COUNT_INCREASED",
                "DEX file count increased",
                $"DEX files increased from {signals.BeforeDexFiles} to {signals.AfterDexFiles}."));
        }

        if (signals.AddedAbis.Count > 0)
        {
            findings.Add(new ComparisonFinding(
                "REVIEW",
                "ABI_ADDED",
                "Packaged ABI added",
                $"Added ABI(s): {string.Join(", ", signals.AddedAbis)}."));
        }

        if (signals.RemovedAbis.Count > 0)
        {
            findings.Add(new ComparisonFinding(
                "REVIEW",
                "ABI_REMOVED",
                "Packaged ABI removed",
                $"Removed ABI(s): {string.Join(", ", signals.RemovedAbis)}. Confirm this platform coverage change is intentional."));
        }

        if (signals.AddedContributors > 0)
        {
            findings.Add(new ComparisonFinding(
                "REVIEW",
                "BINARY_CONTRIBUTORS_ADDED",
                "Binary contributors were added",
                $"{signals.AddedContributors:N0} logical packaged binary contributor(s) were added."));
        }

        if (signals.RemovedContributors > 0)
        {
            findings.Add(new ComparisonFinding(
                "REVIEW",
                "BINARY_CONTRIBUTORS_REMOVED",
                "Binary contributors were removed",
                $"{signals.RemovedContributors:N0} logical packaged binary contributor(s) were removed. Confirm the capability change is intentional."));
        }

        AddDecreaseImprovement(
            improvements,
            "DEX_SIZE_DECREASED",
            "DEX raw footprint decreased",
            signals.BeforeDexBytes,
            signals.AfterDexBytes);

        AddDecreaseImprovement(
            improvements,
            "AAB_SIZE_DECREASED",
            "AAB size decreased",
            signals.BeforeAabBytes,
            signals.AfterAabBytes);

        AddDecreaseImprovement(
            improvements,
            "NATIVE_SIZE_DECREASED",
            "Native/AOT footprint decreased",
            signals.BeforeNativeBytes,
            signals.AfterNativeBytes);

        var verdict =
            findings.Any(f => f.Severity == "REGRESSION") ? "REGRESSION" :
            findings.Any(f => f.Severity == "REVIEW") ? "REVIEW" :
            "PASS";

        return new ComparisonIntelligenceResult(
            verdict,
            policy,
            findings,
            improvements);
    }

    private static void AddGrowthReview(
        List<ComparisonFinding> findings,
        string code,
        string title,
        long before,
        long after,
        decimal thresholdPercent)
    {
        if (before <= 0 || after <= before)
            return;

        var growthPercent = ((decimal)(after - before) / before) * 100m;
        if (growthPercent < thresholdPercent)
            return;

        findings.Add(new ComparisonFinding(
            "REVIEW",
            code,
            title,
            $"Increased from {before:N0} to {after:N0} bytes ({growthPercent:N2}%). Internal review threshold: {thresholdPercent:N2}%."));
    }

    private static void AddDecreaseImprovement(
        List<ComparisonFinding> improvements,
        string code,
        string title,
        long before,
        long after)
    {
        if (after >= before)
            return;

        var delta = after - before;
        var percent = before > 0 ? ((decimal)delta / before) * 100m : 0m;

        improvements.Add(new ComparisonFinding(
            "IMPROVEMENT",
            code,
            title,
            $"{before:N0} -> {after:N0} bytes ({delta:+#,0;-#,0;0}; {percent:N2}%)."));
    }
}
