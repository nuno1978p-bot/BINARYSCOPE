using System.Windows;
using NONIVERUS.BinaryScope.Windows.Product;

namespace NONIVERUS.BinaryScope.Windows;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        var preferences = ProductPreferencesStore.Load();
        ThemeService.Apply(preferences.ThemeKey);

        var forceOnboarding = string.Equals(
            Environment.GetEnvironmentVariable("NONIVERUS_BINARYSCOPE_FORCE_ONBOARDING"),
            "1",
            StringComparison.Ordinal);

        var forceWhatsNew = string.Equals(
            Environment.GetEnvironmentVariable("NONIVERUS_BINARYSCOPE_FORCE_WHATSNEW"),
            "1",
            StringComparison.Ordinal);

        var onboardingCompletedThisRun = false;

        if (!preferences.OnboardingCompleted || forceOnboarding)
        {
            var onboarding = new OnboardingWindow(preferences);
            var accepted = onboarding.ShowDialog() == true;

            if (!accepted)
            {
                Shutdown();
                return;
            }

            preferences = onboarding.ResultPreferences;
            preferences.OnboardingCompleted = true;
            preferences.LastSeenWhatsNewVersion = ProductCatalog.CurrentWhatsNewVersion;
            ProductPreferencesStore.Save(preferences);
            ThemeService.Apply(preferences.ThemeKey);
            onboardingCompletedThisRun = true;
        }

        var mainWindow = new MainWindow();
        ProductShellHost.Attach(mainWindow, preferences);

        MainWindow = mainWindow;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        mainWindow.Loaded += (_, _) =>
        {
            var shouldShow = forceWhatsNew ||
                             (!onboardingCompletedThisRun && ProductCatalog.ShouldShowWhatsNew(preferences));

            if (!shouldShow)
                return;

            ProductShellHost.ShowWhatsNew(mainWindow);
        };

        mainWindow.Show();
    }
}
