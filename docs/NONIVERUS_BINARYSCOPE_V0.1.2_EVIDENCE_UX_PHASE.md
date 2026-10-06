# NONIVERUS BinaryScope v0.1.2 — Evidence UX & Policy Presentation

## Scope

Presentation-only phase. The v0.1.1 measurement engine remains unchanged.

### Allowed
- Human-readable policy state.
- First-class display of raw/compressed DEX and deep metrics.
- Manifest-proven module evidence.
- Explicit Play Console-only `UNPROVEN LOCALLY` state.
- Professional TXT evidence summary export.
- UI hierarchy/readability improvements.

### Forbidden in this phase
- DEX parser changes.
- Google threshold changes.
- New dependencies/packages.
- AAB mutation.
- Core analysis/report schema changes.
- Version/build changes without explicit authorization.
- Commit/push/tag.

## Acceptance gates

1. Only Windows presentation files + phase doc change.
2. Release build passes with zero observed warnings/errors.
3. Core DLL SHA-256 remains identical to v0.1.1 baseline:
   `B58B05A7E6ECAA7194E7809407F1B21EF5EF96D19427EFB95B49B0F6C9671A1E`
4. Smoke DLL SHA-256 remains identical to v0.1.1 baseline:
   `772B7ECF2527DE71031F0D2C2792F38842EA92CC5D0BBC362088FD03EBA2EBA9`
5. Manual UI smoke required after build using a known AAB.
6. No Play Console percentage claim from local evidence.
