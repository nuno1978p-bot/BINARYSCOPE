namespace NONIVERUS.BinaryScope.Windows.Product;

public sealed class ProductPreferences
{
    public bool OnboardingCompleted { get; set; }

    public string LanguageCode { get; set; } = "en";

    public string ThemeKey { get; set; } = ThemeService.MidnightKey;

    public string LastSeenWhatsNewVersion { get; set; } = string.Empty;

    public ProductPreferences Clone() => new()
    {
        OnboardingCompleted = OnboardingCompleted,
        LanguageCode = LanguageCode,
        ThemeKey = ThemeKey,
        LastSeenWhatsNewVersion = LastSeenWhatsNewVersion
    };
}
