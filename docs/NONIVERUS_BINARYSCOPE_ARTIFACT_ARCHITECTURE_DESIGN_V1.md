# NONIVERUS BinaryScope — Artifact Architecture Design v1

**Status:** design baseline only — no new artifact format implementation authorized
**Depends on:** Project Contract v2
**Canonical implementation baseline:** `91ab6c4bfbc0a0bb3a0866e154b54e33d003836c`

## 1. Objective

Define an extensible architecture that allows BinaryScope to grow from a proven AAB analyzer into a professional Android binary inspection and release-intelligence product without creating independent duplicated analyzers for each file extension.

This document authorizes architecture design and interface planning only.

It does **not** authorize APK/APKS/AAR/XAPK/APKM production implementation.

## 2. Architectural principle

Different Android artifacts are containers around overlapping evidence types.

Therefore the architecture should separate:

1. artifact/container detection;
2. container enumeration;
3. component identification;
4. component-specific parsing;
5. evidence production;
6. confidence classification;
7. artifact-level synthesis;
8. comparison;
9. release intelligence.

Target flow:

```text
Artifact input
    ↓
Artifact Detector
    ↓
Container Reader
    ↓
Component Router
    ↓
┌──────────┬──────────┬────────────┬──────────┬────────────┐
│ Manifest │   DEX    │ Resources  │ Native   │ Signing    │
│ Engine   │ Engine   │ Engine     │ / ELF    │ Engine     │
└──────────┴──────────┴────────────┴──────────┴────────────┘
    ↓
Evidence Model
    ↓
Confidence Model
    ↓
Artifact Analysis
    ↓
Artifact Comparison
    ↓
Release Intelligence
```

## 3. Proposed core abstractions

Names below are design candidates and may be refined before implementation.

### ArtifactKind

Candidate values:

- `Aab`
- `Apk`
- `Apks`
- `Aar`
- `Xapk`
- `Apkm`
- `Dex`
- `Jar`
- `NativeBinary`
- `Unknown`

The commercial UI does not need to expose every internal component kind as a first-class product format.

### ArtifactDescriptor

Candidate responsibilities:

- source path;
- filename;
- detected kind;
- total bytes;
- SHA-256;
- container signature/type evidence;
- analysis timestamp.

Extension alone must not be treated as sufficient proof when container signature evidence is available.

### IArtifactReader

Candidate responsibility:

- open artifact read-only;
- enumerate logical entries/components;
- expose raw/compressed sizes;
- preserve source identity;
- never mutate artifact.

A concrete reader may use ZIP/container semantics where applicable.

### ArtifactComponent

Candidate component kinds:

- Manifest;
- DEX;
- Resources table;
- generic resource/asset;
- native binary;
- JAR/classes;
- signing/certificate;
- metadata;
- mapping/profile evidence;
- nested APK/container;
- unknown entry.

### EvidenceItem

Candidate fields:

- code/stable identifier;
- title;
- value;
- source component;
- source path/entry;
- evidence detail;
- confidence state;
- severity/relevance where applicable.

The evidence model should be serializable without UI dependencies.

## 4. Confidence model

Core enum candidate:

```text
CONFIRMED
INFERRED
UNKNOWN
NOT_APPLICABLE
```

Rules:

### CONFIRMED
Use only when direct parsed evidence supports the statement.

Examples:
- permission exists in parsed manifest;
- ABI exists in packaged native path/ELF metadata;
- DEX method count parsed from DEX structure;
- certificate fingerprint parsed from signing evidence.

### INFERRED
Use when multiple evidence signals support a likely interpretation but static evidence cannot prove the complete behavior.

Examples:
- likely analytics SDK presence based on packaged classes/packages;
- likely network endpoint based on strings/references;
- likely library family based on contributor/package evidence.

### UNKNOWN
Use when BinaryScope cannot safely determine the answer.

Examples:
- actual runtime network transmission;
- whether code path is reachable in production;
- Play Console-calculated optimization percentage without store evidence.

### NOT_APPLICABLE
Use when the question does not apply to the artifact or platform context.

The UI must never visually collapse `INFERRED` into `CONFIRMED`.

## 5. Shared evidence engines

### Manifest Engine

Target future evidence:

- package/application identity when available;
- min/target SDK;
- permissions;
- activities;
- services;
- receivers;
- providers;
- exported state;
- features;
- intent-filter evidence;
- debuggable/backup/security-relevant manifest flags where safely parsed.

The engine must report parser confidence and source provenance.

### DEX Engine

Preserve current proven metrics and extend safely toward:

- package/class namespace inventory;
- package-size contribution;
- method/reference concentration;
- string/reference inventory where useful;
- SDK/library signature evidence;
- duplicate/redundancy signals where objectively measurable.

No source-code decompilation is required for the core product direction.

### Resources / Assets Engine

Candidate evidence:

- resource type inventory;
- resources/assets raw/compressed footprint;
- largest packaged resources;
- density/language/configuration footprint;
- duplicate or suspiciously redundant packaged assets only when proof is objective.

