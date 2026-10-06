namespace NONIVERUS.BinaryScope.Models;

public sealed record ComparisonGatePolicySnapshot(
    long GoogleDexThresholdBytes,
    long NoniverusAdvisoryBytes,
    decimal DexGrowthReviewPercent,
    decimal AabGrowthReviewPercent,
    decimal NativeGrowthReviewPercent);

public sealed record ComparisonFinding(
    string Severity,
    string Code,
    string Title,
    string Detail);

public sealed record ComparisonIntelligenceResult(
    string Verdict,
    ComparisonGatePolicySnapshot Policy,
    IReadOnlyList<ComparisonFinding> Findings,
    IReadOnlyList<ComparisonFinding> Improvements)
{
    public static ComparisonIntelligenceResult Empty { get; } = new(
        "UNPROVEN",
        new ComparisonGatePolicySnapshot(10_000_000, 8_000_000, 5m, 10m, 10m),
        Array.Empty<ComparisonFinding>(),
        Array.Empty<ComparisonFinding>());
}

public sealed record ComparisonSignals(
    long BeforeDexBytes,
    long AfterDexBytes,
    long BeforeAabBytes,
    long AfterAabBytes,
    long BeforeNativeBytes,
    long AfterNativeBytes,
    int BeforeDexFiles,
    int AfterDexFiles,
    IReadOnlyList<string> AddedAbis,
    IReadOnlyList<string> RemovedAbis,
    int AddedContributors,
    int RemovedContributors);
