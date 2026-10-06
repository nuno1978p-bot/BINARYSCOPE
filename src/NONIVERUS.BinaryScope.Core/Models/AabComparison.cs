namespace NONIVERUS.BinaryScope.Models;

public sealed record ComparisonMetric(
    string Metric,
    long BeforeValue,
    long AfterValue,
    long Delta,
    double? PercentChange)
{
    public string Change => Delta switch
    {
        > 0 => "INCREASE",
        < 0 => "DECREASE",
        _ => "UNCHANGED"
    };
}

public sealed record BinaryContributorDelta(
    string ContributorName,
    string Category,
    long BeforeRawBytes,
    long AfterRawBytes,
    long DeltaRawBytes,
    long BeforeCompressedBytes,
    long AfterCompressedBytes,
    long DeltaCompressedBytes,
    int BeforeAbiCount,
    int AfterAbiCount,
    string Change);

public sealed record BinaryCategoryDelta(
    string Category,
    long BeforeRawBytes,
    long AfterRawBytes,
    long DeltaRawBytes,
    long BeforeCompressedBytes,
    long AfterCompressedBytes,
    long DeltaCompressedBytes,
    int BeforeContributorCount,
    int AfterContributorCount,
    int DeltaContributorCount,
    string Change);

public sealed record AabComparison(
    string BeforePath,
    string BeforeFileName,
    string BeforeSha256,
    string AfterPath,
    string AfterFileName,
    string AfterSha256,
    DateTimeOffset ComparedAtUtc,
    IReadOnlyList<ComparisonMetric> Metrics,
    IReadOnlyList<BinaryContributorDelta> ContributorChanges,
    IReadOnlyList<BinaryCategoryDelta> CategoryChanges,
    IReadOnlyList<string> AddedAbis,
    IReadOnlyList<string> RemovedAbis,
    string DependencyGraphStatus)
{
    public ComparisonIntelligenceResult Intelligence { get; init; } = ComparisonIntelligenceResult.Empty;
}
