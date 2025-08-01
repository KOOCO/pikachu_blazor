#!/usr/bin/env pwsh

# Extract API signatures from Domain.Shared build errors
param(
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "src/Kooco.Pikachu.Domain.Shared",
    
    [Parameter(Mandatory=$false)]
    [string]$BuildOutput,
    
    [Parameter(Mandatory=$false)]
    [string]$OutputFile,
    
    [Parameter(Mandatory=$false)]
    [switch]$FromFile,
    
    [Parameter(Mandatory=$false)]
    [string]$InputFile
)

# Get build output from appropriate source
if ($BuildOutput) {
    # Use provided build output
    $output = $BuildOutput
} elseif ($FromFile -and $InputFile) {
    # Read from file
    if (Test-Path $InputFile) {
        $output = Get-Content $InputFile -Raw
    } else {
        Write-Error "Input file not found: $InputFile"
        exit 1
    }
} elseif ($ProjectPath) {
    # Build project and capture output
    Write-Host "🔨 Building $ProjectPath to extract API signatures..."
    $env:CI = "true"
    $output = dotnet build $ProjectPath --verbosity minimal --no-restore 2>&1 | Out-String
} else {
    Write-Error "Must provide either -BuildOutput, -FromFile with -InputFile, or -ProjectPath"
    exit 1
}

# Extract unique API signatures
$apiSignatures = @()
$output -split "`n" | ForEach-Object {
    if ($_ -match "符號\s+'([^']+)'.*並非宣告的公用 API") {
        $apiSignatures += $matches[1]
    }
}

# Get unique signatures and sort them
$uniqueApis = $apiSignatures | Sort-Object | Get-Unique

if ($uniqueApis.Count -eq 0) {
    Write-Host "✅ No missing API signatures found"
    exit 0
}

Write-Host "📊 Found $($uniqueApis.Count) missing API signatures:"
$uniqueApis | ForEach-Object { Write-Host "  $_" }

# Save to file if requested
if ($OutputFile) {
    $uniqueApis | Out-File -FilePath $OutputFile -Encoding UTF8
    Write-Host "`n✅ API signatures saved to: $OutputFile"
} else {
    # Output them in PublicAPI format
    Write-Host "`n📝 Formatted API signatures:"
    $uniqueApis | ForEach-Object { Write-Host $_ }
}

# Return count for scripting
exit $uniqueApis.Count