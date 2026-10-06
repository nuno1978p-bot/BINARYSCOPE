$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot

Write-Host "NONIVERUS BinaryScope - Windows build gate" -ForegroundColor Cyan
Write-Host "Root: $root"

$dotnet = Get-Command dotnet -ErrorAction Stop
$info = (& dotnet --version).Trim()
Write-Host ".NET SDK: $info"

if (-not $info.StartsWith('10.')) {
    throw ".NET 10 SDK required. Detected: $info"
}

& dotnet build "$root\NONIVERUS.BinaryScope.sln" -c Release

if ($LASTEXITCODE -ne 0) {
    throw "Release build failed with ExitCode $LASTEXITCODE."
}

Write-Host "[PASS] Release build completed." -ForegroundColor Green
