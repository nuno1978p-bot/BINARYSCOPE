namespace NONIVERUS.BinaryScope.Models;

public sealed record AabEvidence(
    bool BundleMetadataPresent,
    bool ProguardMappingPresent,
    IReadOnlyList<string> MappingEntries,
    IReadOnlyList<string> MetadataEntries,
    IReadOnlyList<string> NativeLibraryEntries,
    IReadOnlyList<string> ModuleManifestEntries,
    bool ModulesProvenByManifest);

public sealed record AabAnalysis(
    string SourcePath,
    string FileName,
    long BundleBytes,
    string BundleSha256,
    DateTimeOffset AnalyzedAtUtc,
    IReadOnlyList<string> Modules,
    IReadOnlyList<DexFileAnalysis> DexFiles,
    long TotalDexBytes,
    long TotalDexCompressedBytes,
    ulong TotalMethodIds,
    ulong UniqueMethodReferences,
    ulong TotalClassDefs,
    ulong TotalDefinedMethods,
    ulong TotalMethodsWithCode,
    IReadOnlyList<NativeBinaryEntryAnalysis> NativeBinaryEntries,
    IReadOnlyList<BinaryContributorSummary> BinaryContributors,
    IReadOnlyList<BinaryCategorySummary> BinaryCategories,
    IReadOnlyList<string> NativeAbis,
    long TotalNativeBinaryBytes,
    long TotalNativeBinaryCompressedBytes,
    string DependencyGraphStatus,
    AabEvidence Evidence,
    GoogleDexPolicyResult GooglePolicy);
