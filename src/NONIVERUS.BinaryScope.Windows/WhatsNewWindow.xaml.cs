using System.Windows;
using NONIVERUS.BinaryScope.Windows.Product;

namespace NONIVERUS.BinaryScope.Windows;

public sealed record WhatsNewDisplayItem(string Title, string Body);

public partial class WhatsNewWindow : Window
{
    private readonly string _languageCode;

    public WhatsNewWindow(string languageCode)
    {
        _languageCode = languageCode;

        InitializeComponent();
        WindowState = WindowState.Maximized;

        VersionBadgeText.Text = "v" + ProductCatalog.CurrentProductVersion;
        RefreshContent();
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void RefreshContent()
    {
        HeaderText.Text = LocalizationService.T(_languageCode, "WhatsNew.Title");
        SubtitleText.Text = LocalizationService.Format(
            _languageCode,
            "WhatsNew.Subtitle",
            ProductCatalog.CurrentProductVersion);

        NoteText.Text = LocalizationService.T(_languageCode, "WhatsNew.Note");
        CloseButton.Content = LocalizationService.T(_languageCode, "Common.Close");

        EntriesControl.ItemsSource = ProductCatalog.CurrentWhatsNewEntries
            .Select(x => new WhatsNewDisplayItem(
                LocalizationService.T(_languageCode, x.TitleKey),
                LocalizationService.T(_languageCode, x.BodyKey)))
            .ToArray();
    }
}
