# Simple PublicAPI validation script for CI/CD
# Compatible with Windows PowerShell

param(
    [string[]]$ProjectPaths = @(
        "src/Kooco.Pikachu.Application.Contracts",
        "src/Kooco.Pikachu.HttpApi",
        "src/Kooco.Pikachu.Domain.Shared"
    )
)

$hasErrors = $false

foreach ($projectPath in $ProjectPaths) {
    Write-Host "Checking project: $projectPath"
    
    # Build project
    $buildOutput = & dotnet build $projectPath --verbosity minimal --no-restore 2>&1
    $buildExitCode = $LASTEXITCODE
    
    # Check for RS0016/RS0017 errors
    $apiErrors = @()
    foreach ($line in $buildOutput) {
        if ($line -match "RS0016" -or $line -match "RS0017") {
            $apiErrors += $line
        }
    }
    
    if ($apiErrors.Count -gt 0) {
        Write-Host "Found $($apiErrors.Count) API errors in $projectPath" -ForegroundColor Red
        $hasErrors = $true
    } else {
        Write-Host "No API errors in $projectPath" -ForegroundColor Green
    }
}

if ($hasErrors) {
    Write-Host "API validation failed" -ForegroundColor Red
    exit 1
} else {
    Write-Host "API validation passed" -ForegroundColor Green
    exit 0
}