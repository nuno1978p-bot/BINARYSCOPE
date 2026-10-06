# NONIVERUS BinaryScope — Project Contract v1

**Product:** NONIVERUS BinaryScope  
**Platform first:** Windows  
**Primary purpose:** read-only preflight analysis of Android App Bundles (`.aab`) for DEX footprint and Google Play February 2027 optimization readiness.  
**Baseline:** v0.1.0 / AAB Read-Only Analyzer  
**Inheritance:** NONIVERUS MASTERPLAN + MASTER PROMPT SYSTEM v3 and NONIVERUS QR R8 / DEX LAB STUDY (2026-10-03).

## Product boundary
BinaryScope is not a Play Console replacement and does not claim the official Google shrinking/optimization/obfuscation percentages from local inference.

It may prove locally:
- AAB byte size and SHA-256;
- modules visible in the bundle;
- `classes*.dex` inventory per module;
- uncompressed and ZIP-compressed DEX bytes;
- DEX SHA-256;
- DEX header counts (strings/types/protos/fields/method IDs/class defs);
- BUNDLE-METADATA presence;
- ProGuard mapping evidence when present;
- native `.so` inventory;
- local threshold classification.

It must keep as UNPROVEN until store evidence exists:
- Google Play shrinking percentage;
- Google Play optimization percentage;
- Google Play obfuscation percentage;
- final Play Console compliance.

## Google policy encoded in v0.1
For apps, the February 2027 optimization requirement applies when DEX code is **> 10 MB**. When applicable, Google requires at least **25%** for each of shrinking, optimization and obfuscation.

BinaryScope uses 10,000,000 bytes as the local decimal-MB policy boundary. A separate 8,000,000-byte warning boundary is a **NONIVERUS advisory only**, not a Google requirement.

## Non-negotiable rules
1. Read-only analysis of the selected AAB.
2. No mutation of source projects or bundles.
3. No upload/network requirement for local analysis.
4. No analytics, ads or tracking.
5. No PASS/CERTIFIED claim without corresponding evidence.
6. A local DEX measurement is not represented as the Play Console measurement.
7. Zero external NuGet dependencies in v0.1 unless a later Dependency Audit authorizes one.
8. Stability and truthful evidence take priority over aggressive optimization.

## Architecture
- `NONIVERUS.BinaryScope.Core` — AAB/DEX/policy/reporting engine, UI-independent.
- `NONIVERUS.BinaryScope.Windows` — WPF Windows client.
- `NONIVERUS.BinaryScope.Smoke` — dependency-free CLI for reproducible local analysis/evidence.

## v0.1 acceptance criteria
- open or drag one `.aab`;
- enumerate module DEX entries;
- calculate total local DEX bytes;
- parse DEX header counts;
- calculate AAB + DEX SHA-256;
- classify `<8 MB`, `8–10 MB`, `>10 MB` locally;
- display Google 25/25/25 requirement when applicable;
- visibly label store percentages `UNPROVEN LOCALLY`;
- export JSON evidence;
- no modification of the analyzed file.

## Next phases (not authorized by v0.1)
- v0.2 AAB-to-AAB comparison and growth attribution;
- v0.3 dependency contributor analysis;
- v0.4 R8 configuration evidence/import;
- v0.5 Play Console metric entry + final gate report;
- later: richer binary analyzers only after current AAB scope is stable.
