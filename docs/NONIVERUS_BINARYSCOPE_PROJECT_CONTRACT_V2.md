# NONIVERUS BinaryScope — Project Contract v2

**Status:** governance baseline after v0.1.6 FUNCTIONAL PASS
**Canonical implementation baseline:** `91ab6c4bfbc0a0bb3a0866e154b54e33d003836c`
**Platform first:** Windows
**Product:** NONIVERUS BinaryScope
**Current factual descriptor:** Android App Bundle Release Analyzer
**Future positioning candidate:** Professional Android Binary Inspection & Release Intelligence
**Inheritance:** NONIVERUS MASTERPLAN + MASTER PROMPT SYSTEM v3.

## 1. Purpose

BinaryScope is a local-first, read-only Windows analysis product for Android release artifacts.

The current certified implementation analyzes Android App Bundles (`.aab`) and provides:

- reproducible artifact identity;
- DEX inventory and deep metrics;
- local Google DEX policy preflight;
- packaged native/AOT footprint;
- packaged binary contributor evidence;
- AAB-to-AAB comparison;
- conservative comparison intelligence;
- human-readable and JSON evidence exports;
- a single-window product dashboard.

BinaryScope is evidence-first. It must distinguish what is proven from the artifact from what can only be inferred or proven by an external platform.

## 2. NONIVERUS product principles

BinaryScope must preserve:

- local-first operation;
- no ads;
- no analytics/tracking;
- no monetization by user data;
- no required cloud account for local analysis;
- no server dependency for core analysis;
- read-only artifact inspection;
- lifetime-license commercial direction;
- premium Windows UX;
- truthful evidence over aggressive claims;
- stability and zero regressions over feature count.

## 3. Current proven AAB capability baseline

At the v0.1.6 functional checkpoint, the product can prove locally:

### Artifact identity
- selected AAB path/name;
- AAB byte size;
- SHA-256;
- analysis timestamp.

### DEX
- manifest-proven modules;
- `classes*.dex` inventory;
- raw DEX bytes;
- ZIP-compressed DEX bytes;
- per-DEX identity/evidence;
- Method IDs header sum;
- unique method references;
- class definitions;
- defined methods;
- methods with code.

### Google local preflight
- NONIVERUS advisory boundary: 8,000,000 bytes;
- Google app DEX threshold: 10,000,000 bytes;
- local threshold margin;
- local classification.

The product must continue to mark official Play Console shrinking, optimization and obfuscation percentages as:

`UNPROVEN LOCALLY`

until official store evidence is supplied.

### Packaged binary evidence
- packaged native/AOT `.so` entries;
- raw/compressed native footprint;
- packaged ABIs;
- logical packaged contributors;
- contributor categories;
- category totals.

Exact NuGet/dependency reconstruction from an AAB is not claimed and remains:

`UNPROVEN FROM AAB ALONE`

### Comparison
For BEFORE → AFTER AAB comparison:

- AAB delta;
- DEX raw/compressed delta;
- DEX/method/class deltas;
- native/AOT delta;
- ABI additions/removals;
- contributor changes;
- category changes;
- PASS / REVIEW / REGRESSION intelligence.

Current conservative hard regression includes crossing the configured Google DEX threshold from at/below to above the threshold.

Internal review thresholds remain internal NONIVERUS signals, not Google requirements.

## 4. Product experience baseline

The v0.1.6 product experience includes:

- first-run onboarding;
- local preferences persistence;
- Midnight, Graphite and Light Precision themes;
- product-experience localization foundation for 10 launch languages;
- versioned What’s New;
- single-window Windows dashboard during normal use;
- internal Analyze, Compare, What’s New and Preferences views;
- maximized application behavior within the Windows work area;
- visible Windows taskbar;
- minimize supported.

The first-run onboarding wizard may exist before the main dashboard is created.

During normal operation, Compare, Preferences and What’s New must not create independent top-level BinaryScope taskbar windows.

## 5. Localization boundary

The launch localization architecture supports:

- English
- Portuguese
- Spanish
- French
- German
- Italian
- Polish
- Dutch
- Czech
- Romanian

