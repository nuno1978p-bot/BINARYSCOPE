# NONIVERUS BinaryScope v0.1.6 — Product Shell, Onboarding & What’s New

## Scope
Productize the verified v0.1.5 engineering core without changing its AAB/DEX/comparison logic.

## Included in v0.1.6
- NONIVERUS BinaryScope product identity and Windows application icon.
- First-run onboarding with product presentation.
- Language selection persisted locally.
- Theme selection persisted locally.
- Three themes: Midnight, Graphite and Light Precision.
- Product-experience localization foundation for 10 launch languages:
  - English
  - Português
  - Español
  - Français
  - Deutsch
  - Italiano
  - Polski
  - Nederlands
  - Čeština
  - Română
- Versioned **What’s New / Novidades** screen.
- What’s New auto-display only after a version whose `CurrentWhatsNewVersion` changes.
- Manual What’s New access from the main window and Preferences.
- Preferences stored locally under the current Windows user profile.
- Product-shell/theme resources support runtime switching; the verified v0.1.5 MainWindow XAML is preserved byte-for-byte in this phase.
- Existing maximized/taskbar/minimize behavior retained for product windows.

## What’s New behavior
- Fresh install: onboarding presents the product and marks the current What’s New version as seen, avoiding an immediate duplicate screen.
- Upgrade with relevant changes: What’s New appears once after the main window loads.
- Manual access remains available at any time.
- Versions with no meaningful product-news content can keep `CurrentWhatsNewVersion` unchanged.

## Persistence
`%LOCALAPPDATA%\NONIVERUS\BinaryScope\product-settings.json`

Stored fields:
- onboarding completed
- selected language
- selected theme
- last seen What’s New version

No server dependency is introduced.

## Localization boundary
v0.1.6 establishes and validates the product-experience localization layer for onboarding, preferences, What’s New and main-shell controls.

The deep analyzer/reporting surface remains primarily English in this phase and must be fully localized before commercial release. **Do not claim full 10-language product localization yet.**

## Safety / engineering constraints
- Core analysis engine unchanged.
- Smoke project unchanged.
- No AAB mutation.
- No dependency removal.
- No new NuGet packages.
- No Google policy-threshold change.
- Existing v0.1.5 comparison-intelligence regression evidence must remain valid.
- `UNPROVEN LOCALLY` and `UNPROVEN FROM AAB ALONE` semantics remain untouched.
- No commit/push/tag/reset/stash/clean.

## v0.1.6 V2 preservation strategy
The verified current `MainWindow.xaml` and `MainWindow.xaml.cs` are guard-only files in this phase and are **not modified**.

The product shell is attached externally at startup through `ProductShellHost`, which wraps the existing window content without changing the analyzer XAML source. This preserves the current v0.1.5 analysis UI while adding product-level access to Preferences and What's New.

Expected current `MainWindow.xaml` SHA-256:
`365819EB7764FEE7BF2D3CCC4AE835C1DC1DC0DBC28CAC4D3ED7E41B1747BB07`
