using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Policy;

public static class GoogleDexPolicy
{
    // Official Google threshold for apps: DEX code > 10 MB.
    // Decimal MB is intentionally used for the policy boundary.
    public const long AppThresholdBytes = 10_000_000;

    // NONIVERUS internal early-warning boundary. This is NOT a Google requirement.
    public const long NoniverusAdvisoryBytes = 8_000_000;

    public const decimal MinimumPercent = 25m;

    public static GoogleDexPolicyResult Evaluate(long localDexBytes)
    {
        var applicability = localDexBytes switch
        {
            > AppThresholdBytes => GoogleDexApplicability.RuleApplies,
            >= NoniverusAdvisoryBytes => GoogleDexApplicability.ApproachingThreshold,
            _ => GoogleDexApplicability.ComfortableMargin
        };

        var verdict = applicability switch
        {
            GoogleDexApplicability.ComfortableMargin => "LOCAL PREFLIGHT: below NONIVERUS 8 MB advisory boundary.",
            GoogleDexApplicability.ApproachingThreshold => "LOCAL PREFLIGHT: approaching Google's >10 MB app DEX threshold.",
            _ => "LOCAL PREFLIGHT: Google February 2027 DEX optimization rule is expected to apply; Play Console verification required."
        };

        return new GoogleDexPolicyResult(
            localDexBytes,
            AppThresholdBytes,
            NoniverusAdvisoryBytes,
            applicability,
            verdict,
            MinimumPercent,
            MinimumPercent,
            MinimumPercent,
            "UNPROVEN LOCALLY — enter/verify the official Play Console App Bundle Explorer percentages.");
    }
}
