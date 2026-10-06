using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NONIVERUS.BinaryScope.Windows.Product;

namespace NONIVERUS.BinaryScope.Windows;

public partial class OnboardingWindow : Window
{
    private const int LastStepIndex = 5;

    private readonly ProductPreferences _working;
    private int _stepIndex;

    private static readonly (string TitleKey, string BodyKey)[] Steps =
    [
        ("Onboarding.Welcome.Title", "Onboarding.Welcome.Body"),
        ("Onboarding.Privacy.Title", "Onboarding.Privacy.Body"),
        ("Onboarding.Compare.Title", "Onboarding.Compare.Body"),
        ("Onboarding.Verdict.Title", "Onboarding.Verdict.Body"),
        ("Onboarding.Choose.Title", "Onboarding.Choose.Body"),
        ("Onboarding.Ready.Title", "Onboarding.Ready.Body")
    ];

    public OnboardingWindow(ProductPreferences preferences)
    {
        _working = preferences.Clone();

        InitializeComponent();
        WindowState = WindowState.Maximized;

        LanguageCombo.ItemsSource = ProductCatalog.Languages;
        ThemeCombo.ItemsSource = ProductCatalog.Themes;

        LanguageCombo.SelectedValue = ProductCatalog.IsSupportedLanguage(_working.LanguageCode)
            ? _working.LanguageCode
            : "en";

        ThemeCombo.SelectedValue = ThemeService.IsSupported(_working.ThemeKey)
            ? _working.ThemeKey
            : ThemeService.MidnightKey;

        Refresh();
    }

    public ProductPreferences ResultPreferences => _working.Clone();

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        if (_stepIndex == 0)
            return;

        _stepIndex--;
        Refresh();
    }

    private void Next_Click(object sender, RoutedEventArgs e)
    {
        if (_stepIndex < LastStepIndex)
        {
            _stepIndex++;
            Refresh();
            return;
        }

        _working.OnboardingCompleted = true;
        _working.LastSeenWhatsNewVersion = ProductCatalog.CurrentWhatsNewVersion;

        DialogResult = true;
        Close();
    }

    private void LanguageCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LanguageCombo.SelectedValue is not string languageCode)
            return;

        _working.LanguageCode = languageCode;
        RefreshTextOnly();
    }

    private void ThemeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeCombo.SelectedValue is not string themeKey)
            return;

        _working.ThemeKey = themeKey;
        ThemeService.Apply(themeKey);
    }

    private void Refresh()
    {
        RefreshTextOnly();

        SelectionPanel.Visibility = _stepIndex == 4 ? Visibility.Visible : Visibility.Collapsed;
        ReadyPanel.Visibility = _stepIndex == 5 ? Visibility.Visible : Visibility.Collapsed;

        BackButton.IsEnabled = _stepIndex > 0;

        var dots = new[] { Dot0, Dot1, Dot2, Dot3, Dot4, Dot5 };
        for (var i = 0; i < dots.Length; i++)
        {
            dots[i].Fill = (Brush)FindResource(i == _stepIndex ? "AccentBrush" : "BorderBrush");
        }
    }

    private void RefreshTextOnly()
    {
        var language = _working.LanguageCode;
        var step = Steps[_stepIndex];

        Title = ProductCatalog.ProductName + " — " + LocalizationService.T(language, "Onboarding.Welcome.Title");
        StepText.Text = LocalizationService.Format(language, "Onboarding.Step", _stepIndex + 1, Steps.Length);
        TitleText.Text = LocalizationService.T(language, step.TitleKey);
        BodyText.Text = LocalizationService.T(language, step.BodyKey);
        LanguageLabelText.Text = LocalizationService.T(language, "Onboarding.Language");
        ThemeLabelText.Text = LocalizationService.T(language, "Onboarding.Theme");
        LocalNoteText.Text = LocalizationService.T(language, "Onboarding.LocalNote");
        ReadyEvidenceText.Text = LocalizationService.T(language, "Onboarding.ReadyEvidence");
        BackButton.Content = LocalizationService.T(language, "Common.Back");
        NextButton.Content = LocalizationService.T(
            language,
            _stepIndex == LastStepIndex ? "Common.Start" : "Common.Next");
    }
}
