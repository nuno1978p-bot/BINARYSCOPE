# NONIVERUS BinaryScope v0.1.0 — baseline

Windows-first, local-first Android App Bundle (`.aab`) DEX preflight analyzer.

## What works in this baseline
- read-only AAB ZIP inspection;
- per-module `classes*.dex` discovery;
- local raw/compressed DEX bytes;
- SHA-256 of AAB and each DEX;
- DEX header counts for method IDs and class defs (plus other tables in JSON);
- `BUNDLE-METADATA` and mapping evidence detection;
- native `.so` inventory;
- Google February 2027 local applicability warning;
- JSON evidence export;
- WPF Windows UI + CLI smoke analyzer.

## Truth boundary
BinaryScope does **not** infer Google Play's official Shrinking / Optimization / Obfuscation percentages. Those remain UNPROVEN until Play Console evidence is supplied.

## Build on Windows
1. Install .NET 10 SDK.
2. Open `NONIVERUS.BinaryScope.sln` in Visual Studio, or run `scripts\BUILD_WINDOWS.ps1`.
3. Start `NONIVERUS.BinaryScope.Windows`.

## CLI smoke
`dotnet run --project src\NONIVERUS.BinaryScope.Smoke -- "C:\path\app.aab"`

## Environment note
This source package was generated in an environment without the .NET SDK, so compilation must be executed and certified on the target Windows development machine before any PASS claim.
