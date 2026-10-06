using System.Text.Json;
using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Reporting;

public static class AnalysisReportWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static string ToJson(AabAnalysis analysis) => JsonSerializer.Serialize(analysis, JsonOptions);

    public static void WriteJson(AabAnalysis analysis, string path) =>
        File.WriteAllText(path, ToJson(analysis));
}
