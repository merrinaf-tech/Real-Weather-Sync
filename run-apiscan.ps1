<#
.SYNOPSIS
    Checks whether a game update changed what the game reads back from this mod's overrides.

.DESCRIPTION
    Real Weather Sync overrides four values on ClimateSystem. Each is an OverridableProperty:
    read through op_Implicit a caller sees the mod's number, read through .value it sees the
    game's own. Which systems fall on which side is stated in the README and in the store
    listing, so a game update moving one across would make published text untrue.

    This scans Game.dll and compares the result against tools/ApiScan/baseline.txt, printing a
    diff and returning a non-zero exit code when anything moved.

    Run it after every game update, before deciding whether the mod needs a new release.

.PARAMETER GamePath
    The Cities: Skylines II folder. Defaults to CSII_GAMEPATH, then the usual Steam locations.

.PARAMETER UpdateBaseline
    Rewrite the baseline from what was found. Only after reviewing a real change - and update
    the README's "What the game reads back" section in the same commit.

.EXAMPLE
    .\run-apiscan.ps1
    .\run-apiscan.ps1 -UpdateBaseline
#>
[CmdletBinding()]
param(
    [string]$GamePath,
    [switch]$UpdateBaseline
)

$ErrorActionPreference = 'Stop'

# The .NET SDK is not on PATH on the machine this mod is developed on.
$dotnetCandidates = @(
    'dotnet',
    (Join-Path $env:USERPROFILE '.dotnet\dotnet.exe'),
    'C:\Program Files\dotnet\dotnet.exe'
)

$dotnet = $null
foreach ($candidate in $dotnetCandidates) {
    try {
        $resolved = (Get-Command $candidate -ErrorAction Stop).Source
        if ($resolved) { $dotnet = $resolved; break }
    }
    catch {
        if (Test-Path $candidate) { $dotnet = $candidate; break }
    }
}

if (-not $dotnet) {
    Write-Error "Could not find the .NET SDK. Install it, or add dotnet to PATH."
    exit 1
}

$project  = Join-Path $PSScriptRoot 'tools\ApiScan'
$baseline = Join-Path $project 'baseline.txt'

$arguments = @('run', '--project', $project, '-c', 'Release', '-v', 'quiet', '--', '--baseline', $baseline)
if ($GamePath)       { $arguments += @('--game', $GamePath) }
if ($UpdateBaseline) { $arguments += '--update-baseline' }

& $dotnet @arguments
exit $LASTEXITCODE
