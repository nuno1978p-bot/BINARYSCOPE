# NONIVERUS BinaryScope v0.1.5 — Comparison Intelligence & Regression Gates

## Scope
Add deterministic, read-only interpretation on top of the verified v0.1.4 AAB comparison.

## Verdicts
- PASS: no review/regression finding.
- REVIEW: significant change that requires human confirmation.
- REGRESSION: the AFTER AAB newly crosses the configured >10,000,000-byte DEX applicability boundary.

## Internal review defaults
These are NONIVERUS internal review signals, not Google requirements:
- DEX raw growth >= 5%
- AAB size growth >= 10%
- Native/AOT raw growth >= 10%
- DEX file count increase
- ABI added or removed
- logical binary contributor added or removed
- crossing the NONIVERUS 8,000,000-byte advisory boundary

## Improvement signals
Size decreases and recovery below configured DEX boundaries are shown as improvements but never override REVIEW/REGRESSION findings.

## Safety
- No dependency removal.
- No AAB mutation.
- No new NuGet packages.
- No Google threshold change.
- Dependency graph remains UNPROVEN FROM AAB ALONE.
- Human review remains authoritative for intentional architecture/capability changes.
