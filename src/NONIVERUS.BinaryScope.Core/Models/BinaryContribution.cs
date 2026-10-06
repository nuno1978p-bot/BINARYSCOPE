namespace NONIVERUS.BinaryScope.Models;

public sealed record NativeBinaryEntryAnalysis(
    string EntryName,
    string Module,
    string Abi,
    string FileName,
    string ContributorName,
    string Category,
    long UncompressedBytes,
    long CompressedBytes);

public sealed record BinaryContributorSummary(
    string ContributorName,
    string Category,
    long UncompressedBytes,
    long CompressedBytes,
    int EntryCount,
    int AbiCount,
    IReadOnlyList<string> Abis);

public sealed record BinaryCategorySummary(
    string Category,
    long UncompressedBytes,
    long CompressedBytes,
    int ContributorCount,
    int EntryCount);