### Native / ELF Engine

Current path/size/ABI evidence is already useful.

Future ELF depth may include:

- architecture confirmation;
- dynamic dependencies;
- sections;
- symbol/stripped evidence where technically safe;
- per-library native contribution.

ELF depth must be justified by release-decision value before implementation.

### Signing / Certificate Engine

Candidate evidence:

- signing scheme evidence where available;
- certificate subject/issuer;
- fingerprints;
- validity period;
- signer count;
- comparison of signer identity between artifacts.

No trust/safety verdict should be inferred solely from a valid signature.

## 6. Container routing by format

### AAB

Current proven path remains the reference implementation.

Expected component routing:

- module manifests;
- module DEX;
- module resources/assets;
- module native binaries;
- BUNDLE-METADATA;
- mapping/profile/signing-related evidence where actually present.

### APK

Candidate routing:

- AndroidManifest;
- classes*.dex;
- resources.arsc/resources;
- assets;
- lib/<abi>/*.so;
- META-INF/signing evidence.

APK should be the first major additional container candidate after architecture interfaces are proven.

### APKS

Treat as an APK-set container, not as a completely new binary model.

Candidate flow:

`APKS → enumerate contained APKs → classify split roles → route each APK through APK reader/engines → synthesize set-level evidence`

Do not duplicate APK parsing logic inside the APKS path.

### AAR

Treat as an Android library artifact.

Candidate evidence:

- manifest contribution;
- classes.jar;
- resources;
- assets;
- native libraries;
- consumer/proguard metadata where present.

The commercial value target is **SDK Impact Analysis**, not merely “AAR opens successfully”.

### XAPK / APKM

Future container adapters may enumerate contained APKs and metadata, then reuse APK routing.

These formats should remain lower priority until shared routing is proven.

## 7. Artifact comparison architecture

Comparison should evolve from `AabComparison` toward an artifact-neutral comparison model only when doing so does not regress current AAB accuracy.

Candidate layers:

- artifact identity delta;
- component presence delta;
- DEX delta;
- manifest delta;
- resource delta;
- native delta;
- signing delta;
- confidence-aware findings;
- improvement signals;
- regression/review signals.

AAB-to-AAB comparison must remain regression-tested throughout any abstraction work.

## 8. Release Intelligence boundary

Release Intelligence should answer:

- What changed?
- How large is the change?
- Where did the change come from?
- Is platform coverage different?
- Did declared capabilities/permissions change?
- Did signing identity change?
- Did binary footprint materially regress?
- What is confirmed, inferred or unknown?
- What deserves review before release?

It should not answer:

- “this app is safe” without sufficient evidence;
- “this app sends data” from a string alone;
- “Google Play will approve this build”;
- “this dependency definitely caused X” when causality is not proven.

## 9. Migration strategy

Architecture work must avoid a big-bang rewrite.

Recommended sequence:

### Stage A — abstraction around current AAB path
Introduce artifact/evidence interfaces while preserving current AAB behavior and outputs.

Gate:
- existing QR Build20/Build21 reference metrics remain identical;
- PASS/REVIEW/REGRESSION self-test remains identical.

### Stage B — Confidence Model
Introduce common confidence semantics and map existing truthful states without weakening them.

Gate:
- no existing `UNPROVEN` boundary becomes a stronger claim.

### Stage C — deepen AAB component engines
Manifest/resources/signing/package intelligence where evidence can be proven.

### Stage D — APK adapter
Reuse shared DEX/native/resources/manifest/signing engines.

### Stage E — APKS adapter
Reuse APK path.

### Stage F — AAR impact study
Implement only if the SDK-impact output is materially useful.

## 10. Dependency strategy

Default preference: BCL/.NET implementations where practical and robust.

Before introducing any parser/library package:

- dependency audit;
- license audit;
- maintenance review;
- security/supply-chain review;
- binary-size impact;
- offline behavior;
- alternative implementation cost.

No new dependency is authorized by this design document.

## 11. UX integration

The single-window dashboard is a product invariant.

Future workspace candidates may include:

- Analyze
- Compare
- History
- Projects
- Settings

New artifact support should enter the same Analyze workflow via artifact detection rather than spawning separate products or top-level windows.

Example future user flow:

`Drop file → Detect artifact → Analyze → Understand → Decide`

## 12. Acceptance gate for architecture implementation

Before any new format implementation begins, the architecture phase must prove:

- current v0.1.6 baseline builds;
- current AAB analysis metrics remain exact;
- current AAB comparison remains exact;
- no Core regression;
- no UI regression;
- no source mutation of analyzed artifacts;
- dependency changes explicitly audited;
- evidence/confidence semantics documented;
- new abstractions do not force unsupported commercial claims.

## 13. Explicitly deferred

Not authorized by this design baseline:

- production APK implementation;
- production APKS implementation;
- production AAR implementation;
- XAPK/APKM implementation;
- malware/security verdict engine;
- decompiler;
- source optimizer;
- cloud scanning service;
- Play Console API integration.

Those require a later current-phase authorization under the Master.
