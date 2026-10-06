# NONIVERUS BinaryScope — v0.1.1 DEX Accuracy Phase

**Status:** CURRENT PHASE / ENGINE ACCURACY  
**Scope:** Windows AAB read-only analysis only  
**Master:** NONIVERUS MASTERPLAN + MASTER PROMPT SYSTEM v3

## Authorized changes

1. Correct AAB module counting by proving modules through `<module>/manifest/AndroidManifest.xml`.
2. Preserve a clearly labelled DEX-module fallback only when manifest proof is unavailable.
3. Add exact local metrics derivable from DEX structures:
   - DEX raw/compressed totals;
   - header Method ID sum;
   - unique method references across all DEX files;
   - class definitions;
   - defined methods;
   - methods with code;
   - per-DEX SHA-256 already preserved.
4. Add an independent PowerShell AAB structure verifier that does not call BinaryScope.Core.
5. Keep Google Play shrinking/optimization/obfuscation as UNPROVEN locally.

## Not authorized in this phase

- UI redesign;
- new NuGet dependencies;
- R8 configuration changes;
- AAB mutation;
- source app mutation;
- package/version/build changes;
- claims of Play Console compliance from local measurements.

## Gates

- Release build: 0 errors / 0 warnings expected.
- QR AAB structural metrics match independent verifier.
- CicloAura AAB structural metrics match independent verifier.
- CicloAura target metric cross-check where artifact identity permits:
  - class defs 17,900 expected from prior measured baseline;
  - unique method references 158,698 expected only if exact/equivalent artifact contents support it;
  - no forced PASS when artifact identity differs.
- Module count must be manifest-proven or explicitly labelled fallback/unproven.
