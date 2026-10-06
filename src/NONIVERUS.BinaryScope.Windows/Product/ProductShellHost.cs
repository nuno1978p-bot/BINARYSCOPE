using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NONIVERUS.BinaryScope.Windows.Product;

public static class ProductShellHost
{
    public static void Attach(MainWindow window, ProductPreferences preferences)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(preferences);

        if (window.Content is not UIElement originalContent)
            return;

        window.Content = null;

        var shell = new Grid();
        shell.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        shell.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        var toolbar = BuildToolbar(window, preferences);
        Grid.SetRow(toolbar, 0);
        shell.Children.Add(toolbar);

        Grid.SetRow(originalContent, 1);
        shell.Children.Add(originalContent);

        window.Content = shell;
    }

    private static Border BuildToolbar(MainWindow owner, ProductPreferences preferences)
    {
        var brandText = new TextBlock
        {
            FontSize = 15,
            FontWeight = FontWeights.SemiBold,
            VerticalAlignment = VerticalAlignment.Center
        };
        brandText.SetResourceReference(TextBlock.ForegroundProperty, "TextBrush");

        var descriptorText = new TextBlock
        {
            Margin = new Thickness(12, 0, 0, 0),
            FontSize = 12,
            VerticalAlignment = VerticalAlignment.Center
        };
        descriptorText.SetResourceReference(TextBlock.ForegroundProperty, "MutedBrush");

        var versionText = new TextBlock
        {
            Margin = new Thickness(16, 0, 0, 0),
            FontFamily = new FontFamily("Consolas"),
            FontSize = 12,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            Text = "v" + ProductCatalog.CurrentProductVersion
        };
        versionText.SetResourceReference(TextBlock.ForegroundProperty, "AccentBrush");

        var whatsNewButton = CreateSecondaryButton();
        var settingsButton = CreateSecondaryButton();
        settingsButton.Margin = new Thickness(10, 0, 0, 0);

        void RefreshLabels()
        {
            brandText.Text = ProductCatalog.ProductName;
            descriptorText.Text = LocalizationService.T(preferences.LanguageCode, "Main.Subtitle");
            whatsNewButton.Content = LocalizationService.T(preferences.LanguageCode, "Main.WhatsNew");
            settingsButton.Content = LocalizationService.T(preferences.LanguageCode, "Main.Settings");
        }

        whatsNewButton.Click += (_, _) =>
        {
            var dialog = new WhatsNewWindow(preferences.LanguageCode)
            {
                Owner = owner
            };

            dialog.ShowDialog();
            preferences.LastSeenWhatsNewVersion = ProductCatalog.CurrentWhatsNewVersion;
            ProductPreferencesStore.Save(preferences);
        };

        settingsButton.Click += (_, _) =>
        {
            var dialog = new SettingsWindow(preferences)
            {
                Owner = owner
            };

            if (dialog.ShowDialog() != true)
                return;

            var result = dialog.ResultPreferences;
            preferences.LanguageCode = result.LanguageCode;
            preferences.ThemeKey = result.ThemeKey;
            preferences.OnboardingCompleted = result.OnboardingCompleted;
            preferences.LastSeenWhatsNewVersion = result.LastSeenWhatsNewVersion;

            ThemeService.Apply(preferences.ThemeKey);
            ProductPreferencesStore.Save(preferences);
            RefreshLabels();
        };

        RefreshLabels();

        var left = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };
        left.Children.Add(brandText);
        left.Children.Add(descriptorText);
        left.Children.Add(versionText);

        var right = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            VerticalAlignment = VerticalAlignment.Center
        };
        right.Children.Add(whatsNewButton);
        right.Children.Add(settingsButton);

        var layout = new Grid();
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        layout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Grid.SetColumn(left, 0);
        Grid.SetColumn(right, 1);
        layout.Children.Add(left);
        layout.Children.Add(right);

        var border = new Border
        {
            Padding = new Thickness(28, 10, 28, 10),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Child = layout
        };
        border.SetResourceReference(Border.BackgroundProperty, "PanelBrush");
        border.SetResourceReference(Border.BorderBrushProperty, "BorderBrush");
        return border;
    }

    private static Button CreateSecondaryButton()
    {
        var button = new Button
        {
            Padding = new Thickness(14, 7, 14, 7),
            MinWidth = 110,
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
