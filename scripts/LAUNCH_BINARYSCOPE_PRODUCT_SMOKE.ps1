[CmdletBinding()]
param(
    [ValidateSet("Normal", "Onboarding", "WhatsNew")]
    [string]$Mode = "Normal",
    [string]$ProjectRoot = "C:\NONIVERUS\NONIVERUS_BINARYSCOPE"
)

$ErrorActionPreference = "Stop"

$exe = Join-Path $ProjectRoot "src\NONIVERUS.BinaryScope.Windows\bin\Release\net10.0-windows\NONIVERUS.BinaryScope.Windows.exe"

if (-not (Test-Path -LiteralPath $exe -PathType Leaf)) {
    throw "BinaryScope Release executable not found: $exe"
}

$oldOnboarding = $env:NONIVERUS_BINARYSCOPE_FORCE_ONBOARDING
$oldWhatsNew = $env:NONIVERUS_BINARYSCOPE_FORCE_WHATSNEW

try {
    Remove-Item Env:\NONIVERUS_BINARYSCOPE_FORCE_ONBOARDING -ErrorAction SilentlyContinue
    Remove-Item Env:\NONIVERUS_BINARYSCOPE_FORCE_WHATSNEW -ErrorAction SilentlyContinue

    if ($Mode -eq "Onboarding") {
        $env:NONIVERUS_BINARYSCOPE_FORCE_ONBOARDING = "1"
    }
    elseif ($Mode -eq "WhatsNew") {
        $env:NONIVERUS_BINARYSCOPE_FORCE_WHATSNEW = "1"
    }

    Start-Process -FilePath $exe
    Write-Host "[PASS] Launched BinaryScope product smoke mode: $Mode" -ForegroundColor Green
}
finally {
    if ($null -eq $oldOnboarding) {
        Remove-Item Env:\NONIVERUS_BINARYSCOPE_FORCE_ONBOARDING -ErrorAction SilentlyContinue
    } else {
        $env:NONIVERUS_BINARYSCOPE_FORCE_ONBOARDING = $oldOnboarding
    }

    if ($null -eq $oldWhatsNew) {
        Remove-Item Env:\NONIVERUS_BINARYSCOPE_FORCE_WHATSNEW -ErrorAction SilentlyContinue
    } else {
        $env:NONIVERUS_BINARYSCOPE_FORCE_WHATSNEW = $oldWhatsNew
    }
}
