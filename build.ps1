param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

Write-Host "=== BaroCsSchema Build Script ==="
Write-Host "Configuration: $Configuration"

& "$PSScriptRoot\get-refs.ps1"
if (-not $?) {
    throw "restore-refs.ps1 failed"
}

Write-Host "Restoring packages..."
dotnet restore
if (-not $?) {
    throw "dotnet restore failed"
}

Write-Host "Building in ($Configuration)..."
dotnet build -c $Configuration
if (-not $?) {
    throw "dotnet build failed"
}

Write-Host "=== Build completed successfully ==="
