using Microsoft.Win32;
using System.Windows;
using NONIVERUS.BinaryScope.Models;
using NONIVERUS.BinaryScope.Reporting;

namespace NONIVERUS.BinaryScope.Windows;

public partial class CompareWindow : Window
{
    private readonly AabComparison _comparison;

    public CompareWindow(AabAnalysis before, AabAnalysis after, AabComparison comparison)
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;

        _comparison = comparison;

        BeforeNameText.Text = before.FileName;
        BeforeShaText.Text = before.BundleSha256;
        AfterNameText.Text = after.FileName;
        AfterShaText.Text = after.BundleSha256;

        MetricsGrid.ItemsSource = comparison.Metrics;
        CategoryGrid.ItemsSource = comparison.CategoryChanges;
        ContributorChangesGrid.ItemsSource = comparison.ContributorChanges;

        AbiDeltaText.Text =
            $"Added ABIs: {(comparison.AddedAbis.Count == 0 ? "none" : string.Join(", ", comparison.AddedAbis))} · " +
            $"Removed ABIs: {(comparison.RemovedAbis.Count == 0 ? "none" : string.Join(", ", comparison.RemovedAbis))}";

        ContributorSummaryText.Text =
            $"{comparison.ContributorChanges.Count:N0} changed contributors · " +
            $"{comparison.CategoryChanges.Count:N0} changed categories";

        DependencyGraphText.Text = "Dependency graph: " + comparison.DependencyGraphStatus;

        GateVerdictText.Text = comparison.Intelligence.Verdict;
        GatePolicyText.Text =
            $"Internal review thresholds · DEX +{comparison.Intelligence.Policy.DexGrowthReviewPercent:N0}% · " +
            $"AAB +{comparison.Intelligence.Policy.AabGrowthReviewPercent:N0}% · " +
            $"Native +{comparison.Intelligence.Policy.NativeGrowthReviewPercent:N0}%";

        GateFindingsText.Text = comparison.Intelligence.Findings.Count == 0
            ? "none"
            : string.Join(" · ", comparison.Intelligence.Findings.Select(f => $"[{f.Severity}] {f.Title}"));

        GateImprovementsText.Text = comparison.Intelligence.Improvements.Count == 0
            ? "none"
            : string.Join(" · ", comparison.Intelligence.Improvements.Select(i => i.Title));
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
    }

    private void ExportJson_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export BinaryScope comparison JSON",
            Filter = "JSON report (*.json)|*.json",
            FileName = "binaryscope-compare.json"
        };

        if (dialog.ShowDialog(this) == true)
            ComparisonReportWriter.WriteJson(_comparison, dialog.FileName);
    }

    private void ExportText_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Export BinaryScope comparison summary",
            Filter = "Text report (*.txt)|*.txt",
            FileName = "binaryscope-compare.txt"
        };

        if (dialog.ShowDialog(this) == true)
            ComparisonReportWriter.WriteText(_comparison, dialog.FileName);
    }
}
