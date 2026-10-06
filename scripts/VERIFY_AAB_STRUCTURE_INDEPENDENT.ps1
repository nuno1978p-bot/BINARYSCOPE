[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$AabPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path -LiteralPath $AabPath -PathType Leaf)) {
    throw "AAB not found: $AabPath"
}

Add-Type -AssemblyName System.IO.Compression

$resolved = (Resolve-Path -LiteralPath $AabPath).Path
$sha = (Get-FileHash -LiteralPath $resolved -Algorithm SHA256).Hash
$file = [System.IO.File]::Open($resolved, [System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::Read)

try {
    $zip = [System.IO.Compression.ZipArchive]::new($file, [System.IO.Compression.ZipArchiveMode]::Read, $false)
    try {
        $dex = @($zip.Entries | Where-Object {
            $n = $_.FullName.Replace('\','/')
            $parts = @($n.Split([char[]]@('/'), [System.StringSplitOptions]::RemoveEmptyEntries))
            $parts.Count -ge 3 -and
            $parts[$parts.Count - 2] -ieq 'dex' -and
            $parts[$parts.Count - 1] -match '^classes.*\.dex$'
        } | Sort-Object FullName)

        $manifests = @($zip.Entries | Where-Object {
            $n = $_.FullName.Replace('\','/')
            $parts = @($n.Split([char[]]@('/'), [System.StringSplitOptions]::RemoveEmptyEntries))
            $parts.Count -eq 3 -and
            $parts[1] -ieq 'manifest' -and
            $parts[2] -ieq 'AndroidManifest.xml'
        } | Sort-Object FullName)

        $modules = @($manifests | ForEach-Object {
            $_.FullName.Replace('\','/').Split([char[]]@('/'), [System.StringSplitOptions]::RemoveEmptyEntries)[0]
        } | Sort-Object -Unique)

        $raw = [long]0
        $compressed = [long]0

        Write-Host 'NONIVERUS BinaryScope - INDEPENDENT AAB STRUCTURE VERIFIER' -ForegroundColor Cyan
        Write-Host "AAB: $resolved"
        Write-Host "SHA-256: $sha"
        Write-Host ''
        Write-Host "Modules proven by manifest: $($modules.Count)"
        foreach ($m in $modules) { Write-Host "  $m" }
        Write-Host ''
        Write-Host "DEX files: $($dex.Count)"
        foreach ($entry in $dex) {
            $raw += [long]$entry.Length
            $compressed += [long]$entry.CompressedLength
            Write-Host ("  {0} | raw={1} | compressed={2}" -f $entry.FullName, $entry.Length, $entry.CompressedLength)
        }
        Write-Host ''
        Write-Host "DEX raw total:        $raw bytes"
        Write-Host "DEX compressed total: $compressed bytes"
        Write-Host ''
        Write-Host '[PASS] Structural measurement completed without BinaryScope.Core.' -ForegroundColor Green
    }
    finally {
        $zip.Dispose()
    }
}
finally {
    $file.Dispose()
}
