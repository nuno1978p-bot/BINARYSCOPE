namespace NONIVERUS.BinaryScope.Windows.Product;

public static class ShellNavigationLocalization
{
    public static string Analyze(string? languageCode) => Normalize(languageCode) switch
    {
        "pt" => "Analisar",
        "es" => "Analizar",
        "fr" => "Analyser",
        "de" => "Analysieren",
        "it" => "Analizza",
        "pl" => "Analizuj",
        "nl" => "Analyseren",
        "cs" => "Analyzovat",
        "ro" => "Analizează",
        _ => "Analyze"
    };

    private static string Normalize(string? languageCode)
    {
        if (string.IsNullOrWhiteSpace(languageCode))
            return "en";
        var value = languageCode.Trim().ToLowerInvariant();
        var dash = value.IndexOf('-');
        return dash > 0 ? value[..dash] : value;
    }
}
