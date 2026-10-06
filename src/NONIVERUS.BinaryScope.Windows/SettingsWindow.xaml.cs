using System.Windows.Controls;
using NONIVERUS.BinaryScope.Windows.Product;

namespace NONIVERUS.BinaryScope.Windows;

public partial class SettingsWindow : UserControl
{
    private readonly ProductPreferences _original;
    private readonly ProductPreferences _working;

    public SettingsWindow(ProductPreferences preferences)
    {
        _original = preferences.Clone();
        _working = preferences.Clone();

        InitializeComponent();

        LanguageCombo.ItemsSource = ProductCatalog.Languages;
        ThemeCombo.ItemsSource = ProductCatalog.Themes;

        LanguageCombo.SelectedValue = ProductCatalog.IsSupportedLanguage(_working.LanguageCode)
            ? _working.LanguageCode
            : "en";

        ThemeCombo.SelectedValue = ThemeService.IsSupported(_working.ThemeKey)
            ? _working.ThemeKey
            : ThemeService.MidnightKey;

        RefreshText();
    }

    public event Action<ProductPreferences>? Saved;
    public event Action? Cancelled;
    public event Action? OpenWhatsNewRequested;

    public void CancelPreview() => ThemeService.Apply(_original.ThemeKey);

    private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LanguageCombo.SelectedValue is not string languageCode)
            return;
        _working.LanguageCode = languageCode;
        RefreshText();
    }

    private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeCombo.SelectedValue is not string themeKey)
            return;
        _working.ThemeKey = themeKey;
        ThemeService.Apply(themeKey);
    }

    private void WhatsNew_Click(object sender, System.Windows.RoutedEventArgs e) =>
        OpenWhatsNewRequested?.Invoke();

    private void Save_Click(object sender, System.Windows.RoutedEventArgs e) =>
        Saved?.Invoke(_working.Clone());

    private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CancelPreview();
        Cancelled?.Invoke();
    }

    private void RefreshText()
    {
        var language = _working.LanguageCode;
        HeaderText.Text = LocalizationService.T(language, "Settings.Title");
        SubtitleText.Text = LocalizationService.T(language, "Settings.Subtitle");
        LanguageLabelText.Text = LocalizationService.T(language, "Settings.Language");
        ThemeLabelText.Text = LocalizationService.T(language, "Settings.Theme");
        LocalNoteText.Text = LocalizationService.T(language, "Settings.LocalNote");
        VersionText.Text = LocalizationService.Format(language, "Settings.Version", ProductCatalog.CurrentProductVersion);
        WhatsNewButton.Content = LocalizationService.T(language, "Settings.WhatsNew");
        SaveButton.Content = LocalizationService.T(language, "Common.Save");
        CancelButton.Content = LocalizationService.T(language, "Common.Cancel");
    }
}