The current product-experience layer is localized substantially beyond the original shell foundation, but deep analyzer/reporting terminology remains partly English.

Do not claim full 10-language commercial localization until all analyzer, comparison, reporting and edge-state surfaces have been translated and QA validated.

## 6. Evidence confidence model

Future analysis must converge on a common four-state confidence vocabulary:

- `CONFIRMED` — directly proven by parsed artifact evidence;
- `INFERRED` — supported by evidence but not directly provable;
- `UNKNOWN` — cannot be determined safely from available static evidence;
- `NOT APPLICABLE` — capability does not apply to the artifact/context.

This model is a Core-level engineering requirement for future intelligence. It is not merely a visual badge.

Existing truthful states such as `UNPROVEN LOCALLY`, `UNPROVEN FROM AAB ALONE` and manifest-proven evidence remain valid until deliberately migrated.

## 7. Architecture boundary

The product must evolve toward a shared artifact architecture rather than independent per-extension analyzers.

Target conceptual flow:

`Artifact Detection → Container Reader → Component Router → Evidence Engines → Confidence Model → Analysis → Comparison → Release Intelligence`

Shared component engines may include:

- Manifest engine;
- DEX engine;
- Resources/assets engine;
- Native/ELF engine;
- Signing/certificate engine;
- package/string/reference intelligence.

Format support must reuse common component engines wherever technically valid.

## 8. Format policy

### Current supported commercial analysis
- `.aab` — implemented and validated.

### Authorized for architecture/design study
- `.apk`
- `.apks`
- `.aar`
- `.xapk`
- `.apkm`
- component-level DEX/JAR/native analysis where needed internally.

### Implementation priority candidate
1. deepen AAB evidence where current gaps remain;
2. APK;
3. APKS after APK routing exists;
4. AAR only when SDK-impact analysis can provide meaningful evidence;
5. XAPK/APKM after container/component routing is mature.

This ordering is an architecture recommendation, not authorization to implement all formats.

## 9. Explicit non-goals

BinaryScope must not drift into becoming:

- a full JADX-style decompiler;
- a full MobSF-style security platform;
- a JEB-style reverse-engineering suite;
- an Android emulator;
- a Play Console replacement;
- a malware verdict engine;
- a source-code mutation/optimizer tool.

It may expose useful static evidence without pretending to prove runtime behavior.

## 10. Dependency policy

New third-party dependencies require a Dependency Audit before adoption.

The audit must establish:

- why the dependency is needed;
- whether the capability can be implemented safely without it;
- license/commercial compatibility;
- maintenance/activity;
- supply-chain risk;
- binary footprint;
- offline/local-first impact;
- update strategy;
- alternatives considered.

No dependency is approved merely because it accelerates development.

## 11. Proof Before Claim

No feature may be described as:

- PASS;
- CERTIFIED;
- SAFE;
- COMPLETE;
- RELEASE-READY;
- fully localized;
- privacy-proving;
- optimization-proving;

without matching evidence.

Static analysis findings must clearly distinguish evidence from inference.

## 12. Source-control governance

For implementation work:

- preserve pre-existing local changes;
- guard source identity before patching;
- backup the patch scope where appropriate;
- build/test before checkpoint;
- no reset/revert/stash/clean without explicit authorization;
- no commit/push/tag without explicit authorization;
- do not mix unrelated scope into a phase checkpoint.

## 13. Commercial boundary

Reference commercial direction:

- premium professional Windows product;
- lifetime license;
- current reference price: €59.99;
- free-to-try when commercially appropriate;
- no subscription requirement.

The €59.99 positioning must be justified by evidence quality, comparison intelligence, release-decision value, UX and reliability — not by counting supported file extensions.

## 14. Contract precedence

This Contract v2 supersedes `NONIVERUS_BINARYSCOPE_PROJECT_CONTRACT_V1.md` for current/future engineering decisions.

Contract v1 remains in the repository as historical evidence of the original AAB/DEX scope.

The hierarchy remains:

`MASTER → PROJECT CONTRACT → CURRENT PHASE/TASK`
