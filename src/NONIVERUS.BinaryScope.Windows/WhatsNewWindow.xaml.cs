using System.Windows.Controls;
using NONIVERUS.BinaryScope.Windows.Product;

namespace NONIVERUS.BinaryScope.Windows;

public sealed record WhatsNewDisplayItem(string Title, string Body);

public partial class WhatsNewWindow : UserControl
{
    private readonly string _languageCode;

    public WhatsNewWindow(string languageCode)
    {
        _languageCode = languageCode;
        InitializeComponent();
        VersionBadgeText.Text = "v" + ProductCatalog.CurrentProductVersion;
        RefreshContent();
    }

    public event Action? CloseRequested;

    private void Close_Click(object sender, System.Windows.RoutedEventArgs e) =>
        CloseRequested?.Invoke();

    private void RefreshContent()
    {
        HeaderText.Text = LocalizationService.T(_languageCode, "WhatsNew.Title");
        SubtitleText.Text = LocalizationService.Format(
            _languageCode, "WhatsNew.Subtitle", ProductCatalog.CurrentProductVersion);
        NoteText.Text = LocalizationService.T(_languageCode, "WhatsNew.Note");
        CloseButton.Content = LocalizationService.T(_languageCode, "Common.Close");

        EntriesControl.ItemsSource = ProductCatalog.CurrentWhatsNewEntries
            .Select(x => new WhatsNewDisplayItem(
                LocalizationService.T(_languageCode, x.TitleKey),
                LocalizationService.T(_languageCode, x.BodyKey)))
            .ToArray();
    }
}
