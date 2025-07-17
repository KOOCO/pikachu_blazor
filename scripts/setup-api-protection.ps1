#!/usr/bin/env pwsh

Write-Host "🎯 Pikachu API 保護機制設置腳本" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

# 檢查前置條件
Write-Host "📋 檢查前置條件..." -ForegroundColor Yellow

# 檢查是否在正確的目錄
if (!(Test-Path "src/Kooco.Pikachu.Blazor")) {
    Write-Host "❌ 錯誤：請在專案根目錄執行此腳本" -ForegroundColor Red
    exit 1
}

# 創建 API 保護腳本目錄
Write-Host "📁 創建 API 保護目錄結構..." -ForegroundColor Blue
$scriptsDir = "scripts/api-protection"
$testsDir = "test/Kooco.Pikachu.API.Tests"
$contractsDir = "api-contracts"

New-Item -ItemType Directory -Force -Path $scriptsDir | Out-Null
New-Item -ItemType Directory -Force -Path $testsDir | Out-Null  
New-Item -ItemType Directory -Force -Path $contractsDir | Out-Null

Write-Host "✅ 目錄結構創建完成" -ForegroundColor Green

# Phase 1: 啟動 API 並建立基準契約
Write-Host "🚀 Phase 1: 建立 API 契約基準..." -ForegroundColor Magenta

Write-Host "正在檢查 API 狀態..." -ForegroundColor Blue

# 嘗試連接到 API
$apiAvailable = $false
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/" -TimeoutSec 3 -ErrorAction Stop
    $apiAvailable = $true
    Write-Host "✅ API 已在運行" -ForegroundColor Green
} catch {
    Write-Host "⚠️  API 未運行，需要啟動" -ForegroundColor Yellow
    Write-Host "請在另一個終端執行：" -ForegroundColor Cyan
    Write-Host "cd src/Kooco.Pikachu.Blazor" -ForegroundColor White
    Write-Host "dotnet run" -ForegroundColor White
    Write-Host ""
    Write-Host "API 啟動後，瀏覽到 http://localhost:5000/swagger 確認正常運作" -ForegroundColor Yellow
    Write-Host "然後按 Enter 繼續..." -ForegroundColor Yellow
    Read-Host
}

# 等待 API 就緒並擷取 Swagger 文檔
Write-Host "⏳ 等待 API 就緒並擷取契約..." -ForegroundColor Blue
$maxAttempts = 10
$attempt = 0
$contractObtained = $false

do {
    try {
        $swaggerUrl = "http://localhost:5000/swagger/v1/swagger.json"
        $apiContract = Invoke-RestMethod -Uri $swaggerUrl -TimeoutSec 5 -ErrorAction Stop
        
        $baselineFile = "$contractsDir/api-contract-baseline.json"
        $apiContract | ConvertTo-Json -Depth 20 | Out-File -FilePath $baselineFile -Encoding utf8
        
        $contractObtained = $true
        Write-Host "✅ API 契約基準已成功儲存" -ForegroundColor Green
        
        # 顯示 API 統計
        $pathsCount = ($apiContract.paths | Get-Member -MemberType NoteProperty).Count
        $schemasCount = if ($apiContract.components -and $apiContract.components.schemas) { 
            ($apiContract.components.schemas | Get-Member -MemberType NoteProperty).Count 
        } else { 0 }
        
        Write-Host "📊 API 契約統計：" -ForegroundColor Cyan
        Write-Host "   - API 端點數量：$pathsCount" -ForegroundColor White  
        Write-Host "   - 資料模型數量：$schemasCount" -ForegroundColor White
        Write-Host "   - API 版本：$($apiContract.info.version)" -ForegroundColor White
        Write-Host "   - 契約檔案：$baselineFile" -ForegroundColor White
        
    } catch {
        $attempt++
        if ($attempt -le $maxAttempts) {
            Write-Host "⏳ 嘗試 $attempt/$maxAttempts - 等待 Swagger 可用..." -ForegroundColor Yellow
            Start-Sleep -Seconds 3
        }
    }
} while (-not $contractObtained -and $attempt -le $maxAttempts)

if (-not $contractObtained) {
    Write-Host "❌ 無法擷取 API 契約" -ForegroundColor Red
    Write-Host "請確保：" -ForegroundColor Yellow
    Write-Host "1. Blazor 應用程式正在運行在 http://localhost:5000" -ForegroundColor White
    Write-Host "2. Swagger 在 http://localhost:5000/swagger 可用" -ForegroundColor White
    exit 1
}

Write-Host ""
Write-Host "🎉 Phase 1 完成！API 契約基準已建立" -ForegroundColor Green
Write-Host "📂 下一步執行：pwsh scripts/api-protection/create-integration-tests.ps1" -ForegroundColor Cyan
Write-Host "或者直接執行完整設置：pwsh scripts/api-protection/setup-complete.ps1" -ForegroundColor Magenta
