#!/usr/bin/env pwsh

# Extract missing API signatures from build errors
param(
    [Parameter(Mandatory=$true, HelpMessage="Path to the project to analyze")]
    [ValidateScript({
        if(-not (Test-Path $_)) {
            throw "Project path does not exist: $_"
        }
        if(-not (Test-Path (Join-Path $_ "*.csproj"))) {
            throw "No .csproj file found in: $_"
        }
        return $true
    })]
    [string]$ProjectPath,
    
    [Parameter(Mandatory=$false, HelpMessage="Output file path for missing API signatures")]
    [ValidateNotNullOrEmpty()]
    [string]$OutputFile = "missing_apis.txt"
)

Write-Host "🔍 Extracting missing API signatures from $ProjectPath..."

# Build with CI=true to get strict validation
$env:CI = "true"

# Check if dotnet CLI is available
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error "❌ dotnet CLI is not installed or not in PATH"
    exit 1
}

# Run build and capture output
$buildOutput = dotnet build $ProjectPath --verbosity minimal --no-restore 2>&1
$buildExitCode = $LASTEXITCODE

# Check for build errors (excluding RS0016/RS0017 which are expected)
if ($buildExitCode -ne 0) {
    $hasOnlyApiErrors = $true
    $errorCount = 0
    
    $buildOutput | ForEach-Object {
        if ($_ -match "error\s+(?!RS0016|RS0017)") {
            $hasOnlyApiErrors = $false
            $errorCount++
        }
    }
    
    if (-not $hasOnlyApiErrors) {
        Write-Error "❌ Build failed with $errorCount non-API errors. Please fix build errors first."
        Write-Host "`nError details:"
        $buildOutput | Where-Object { $_ -match "error" -and $_ -notmatch "RS0016|RS0017" } | ForEach-Object { 
            Write-Host "  $_" -ForegroundColor Red
        }
        exit 1
    }
}

# Extract RS0016 errors and parse API signatures
$missingApis = @()
$buildOutput | ForEach-Object {
    # Language-agnostic pattern: Match RS0016 error code and extract symbol between quotes
    if ($_ -match "RS0016.*'([^']+)'") {
        $apiSignature = $matches[1]
        Write-Host "  Found missing API: $apiSignature"
        $missingApis += $apiSignature
    }
}

if ($missingApis.Count -gt 0) {
    # Sort and deduplicate
    $uniqueApis = $missingApis | Sort-Object | Get-Unique
    
    Write-Host "📝 Found $($uniqueApis.Count) missing API signatures"
    
    # Try to write to file with error handling
    try {
        # Check if output directory exists, create if needed
        $outputDir = Split-Path -Path $OutputFile -Parent
        if ($outputDir -and -not (Test-Path $outputDir)) {
            Write-Host "📁 Creating output directory: $outputDir"
            New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
        }
        
        # Write API signatures to file
        $uniqueApis | Out-File -FilePath $OutputFile -Encoding UTF8 -ErrorAction Stop
        
        # Verify the file was written successfully
        if (Test-Path $OutputFile) {
            $fileSize = (Get-Item $OutputFile).Length
            Write-Host "✅ API signatures saved to $OutputFile ($fileSize bytes)"
        } else {
            throw "File was not created successfully"
        }
    }
    catch {
        Write-Error "❌ Failed to write API signatures to file: $_"
        
        # Fallback: Display APIs to console
        Write-Host "`n⚠️  Displaying API signatures to console instead:"
        Write-Host "----------------------------------------"
        $uniqueApis | ForEach-Object { Write-Host $_ }
        Write-Host "----------------------------------------"
        
        # Still return count but with error exit code
        return -1
    }
    
    return $uniqueApis.Count
} else {
    Write-Host "✅ No missing APIs found"
    return 0
}