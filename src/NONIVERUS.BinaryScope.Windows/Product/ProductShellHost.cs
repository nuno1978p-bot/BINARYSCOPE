using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NONIVERUS.BinaryScope.Models;

namespace NONIVERUS.BinaryScope.Windows.Product;

public static class ProductShellHost
{
    private static readonly ConditionalWeakTable<MainWindow, ShellState> States = new();

    public static void Attach(MainWindow window, ProductPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(preferences);

        if (window.Content is not UIElement originalContent)
            return;

        window.Content = null;
        var state = new ShellState(window, preferences, originalContent);
        States.Remove(window);
        States.Add(window, state);
        window.Content = state.Root;
        state.ShowAnalyze();
    }

    public static void ShowComparison(MainWindow window, AabAnalysis before, AabAnalysis after, AabComparison comparison)
    {
        if (!States.TryGetValue(window, out var state))
            throw new InvalidOperationException("BinaryScope dashboard shell is not attached.");
        state.ShowComparison(before, after, comparison);
    }

    public static void ShowWhatsNew(MainWindow window)
    {
        if (!States.TryGetValue(window, out var state))
            throw new InvalidOperationException("BinaryScope dashboard shell is not attached.");
        state.ShowWhatsNew();
    }

    private sealed class ShellState
    {
        private readonly MainWindow _window;
        private readonly ProductPreferences _preferences;
        private readonly UIElement _analyzeContent;
        private readonly ContentControl _workspace = new();
        private readonly TextBlock _brandText = new();
        private readonly TextBlock _descriptorText = new();
        private readonly TextBlock _versionText = new();
        private readonly Button _analyzeButton = CreateSecondaryButton();
        private readonly Button _whatsNewButton = CreateSecondaryButton();
        private readonly Button _settingsButton = CreateSecondaryButton();

        public ShellState(MainWindow window, ProductPreferences preferences, UIElement analyzeContent)
        {
            _window = window;
            _preferences = preferences;
            _analyzeContent = analyzeContent;

            _brandText.FontSize = 15;
            _brandText.FontWeight = FontWeights.SemiBold;
            _brandText.VerticalAlignment = VerticalAlignment.Center;
            _brandText.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");

            _descriptorText.Margin = new Thickness(12, 0, 0, 0);
            _descriptorText.FontSize = 12;
            _descriptorText.VerticalAlignment = VerticalAlignment.Center;
            _descriptorText.SetResourceReference(TextBlock.ForegroundProperty, "MutedBrush");

            _versionText.Margin = new Thickness(16, 0, 0, 0);
            _versionText.FontFamily = new FontFamily("Consolas");
            _versionText.FontSize = 12;
            _versionText.FontWeight = FontWeights.Bold;
            _versionText.VerticalAlignment = VerticalAlignment.Center;
            _versionText.Text = "v" + ProductCatalog.CurrentProductVersion;
            _versionText.SetResourceReference(TextBlock.ForegroundProperty, "AccentBrush");

            _whatsNewButton.Margin = new Thickness(10, 0, 0, 0);
            _settingsButton.Margin = new Thickness(10, 0, 0, 0);

            _analyzeButton.Click += (_, _) => ShowAnalyze();
            _whatsNewButton.Click += (_, _) => ShowWhatsNew();
            _settingsButton.Click += (_, _) => ShowSettings();

            Root = BuildRoot();
            RefreshLabels();
        }

        public Grid Root { get; }

        public void ShowAnalyze()
        {
            _workspace.Content = _analyzeContent;
            RefreshLabels();
            MainWindowProductLocalization.Apply(_window, _preferences.LanguageCode);
        }

        public void ShowComparison(AabAnalysis before, AabAnalysis after, AabComparison comparison)
        {
            _workspace.Content = new CompareWindow(before, after, comparison);
            RefreshLabels();
        }

        public void ShowWhatsNew()
        {
            var view = new WhatsNewWindow(_preferences.LanguageCode);
            view.CloseRequested += () =>
            {
                _preferences.LastSeenWhatsNewVersion = ProductCatalog.CurrentWhatsNewVersion;
                ProductPreferencesStore.Save(_preferences);
                ShowAnalyze();
            };

            _workspace.Content = view;
            _preferences.LastSeenWhatsNewVersion = ProductCatalog.CurrentWhatsNewVersion;
            ProductPreferencesStore.Save(_preferences);
            RefreshLabels();
        }

        private void ShowSettings()
        {
            var view = new SettingsWindow(_preferences);

            view.Saved += saved =>
            {
                _preferences.LanguageCode = saved.LanguageCode;
                _preferences.ThemeKey = saved.ThemeKey;
                _preferences.OnboardingCompleted = saved.OnboardingCompleted;
                _preferences.LastSeenWhatsNewVersion = saved.LastSeenWhatsNewVersion;

                ThemeService.Apply(_preferences.ThemeKey);
                ProductPreferencesStore.Save(_preferences);
                RefreshLabels();
                MainWindowProductLocalization.Apply(_window, _preferences.LanguageCode);
                ShowAnalyze();
            };

            view.Cancelled += ShowAnalyze;
            view.OpenWhatsNewRequested += () =>
            {
                view.CancelPreview();
                ShowWhatsNew();
            };

            _workspace.Content = view;
            RefreshLabels();
        }

        private Grid BuildRoot()
        {
            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var left = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            left.Children.Add(_brandText);
            left.Children.Add(_descriptorText);
            left.Children.Add(_versionText);

            var right = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
            right.Children.Add(_analyzeButton);
            right.Children.Add(_whatsNewButton);
            right.Children.Add(_settingsButton);

            var toolbarGrid = new Grid();
            toolbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            toolbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Grid.SetColumn(left, 0);
            Grid.SetColumn(right, 1);
            toolbarGrid.Children.Add(left);
            toolbarGrid.Children.Add(right);

            var toolbar = new Border
            {
                Padding = new Thickness(28, 10, 28, 10),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Child = toolbarGrid
            };
            toolbar.SetResourceReference(Border.BackgroundProperty, "PanelBrush");
            toolbar.SetResourceReference(Border.BorderBrushProperty, "BorderBrush");

            Grid.SetRow(toolbar, 0);
            Grid.SetRow(_workspace, 1);
            root.Children.Add(toolbar);
            root.Children.Add(_workspace);
            return root;
        }

        private void RefreshLabels()
        {
            _brandText.Text = ProductCatalog.ProductName;
            _descriptorText.Text = LocalizationService.T(_preferences.LanguageCode, "Main.Subtitle");
            _analyzeButton.Content = ShellNavigationLocalization.Analyze(_preferences.LanguageCode);
            _whatsNewButton.Content = LocalizationService.T(_preferences.LanguageCode, "Main.WhatsNew");
            _settingsButton.Content = LocalizationService.T(_preferences.LanguageCode, "Main.Settings");
        }
    }

    private static Button CreateSecondaryButton()
    {
        var button = new Button
        {
            Padding = new Thickness(14, 7, 14, 7),
            MinWidth = 105,
            BorderThickness = new Thickness(1),
            FontWeight = FontWeights.SemiBold,
            Cursor = System.Windows.Input.Cursors.Hand
        };
        button.SetResourceReference(Control.BackgroundProperty, "ElevatedBrush");
        button.SetResourceReference(Control.ForegroundProperty, "TextBrush");
        button.SetResourceReference(Control.BorderBrushProperty, "BorderBrush");
        return button;
    }
}
