namespace NONIVERUS.BinaryScope.Models;

public enum GoogleDexApplicability
{
    ComfortableMargin,
    ApproachingThreshold,
    RuleApplies
}

public sealed record GoogleDexPolicyResult(
    long LocalDexBytes,
    long GoogleAppThresholdBytes,
    long NoniverusAdvisoryBytes,
    GoogleDexApplicability Applicability,
    string LocalVerdict,
    decimal RequiredShrinkingPercent,
    decimal RequiredOptimizationPercent,
    decimal RequiredObfuscationPercent,
    string StoreMetricsStatus);
