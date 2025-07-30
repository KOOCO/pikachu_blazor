#!/usr/bin/env pwsh

# Extract missing API signatures from build errors
param(
    [string]$ProjectPath,
    [string]$OutputFile = "missing_apis.txt"
)

Write-Host "🔍 正在提取缺失的 API 簽名從 $ProjectPath..."

# Build with CI=true to get strict validation
$env:CI = "true"
$buildOutput = dotnet build $ProjectPath --verbosity minimal --no-restore 2>&1

# Extract RS0016 errors and parse API signatures
$missingApis = @()
$buildOutput | ForEach-Object {
    if ($_ -match "RS0016.*符號\s+'([^']+)'.*並非宣告的公用 API") {
        $apiSignature = $matches[1]
        Write-Host "  找到缺失 API: $apiSignature"
        $missingApis += $apiSignature
    }
}

if ($missingApis.Count -gt 0) {
    # Sort and deduplicate
    $uniqueApis = $missingApis | Sort-Object | Get-Unique
    
    Write-Host "📝 找到 $($uniqueApis.Count) 個缺失的 API 簽名"
    $uniqueApis | Out-File -FilePath $OutputFile -Encoding UTF8
    
    Write-Host "✅ API 簽名已儲存到 $OutputFile"
    return $uniqueApis.Count
} else {
    Write-Host "✅ 未發現缺失的 API"
    return 0
}