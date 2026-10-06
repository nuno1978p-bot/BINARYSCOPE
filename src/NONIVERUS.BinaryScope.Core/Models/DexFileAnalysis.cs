namespace NONIVERUS.BinaryScope.Models;

public sealed record DexHeaderMetrics(
    string Version,
    uint DeclaredFileSize,
    uint HeaderSize,
    uint StringIds,
    uint TypeIds,
    uint ProtoIds,
    uint FieldIds,
    uint MethodIds,
    uint ClassDefs,
    bool HeaderValid,
    string? Warning);

public sealed record DexFileAnalysis(
    string EntryName,
    string Module,
    long UncompressedBytes,
    long CompressedBytes,
    string Sha256,
    DexHeaderMetrics Header,
    ulong DefinedMethods,
    ulong MethodsWithCode);
