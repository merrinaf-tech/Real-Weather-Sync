<#
.SYNOPSIS
    Runs the Real Weather Sync test suite.

.DESCRIPTION
    Covers everything in the mod that does not depend on the game: the weather mapping, the
    in-game-clock timeline, the antipode transform, the extreme-location table, and the
    Open-Meteo client (against the real API).

    It does NOT cover in-game behaviour - that is what the manual checklist in README.md is for.

.PARAMETER Offline
    Skip the tests that call the real Open-Meteo API.

.EXAMPLE
    .\run-tests.ps1
    .\run-tests.ps1 -Offline
#>
[CmdletBinding()]
param(
    [switch]$Offline
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

$project = Join-Path $PSScriptRoot 'tests\RealWeatherSync.Tests'

$arguments = @('run', '--project', $project, '-c', 'Release')
if ($Offline) { $arguments += @('--', '--offline') }

& $dotnet @arguments
exit $LASTEXITCODE
