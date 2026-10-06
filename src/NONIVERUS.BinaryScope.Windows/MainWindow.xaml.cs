using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using NONIVERUS.BinaryScope.Analysis;
using NONIVERUS.BinaryScope.Models;
using NONIVERUS.BinaryScope.Reporting;
using NONIVERUS.BinaryScope.Windows.Product;

namespace NONIVERUS.BinaryScope.Windows;

public partial class MainWindow : Window
{
    private readonly AabAnalyzer _analyzer = new();
    private AabAnalysis? _lastAnalysis;

    public MainWindow()
    {
        InitializeComponent();
        WindowState = WindowState.Maximized;
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        // Product requirement: BinaryScope operates maximized.
        // Minimize remains allowed; any restore-to-normal state is immediately maximized.
        if (WindowState == WindowState.Normal)
            WindowState = WindowState.Maximized;
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open Android App Bundle",
            Filter = "Android App Bundle (*.aab)|*.aab",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) == true)
            Analyze(dialog.FileName);
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is not string[] files || files.Length != 1)
            return;
        Analyze(files[0]);
    }

    private void Analyze(string path)
    {
        try
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var analysis = _analyzer.Analyze(path);
            _lastAnalysis = analysis;

            BundleNameText.Text = analysis.FileName;
            BundleSizeText.Text = $"AAB {FormatMb(analysis.BundleBytes)} · {analysis.BundleBytes:N0} bytes";

            DexSizeText.Text = FormatMb(analysis.TotalDexBytes);
            DexRawBytesText.Text = $"{analysis.TotalDexBytes:N0} bytes";
            CompressedDexText.Text = FormatMb(analysis.TotalDexCompressedBytes);
            CompressedDexBytesText.Text = $"{analysis.TotalDexCompressedBytes:N0} bytes";
            DexCountText.Text = analysis.DexFiles.Count.ToString();
            ModulesTopText.Text = $"{analysis.Modules.Count} module{(analysis.Modules.Count == 1 ? string.Empty : "s")}";

            var applicabilityLabel = GetApplicabilityLabel(analysis.GooglePolicy.Applicability);
            GateText.Text = applicabilityLabel;
            GateMarginTopText.Text = GetThresholdMarginText(analysis);
            VerdictText.Text = analysis.GooglePolicy.LocalVerdict;

            UniqueMethodRefsText.Text = analysis.UniqueMethodReferences.ToString("N0");
            DefinedMethodsText.Text = analysis.TotalDefinedMethods.ToString("N0");
            MethodsWithCodeText.Text = analysis.TotalMethodsWithCode.ToString("N0");
            HeaderMethodIdsText.Text = analysis.TotalMethodIds.ToString("N0");
            ClassDefsText.Text = analysis.TotalClassDefs.ToString("N0");
            ModuleProofText.Text = analysis.Evidence.ModulesProvenByManifest ? "MANIFEST-PROVEN" : "DEX FALLBACK · UNPROVEN";

            DexGrid.ItemsSource = analysis.DexFiles;
            InventorySummaryText.Text = $"{analysis.DexFiles.Count} DEX · {analysis.Modules.Count} module{(analysis.Modules.Count == 1 ? string.Empty : "s")}";

            var threshold = analysis.GooglePolicy.GoogleAppThresholdBytes;
            var utilization = threshold <= 0 ? 0d : analysis.TotalDexBytes / (double)threshold;
            ThresholdUsageText.Text = $"{FormatMb(analysis.TotalDexBytes)} / {FormatMb(threshold)}";
            ThresholdPercentText.Text = $"{utilization:P1}";
            ThresholdProgress.Value = Math.Clamp(utilization * 100d, 0d, 100d);
            ThresholdMarginText.Text = GetThresholdMarginText(analysis);
            PolicyBoundariesText.Text =
                $"Advisory: {analysis.GooglePolicy.NoniverusAdvisoryBytes:N0} bytes · " +
                $"Google threshold: {analysis.GooglePolicy.GoogleAppThresholdBytes:N0} bytes";
            StoreMetricsText.Text = analysis.GooglePolicy.StoreMetricsStatus;

            NativeRawText.Text = FormatMb(analysis.TotalNativeBinaryBytes);
            NativeCompressedText.Text = FormatMb(analysis.TotalNativeBinaryCompressedBytes);
            NativeAbiText.Text = analysis.NativeAbis.Count == 0 ? "0" : analysis.NativeAbis.Count.ToString("N0");
            ContributorCountText.Text = analysis.BinaryContributors.Count.ToString("N0");
            NativeBinarySummaryText.Text =
                $"{analysis.NativeBinaryEntries.Count:N0} packaged .so entries · " +
                $"{analysis.BinaryContributors.Count:N0} logical contributors · " +
                $"{analysis.NativeAbis.Count:N0} ABI{(analysis.NativeAbis.Count == 1 ? string.Empty : "s")} " +
                $"[{(analysis.NativeAbis.Count == 0 ? "none" : string.Join(", ", analysis.NativeAbis))}]";

            ContributorGrid.ItemsSource = analysis.BinaryContributors.Take(12).ToArray();

            CategorySummaryText.Text = analysis.BinaryCategories.Count == 0
                ? "No packaged native/AOT contributor categories found."
                : string.Join(
                    Environment.NewLine,
                    analysis.BinaryCategories.Take(8).Select(c =>
                        $"{c.Category}: {FormatMb(c.UncompressedBytes)} raw · {c.ContributorCount:N0} contributor{(c.ContributorCount == 1 ? string.Empty : "s")}"));

            DependencyGraphStatusText.Text = analysis.DependencyGraphStatus;

            var moduleProof = analysis.Evidence.ModulesProvenByManifest ? "manifest-proven" : "DEX fallback / UNPROVEN manifest";
            var moduleNames = analysis.Modules.Count == 0 ? "none" : string.Join(", ", analysis.Modules);
            var mappingEvidence = analysis.Evidence.ProguardMappingPresent ? "present" : "not found";
            var metadataEvidence = analysis.Evidence.BundleMetadataPresent ? "present" : "absent";

            EvidenceSummaryText.Text =
                $"{analysis.Modules.Count} module{(analysis.Modules.Count == 1 ? string.Empty : "s")} ({moduleProof}) · " +
                $"BUNDLE-METADATA {metadataEvidence} · mapping {mappingEvidence}";

            EvidenceText.Text =
                $"AAB SHA-256\n{analysis.BundleSha256}\n\n" +
                $"Modules             {analysis.Modules.Count:N0} [{moduleNames}]\n" +
                $"DEX raw             {analysis.TotalDexBytes:N0} bytes\n" +
                $"DEX compressed      {analysis.TotalDexCompressedBytes:N0} bytes\n" +
                $"Native .so entries  {analysis.Evidence.NativeLibraryEntries.Count:N0}\n" +
                $"Native raw          {analysis.TotalNativeBinaryBytes:N0} bytes\n" +
                $"Native compressed   {analysis.TotalNativeBinaryCompressedBytes:N0} bytes\n" +
                $"Native ABIs         {(analysis.NativeAbis.Count == 0 ? "none" : string.Join(", ", analysis.NativeAbis))}\n" +
                $"Contributors        {analysis.BinaryContributors.Count:N0}";

            ExportButton.IsEnabled = true;
            ExportSummaryButton.IsEnabled = true;
            CompareButton.IsEnabled = true;

            // Each new analysis starts at the policy header instead of preserving an old scroll offset.
            RightPanelScrollViewer.ScrollToTop();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "BinaryScope analysis failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void Compare_Click(object sender, RoutedEventArgs e)
    {
        if (_lastAnalysis is null)
            return;

        var dialog = new OpenFileDialog
        {
            Title = "Select AFTER Android App Bundle",
            Filter = "Android App Bundle (*.aab)|*.aab",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != true)
            return;

        try
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var after = _analyzer.Analyze(dialog.FileName);
            var comparison = AabComparator.Compare(_lastAnalysis, after);

            ProductShellHost.ShowComparison(this, _lastAnalysis, after, comparison);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "BinaryScope comparison failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        if (_lastAnalysis is null) return;
        var dialog = new SaveFileDialog
        {
            Title = "Export BinaryScope evidence",
            Filter = "JSON report (*.json)|*.json",
            FileName = Path.GetFileNameWithoutExtension(_lastAnalysis.FileName) + ".binaryscope.json"
        };
        if (dialog.ShowDialog(this) == true)
            AnalysisReportWriter.WriteJson(_lastAnalysis, dialog.FileName);
    }

    private void ExportSummary_Click(object sender, RoutedEventArgs e)
    {
        if (_lastAnalysis is null) return;

        var dialog = new SaveFileDialog
        {
            Title = "Export BinaryScope summary",
            Filter = "Text report (*.txt)|*.txt",
            FileName = Path.GetFileNameWithoutExtension(_lastAnalysis.FileName) + ".binaryscope-summary.txt"
        };

        if (dialog.ShowDialog(this) != true)
            return;

        File.WriteAllText(dialog.FileName, BuildHumanReadableSummary(_lastAnalysis), Encoding.UTF8);
    }

    private static string BuildHumanReadableSummary(AabAnalysis analysis)
    {
        var moduleProof = analysis.Evidence.ModulesProvenByManifest ? "manifest-proven" : "DEX fallback / UNPROVEN manifest";
        var moduleNames = analysis.Modules.Count == 0 ? "none" : string.Join(", ", analysis.Modules);

        var sb = new StringBuilder();
        sb.AppendLine("NONIVERUS BinaryScope — Evidence Summary");
        sb.AppendLine(new string('=', 45));
        sb.AppendLine();
        sb.AppendLine($"Bundle: {analysis.FileName}");
        sb.AppendLine($"Source: {analysis.SourcePath}");
        sb.AppendLine($"Analyzed UTC: {analysis.AnalyzedAtUtc:O}");
        sb.AppendLine($"AAB bytes: {analysis.BundleBytes:N0}");
        sb.AppendLine($"AAB SHA-256: {analysis.BundleSha256}");
        sb.AppendLine();
        sb.AppendLine("DEX MEASUREMENT");
        sb.AppendLine($"DEX files: {analysis.DexFiles.Count:N0}");
        sb.AppendLine($"DEX raw: {analysis.TotalDexBytes:N0} bytes ({FormatMb(analysis.TotalDexBytes)})");
        sb.AppendLine($"DEX compressed: {analysis.TotalDexCompressedBytes:N0} bytes ({FormatMb(analysis.TotalDexCompressedBytes)})");
        sb.AppendLine($"Method IDs (header sum): {analysis.TotalMethodIds:N0}");
        sb.AppendLine($"Unique method references: {analysis.UniqueMethodReferences:N0}");
        sb.AppendLine($"Class definitions: {analysis.TotalClassDefs:N0}");
        sb.AppendLine($"Defined methods: {analysis.TotalDefinedMethods:N0}");
        sb.AppendLine($"Methods with code: {analysis.TotalMethodsWithCode:N0}");
        sb.AppendLine();
        sb.AppendLine("MODULE / PACKAGE EVIDENCE");
        sb.AppendLine($"Modules: {analysis.Modules.Count:N0} ({moduleProof}) [{moduleNames}]");
        sb.AppendLine($"BUNDLE-METADATA: {(analysis.Evidence.BundleMetadataPresent ? "present" : "absent")}");
        sb.AppendLine($"ProGuard/R8 mapping evidence: {(analysis.Evidence.ProguardMappingPresent ? "present" : "not found")}");
        sb.AppendLine($"Native .so entries: {analysis.Evidence.NativeLibraryEntries.Count:N0}");
        sb.AppendLine();
        sb.AppendLine("BINARY CONTRIBUTORS — PACKAGED AAB FOOTPRINT");
        sb.AppendLine($"Native/AOT raw: {analysis.TotalNativeBinaryBytes:N0} bytes ({FormatMb(analysis.TotalNativeBinaryBytes)})");
        sb.AppendLine($"Native/AOT compressed: {analysis.TotalNativeBinaryCompressedBytes:N0} bytes ({FormatMb(analysis.TotalNativeBinaryCompressedBytes)})");
        sb.AppendLine($"ABIs: {analysis.NativeAbis.Count:N0} [{(analysis.NativeAbis.Count == 0 ? "none" : string.Join(", ", analysis.NativeAbis))}]");
        sb.AppendLine($"Logical contributors: {analysis.BinaryContributors.Count:N0}");
        sb.AppendLine($"Packaged .so entries: {analysis.NativeBinaryEntries.Count:N0}");
        sb.AppendLine();
        sb.AppendLine("Category totals:");
        foreach (var category in analysis.BinaryCategories)
            sb.AppendLine($"  {category.Category}: {category.UncompressedBytes:N0} raw · {category.CompressedBytes:N0} compressed · {category.ContributorCount:N0} contributors");
        sb.AppendLine();
        sb.AppendLine("Top contributors:");
        foreach (var contributor in analysis.BinaryContributors.Take(15))
            sb.AppendLine($"  {contributor.ContributorName} | {contributor.Category} | {contributor.UncompressedBytes:N0} raw | {contributor.CompressedBytes:N0} compressed | {contributor.AbiCount:N0} ABI(s)");
        sb.AppendLine();
        sb.AppendLine($"Dependency graph: {analysis.DependencyGraphStatus}");
        sb.AppendLine();
        sb.AppendLine("LOCAL POLICY PREFLIGHT");
        sb.AppendLine($"Applicability: {GetApplicabilityLabel(analysis.GooglePolicy.Applicability)}");
        sb.AppendLine($"Local verdict: {analysis.GooglePolicy.LocalVerdict}");
        sb.AppendLine($"NONIVERUS advisory boundary: {analysis.GooglePolicy.NoniverusAdvisoryBytes:N0} bytes");
        sb.AppendLine($"Google DEX threshold: {analysis.GooglePolicy.GoogleAppThresholdBytes:N0} bytes");
        sb.AppendLine($"Store metrics: {analysis.GooglePolicy.StoreMetricsStatus}");
        sb.AppendLine();
        sb.AppendLine("Important: local BinaryScope measurements do not prove Play Console shrinking, optimization or obfuscation percentages.");
        return sb.ToString();
    }

    private static string GetApplicabilityLabel(GoogleDexApplicability applicability) => applicability switch
    {
        GoogleDexApplicability.ComfortableMargin => "COMFORTABLE MARGIN",
        GoogleDexApplicability.ApproachingThreshold => "APPROACHING",
        GoogleDexApplicability.RuleApplies => "RULE APPLIES",
        _ => "UNVERIFIED"
    };

    private static string GetThresholdMarginText(AabAnalysis analysis)
    {
        var delta = analysis.GooglePolicy.GoogleAppThresholdBytes - analysis.TotalDexBytes;
        if (delta > 0)
            return $"{delta:N0} bytes below 10,000,000-byte threshold";
        if (delta < 0)
            return $"{Math.Abs(delta):N0} bytes above 10,000,000-byte threshold";
        return "Exactly at 10,000,000 bytes — rule applies only when DEX is greater than threshold.";
    }

    private static string FormatMb(long bytes) => $"{bytes / 1_000_000d:N2} MB";
}
