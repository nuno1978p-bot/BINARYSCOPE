using System.IO;
using System.Text.Json;

namespace NONIVERUS.BinaryScope.Windows.Product;

public static class ProductPreferencesStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public static string SettingsDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "NONIVERUS",
            "BinaryScope");

    public static string SettingsPath => Path.Combine(SettingsDirectory, "product-settings.json");

    public static ProductPreferences Load()
    {
        try
        {
            if (!File.Exists(SettingsPath))
                return CreateDefault();

            var json = File.ReadAllText(SettingsPath);
            var preferences = JsonSerializer.Deserialize<ProductPreferences>(json, JsonOptions);
            return Normalize(preferences ?? CreateDefault());
        }
        catch
        {
            // Product preferences must never prevent BinaryScope from starting.
            return CreateDefault();
        }
    }

    public static void Save(ProductPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(preferences);

        var normalized = Normalize(preferences.Clone());
        Directory.CreateDirectory(SettingsDirectory);

        var tempPath = SettingsPath + ".tmp";
        var json = JsonSerializer.Serialize(normalized, JsonOptions);
        File.WriteAllText(tempPath, json);

        File.Move(tempPath, SettingsPath, overwrite: true);
    }

    public static ProductPreferences CreateDefault() => new()
    {
        OnboardingCompleted = false,
        LanguageCode = "en",
        ThemeKey = ThemeService.MidnightKey,
        LastSeenWhatsNewVersion = string.Empty
    };

    private static ProductPreferences Normalize(ProductPreferences preferences)
    {
        if (!ProductCatalog.IsSupportedLanguage(preferences.LanguageCode))
            preferences.LanguageCode = "en";

        if (!ThemeService.IsSupported(preferences.ThemeKey))
            preferences.ThemeKey = ThemeService.MidnightKey;

        preferences.LastSeenWhatsNewVersion ??= string.Empty;
        return preferences;
    }
}
