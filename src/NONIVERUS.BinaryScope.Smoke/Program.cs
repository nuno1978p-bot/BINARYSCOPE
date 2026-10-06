using System.Text.Json;
using NONIVERUS.BinaryScope.Analysis;
using NONIVERUS.BinaryScope.Models;
using NONIVERUS.BinaryScope.Reporting;

if (args.Length == 1 && string.Equals(args[0], "--intelligence-selftest", StringComparison.Ordinal))
{
    var pass = ComparisonIntelligence.EvaluateSignals(new ComparisonSignals(
        9_000_000, 9_000_000,
        30_000_000, 30_000_000,
        20_000_000, 20_000_000,
        2, 2,
        Array.Empty<string>(),
        Array.Empty<string>(),
        0, 0));

    var review = ComparisonIntelligence.EvaluateSignals(new ComparisonSignals(
        9_232_204, 9_231_820,
        36_441_911, 24_086_466,
        50_585_688, 25_192_912,
        2, 2,
        Array.Empty<string>(),
        new[] { "x86_64" },
        0, 0));

    var regression = ComparisonIntelligence.EvaluateSignals(new ComparisonSignals(
        9_900_000, 10_100_000,
        30_000_000, 30_000_000,
        20_000_000, 20_000_000,
        2, 2,
        Array.Empty<string>(),
        Array.Empty<string>(),
        0, 0));

    Console.WriteLine(JsonSerializer.Serialize(new
    {
        Pass = pass,
        Review = review,
        Regression = regression
    }, new JsonSerializerOptions { WriteIndented = true }));

    return 0;
}

if (args.Length is not (1 or 2))
{
    Console.Error.WriteLine("Usage: NONIVERUS.BinaryScope.Smoke <file.aab> [after.aab]");
    Console.Error.WriteLine("       NONIVERUS.BinaryScope.Smoke --intelligence-selftest");
    return 2;
}

try
{
    var analyzer = new AabAnalyzer();
    var before = analyzer.Analyze(args[0]);

    if (args.Length == 1)
    {
        Console.WriteLine(AnalysisReportWriter.ToJson(before));
        return 0;
    }

    var after = analyzer.Analyze(args[1]);
    var comparison = AabComparator.Compare(before, after);
    Console.WriteLine(ComparisonReportWriter.ToJson(comparison));
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
    return 1;
}
