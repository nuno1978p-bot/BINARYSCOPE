using System.Text;
using System.Text.Json;
using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Reporting;

public static class ComparisonReportWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static string ToJson(AabComparison comparison) =>
        JsonSerializer.Serialize(comparison, JsonOptions);

    public static void WriteJson(AabComparison comparison, string path) =>
        File.WriteAllText(path, ToJson(comparison), Encoding.UTF8);

    public static string ToText(AabComparison comparison)
    {
        var sb = new StringBuilder();

        sb.AppendLine("NONIVERUS BinaryScope — AAB Comparison");
        sb.AppendLine(new string('=', 43));
        sb.AppendLine();
        sb.AppendLine($"BEFORE: {comparison.BeforeFileName}");
        sb.AppendLine($"SHA-256: {comparison.BeforeSha256}");
        sb.AppendLine($"AFTER:  {comparison.AfterFileName}");
        sb.AppendLine($"SHA-256: {comparison.AfterSha256}");
        sb.AppendLine($"Compared UTC: {comparison.ComparedAtUtc:O}");
        sb.AppendLine();

        sb.AppendLine("METRIC DELTAS");
        foreach (var metric in comparison.Metrics)
        {
            var percent = metric.PercentChange.HasValue ? $"{metric.PercentChange.Value:P2}" : "N/A";
            sb.AppendLine(
                $"{metric.Metric}: {metric.BeforeValue:N0} -> {metric.AfterValue:N0} | delta {metric.Delta:+#,0;-#,0;0} | {percent}");
        }

        sb.AppendLine();
        sb.AppendLine($"ABIs added: {(comparison.AddedAbis.Count == 0 ? "none" : string.Join(", ", comparison.AddedAbis))}");
        sb.AppendLine($"ABIs removed: {(comparison.RemovedAbis.Count == 0 ? "none" : string.Join(", ", comparison.RemovedAbis))}");

        sb.AppendLine();
        sb.AppendLine("CATEGORY CHANGES");
        if (comparison.CategoryChanges.Count == 0)
        {
            sb.AppendLine("none");
        }
        else
        {
            foreach (var category in comparison.CategoryChanges)
            {
                sb.AppendLine(
                    $"{category.Change} | {category.Category} | raw delta {category.DeltaRawBytes:+#,0;-#,0;0} | " +
                    $"compressed delta {category.DeltaCompressedBytes:+#,0;-#,0;0} | contributors delta {category.DeltaContributorCount:+#,0;-#,0;0}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("CONTRIBUTOR CHANGES");
        if (comparison.ContributorChanges.Count == 0)
        {
            sb.AppendLine("none");
        }
        else
        {
            foreach (var contributor in comparison.ContributorChanges)
            {
                sb.AppendLine(
                    $"{contributor.Change} | {contributor.ContributorName} | {contributor.Category} | " +
                    $"raw {contributor.BeforeRawBytes:N0} -> {contributor.AfterRawBytes:N0} " +
                    $"({contributor.DeltaRawBytes:+#,0;-#,0;0}) | ABIs {contributor.BeforeAbiCount} -> {contributor.AfterAbiCount}");
            }
        }

        sb.AppendLine();
        sb.AppendLine("COMPARISON INTELLIGENCE");
        sb.AppendLine($"Verdict: {comparison.Intelligence.Verdict}");
        sb.AppendLine(
            $"Internal review thresholds: DEX growth {comparison.Intelligence.Policy.DexGrowthReviewPercent:N2}% · " +
            $"AAB growth {comparison.Intelligence.Policy.AabGrowthReviewPercent:N2}% · " +
            $"Native/AOT growth {comparison.Intelligence.Policy.NativeGrowthReviewPercent:N2}%");
        sb.AppendLine(
            $"Configured boundaries: advisory {comparison.Intelligence.Policy.NoniverusAdvisoryBytes:N0} bytes · " +
            $"Google DEX threshold {comparison.Intelligence.Policy.GoogleDexThresholdBytes:N0} bytes");

        sb.AppendLine("Findings:");
        if (comparison.Intelligence.Findings.Count == 0)
        {
            sb.AppendLine("  none");
        }
        else
        {
            foreach (var finding in comparison.Intelligence.Findings)
                sb.AppendLine($"  [{finding.Severity}] {finding.Code} — {finding.Title}: {finding.Detail}");
        }

        sb.AppendLine("Improvement signals:");
        if (comparison.Intelligence.Improvements.Count == 0)
        {
            sb.AppendLine("  none");
        }
        else
        {
            foreach (var improvement in comparison.Intelligence.Improvements)
                sb.AppendLine($"  {improvement.Code} — {improvement.Title}: {improvement.Detail}");
        }

        sb.AppendLine();
        sb.AppendLine($"Dependency graph: {comparison.DependencyGraphStatus}");
        sb.AppendLine("Important: comparison describes packaged AAB evidence only; it does not prove why a dependency changed.");

        return sb.ToString();
    }

    public static void WriteText(AabComparison comparison, string path) =>
        File.WriteAllText(path, ToText(comparison), Encoding.UTF8);
}
