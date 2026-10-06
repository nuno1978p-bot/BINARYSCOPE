namespace NONIVERUS.BinaryScope.Windows.Product;

public sealed record LanguageOption(string Code, string DisplayName);

public sealed record ThemeOption(string Key, string DisplayName);

public sealed record WhatsNewEntry(string TitleKey, string BodyKey);

public static class ProductCatalog
{
    public const string ProductName = "NONIVERUS BinaryScope";
    public const string Descriptor = "Android App Bundle Release Analyzer";
    public const string Tagline = "Inspect. Compare. Release with evidence.";

    // Product-shell version. This phase is pre-release and does not imply commercial readiness.
    public const string CurrentProductVersion = "0.1.6";

    // Keep this unchanged on future versions that do not warrant an automatic What's New screen.
    public const string CurrentWhatsNewVersion = "0.1.6";

    public static IReadOnlyList<LanguageOption> Languages { get; } =
    [
        new("en", "English"),
        new("pt-PT", "Português"),
        new("es", "Español"),
        new("fr", "Français"),
        new("de", "Deutsch"),
        new("it", "Italiano"),
        new("pl", "Polski"),
        new("nl", "Nederlands"),
        new("cs", "Čeština"),
        new("ro", "Română")
    ];

    public static IReadOnlyList<ThemeOption> Themes { get; } =
    [
        new(ThemeService.MidnightKey, "Midnight"),
        new(ThemeService.GraphiteKey, "Graphite"),
        new(ThemeService.LightPrecisionKey, "Light Precision")
    ];

    public static IReadOnlyList<WhatsNewEntry> CurrentWhatsNewEntries { get; } =
    [
        new("WhatsNew.Item.ProductShell.Title", "WhatsNew.Item.ProductShell.Body"),
        new("WhatsNew.Item.Languages.Title", "WhatsNew.Item.Languages.Body"),
        new("WhatsNew.Item.Themes.Title", "WhatsNew.Item.Themes.Body"),
        new("WhatsNew.Item.Screen.Title", "WhatsNew.Item.Screen.Body")
    ];

    public static bool IsSupportedLanguage(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return false;

        return Languages.Any(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase))
            || code.StartsWith("pt-", StringComparison.OrdinalIgnoreCase);
    }

    public static bool ShouldShowWhatsNew(ProductPreferences preferences) =>
        !string.Equals(
            preferences.LastSeenWhatsNewVersion,
            CurrentWhatsNewVersion,
            StringComparison.OrdinalIgnoreCase);
}
