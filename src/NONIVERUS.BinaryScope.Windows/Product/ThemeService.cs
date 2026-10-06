using System.Windows;
using System.Windows.Media;

namespace NONIVERUS.BinaryScope.Windows.Product;

public static class ThemeService
{
    public const string MidnightKey = "midnight";
    public const string GraphiteKey = "graphite";
    public const string LightPrecisionKey = "light";

    private sealed record ThemeColors(
        string Background,
        string Panel,
        string Elevated,
        string Border,
        string Grid,
        string GridAlternate,
        string GridHeader,
        string ProgressTrack,
        string ReviewSurface,
        string ReviewBorder,
        string Accent,
        string AccentText,
        string Text,
        string Muted);

    private static readonly IReadOnlyDictionary<string, ThemeColors> Themes =
        new Dictionary<string, ThemeColors>(StringComparer.OrdinalIgnoreCase)
        {
            [MidnightKey] = new(
                Background: "#07111F",
                Panel: "#0E1A2B",
                Elevated: "#17243B",
                Border: "#2A3A56",
                Grid: "#0D1728",
                GridAlternate: "#13213A",
                GridHeader: "#1B2A43",
                ProgressTrack: "#263650",
                ReviewSurface: "#251D14",
                ReviewBorder: "#8F5C28",
                Accent: "#FF7A1A",
                AccentText: "#101010",
                Text: "#F4F7FB",
                Muted: "#94A3B8"),
            [GraphiteKey] = new(
                Background: "#12161D",
                Panel: "#1B222C",
                Elevated: "#28313D",
                Border: "#3B4654",
                Grid: "#171D25",
                GridAlternate: "#202833",
                GridHeader: "#303A47",
                ProgressTrack: "#354150",
                ReviewSurface: "#2B241B",
                ReviewBorder: "#9A6A30",
                Accent: "#FF7A1A",
                AccentText: "#111111",
                Text: "#F6F7F9",
                Muted: "#A9B0BA"),
            [LightPrecisionKey] = new(
                Background: "#F4F7FB",
                Panel: "#FFFFFF",
                Elevated: "#E9EEF5",
                Border: "#CBD5E1",
                Grid: "#FFFFFF",
                GridAlternate: "#F4F7FB",
                GridHeader: "#E5EAF1",
                ProgressTrack: "#D7DEE8",
                ReviewSurface: "#FFF6E7",
                ReviewBorder: "#E3A13F",
                Accent: "#FF7A1A",
                AccentText: "#111827",
                Text: "#101828",
                Muted: "#667085")
        };

    public static bool IsSupported(string? themeKey) =>
        !string.IsNullOrWhiteSpace(themeKey) && Themes.ContainsKey(themeKey);

    public static void Apply(string? themeKey)
    {
        var key = IsSupported(themeKey) ? themeKey! : MidnightKey;
        var theme = Themes[key];

        SetBrush("NavyBrush", theme.Background);
        SetBrush("PanelBrush", theme.Panel);
        SetBrush("ElevatedBrush", theme.Elevated);
        SetBrush("BorderBrush", theme.Border);
        SetBrush("GridBrush", theme.Grid);
        SetBrush("GridAlternateBrush", theme.GridAlternate);
        SetBrush("GridHeaderBrush", theme.GridHeader);
        SetBrush("ProgressTrackBrush", theme.ProgressTrack);
        SetBrush("ReviewSurfaceBrush", theme.ReviewSurface);
        SetBrush("ReviewBorderBrush", theme.ReviewBorder);
        SetBrush("AccentBrush", theme.Accent);
        SetBrush("AccentTextBrush", theme.AccentText);
        SetBrush("TextBrush", theme.Text);
        SetBrush("MutedBrush", theme.Muted);
    }

    private static void SetBrush(string key, string colorValue)
    {
        if (Application.Current is null)
            return;

        var color = (Color)ColorConverter.ConvertFromString(colorValue);

        if (Application.Current.Resources[key] is SolidColorBrush existing)
        {
            if (existing.IsFrozen)
                Application.Current.Resources[key] = new SolidColorBrush(color);
            else
                existing.Color = color;
        }
        else
        {
            Application.Current.Resources[key] = new SolidColorBrush(color);
        }
    }
}
