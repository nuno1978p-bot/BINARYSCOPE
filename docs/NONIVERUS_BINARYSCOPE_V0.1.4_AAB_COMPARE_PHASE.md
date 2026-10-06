# NONIVERUS BinaryScope v0.1.4 — AAB Compare

## Scope
Read-only comparison of BEFORE vs AFTER Android App Bundles.

## Rules
- AFTER minus BEFORE semantics.
- No AAB mutation.
- No dependency removal or optimization.
- No new NuGet packages.
- Existing single-AAB DEX and binary-footprint metrics must remain unchanged.
- Contributor comparison uses packaged binary evidence only.
- Exact NuGet dependency graph remains UNPROVEN FROM AAB ALONE.

## Outputs
- scalar metric deltas;
- ABI added/removed;
- binary category deltas;
- contributor ADDED / REMOVED / CHANGED;
- comparison JSON;
- comparison TXT.

## Validation pair
QR 1.1.0 Build 20 -> QR 1.1.1 Build 21.

Expected key deltas:
- AAB bytes: -12,355,445
- DEX raw: -384
- DEX compressed: -316
- Native/AOT .so entries: -119
- Native/AOT raw: -25,392,776
- Native/AOT compressed: -12,319,175
- ABI removed: x86_64

Google Play shrinking / optimization / obfuscation percentages remain UNPROVEN LOCALLY.
