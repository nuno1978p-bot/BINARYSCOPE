# NONIVERUS BinaryScope — v0.1.3 Binary & Dependency Analysis

**State:** CURRENT PHASE  
**Scope:** Read-only packaged binary footprint analysis inside Android App Bundles  
**Engine rule:** DEX metrics must remain regression-identical on already validated AAB artifacts.

## Authorized

- Inventory packaged `.so` entries by module and ABI.
- Aggregate logical binary contributors across ABIs.
- Classify recognizable contributor families:
  - App code
  - .NET MAUI
  - .NET / Android runtime
  - Microsoft Extensions
  - AndroidX
  - Google libraries
  - Google Play Billing
  - Health Connect
  - Kotlin / KotlinX
  - SQLite / Data
  - Android resource glue
  - Native / Other
- Measure raw and compressed packaged bytes.
- Show ABI count and names.
- Show largest contributors and category totals.
- Export these results through existing JSON and TXT evidence.
- Explicitly state that an exact NuGet dependency graph is UNPROVEN from AAB contents alone.

## Not authorized

- removing dependencies;
- changing source apps;
- changing R8;
- changing Google thresholds;
- modifying AABs;
- adding NuGet packages;
- claiming a package is unused merely because it looks large;
- claiming causality from footprint alone.

## Regression gates

For the exact validated artifacts when present:

### CicloAura
SHA-256 `6CB8435F6B156B0904E47C8AC13CE545742712DE21BEE74162B77A0B37D46C66`

Must remain:
- DEX count 3
- DEX raw 22,180,620
- DEX compressed 7,961,837
- Method IDs 163,388
- Unique method references 158,696
- Class definitions 17,900
- Defined methods 139,923
- Methods with code 131,880

### QR 1.1.1 Build 21
SHA-256 `1912997A0F701EA5D35F6250E0A91C02E8AC128E63240C80FAE12A951DE22B75`

Must remain:
- DEX count 2
- DEX raw 9,231,820
- DEX compressed 3,813,338
- Method IDs 73,177
- Unique method references 72,404
- Class definitions 10,237
- Defined methods 66,064
- Methods with code 61,616

## Interpretation rule

Packaged contributor size is evidence of **what bytes are transported in the AAB**, not proof that a package is redundant or safely removable.

> Zero regressions > reduction of size.
