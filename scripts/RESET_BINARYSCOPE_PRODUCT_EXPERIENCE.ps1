[CmdletBinding()]
param(
    [switch]$NoBackup
)

$ErrorActionPreference = "Stop"

$settingsDir = Join-Path $env:LOCALAPPDATA "NONIVERUS\BinaryScope"
$settingsPath = Join-Path $settingsDir "product-settings.json"

if (-not (Test-Path -LiteralPath $settingsPath -PathType Leaf)) {
    Write-Host "[INFO] No BinaryScope product settings found."
    Write-Host "Path: $settingsPath"
    exit 0
}

if (-not $NoBackup) {
    $stamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $backupPath = Join-Path $settingsDir ("product-settings.backup_" + $stamp + ".json")
    Copy-Item -LiteralPath $settingsPath -Destination $backupPath -Force
    Write-Host "[PASS] Backup: $backupPath" -ForegroundColor Green
}

Remove-Item -LiteralPath $settingsPath -Force
Write-Host "[PASS] Product experience reset." -ForegroundColor Green
Write-Host "Next launch will show first-run onboarding."
