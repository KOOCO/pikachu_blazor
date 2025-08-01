# ================================================================================
# Auto-update PublicAPI files for Microsoft.CodeAnalysis.PublicApiAnalyzers
# ================================================================================
#
# 📋 功能說明：
# 自動處理 PublicAPI.Shipped.txt 和 PublicAPI.Unshipped.txt 檔案，確保所有公開 API 
# 簽名都正確記錄，避免 RS0016/RS0017 編譯警告
#
# 🎯 主要用途：
# 1. 開發期間：自動將新增的公開 API 加入 PublicAPI.Unshipped.txt
# 2. CI/CD 驗證：檢查是否有未記錄的 API 變更
# 3. 程式碼審查：確保 API 變更都有適當記錄
#
# ⏰ 使用時機：
# 1. 【開發時】新增了公開 API 後執行，自動更新 Unshipped 檔案
# 2. 【提交前】確保沒有未記錄的 API 變更
# 3. 【CI/CD】管道中自動驗證 API 一致性
# 4. 【程式碼審查】驗證 PublicAPI 檔案是否正確更新
#
# 🔧 運作模式：
# - 預設模式：自動修復 RS0016 警告，將新 API 加入 Unshipped.txt
# - CheckOnly 模式：僅檢查，如有問題則回傳錯誤碼（用於 CI/CD）
#
# 📤 回傳碼：
# - 0: 沒有問題或成功修復
# - 1: 發生錯誤
# - 2: 有檔案被更新（僅在非 CheckOnly 模式）
#
# 💡 範例用法：
# pwsh scripts/api-protection/auto-update-public-api.ps1          # 自動修復
# pwsh scripts/api-protection/auto-update-public-api.ps1 -CheckOnly  # 僅檢查
# pwsh scripts/api-protection/auto-update-public-api.ps1 -ProjectPath "src/Project" # 指定專案
# ================================================================================

param(
    [string]$ProjectPath,
    [switch]$CheckOnly = $false
)

function Update-PublicApiForProject {
    param($ProjectPath)
    
    Write-Host "🔍 檢查專案: $ProjectPath"
    
    $publicApiUnshipped = Join-Path $ProjectPath "PublicAPI.Unshipped.txt"
    $publicApiShipped = Join-Path $ProjectPath "PublicAPI.Shipped.txt"
    
    # 確保檔案存在
    if (-not (Test-Path $publicApiUnshipped)) {
        Write-Host "⚠️  PublicAPI.Unshipped.txt 不存在，跳過: $ProjectPath"
        return $false
    }
    
    # 建置並捕獲 RS0016 錯誤
    Write-Host "🔨 建置專案以檢查 API 變更..."
    $buildOutput = & dotnet build $ProjectPath --verbosity normal 2>&1
    $buildExitCode = $LASTEXITCODE
    
    # 檢查建置是否成功 (只有 RS0016/RS0017 錯誤是允許的)
    if ($buildExitCode -ne 0) {
        # 檢查是否只有 API 相關的錯誤
        $hasOnlyApiErrors = $true
        $buildOutput | ForEach-Object {
            if ($_ -match "error\s+(?!RS0016|RS0017)") {
                $hasOnlyApiErrors = $false
            }
        }
        
        if (-not $hasOnlyApiErrors) {
            Write-Error "❌ 建置失敗，存在非 API 相關的錯誤。請先修復建置錯誤。"
            Write-Host "建置輸出:"
            $buildOutput | Where-Object { $_ -match "error" } | ForEach-Object { Write-Host $_ }
            exit 1
        }
    }
    
    # 提取 RS0016 新 API (language-agnostic pattern)
    $newApis = $buildOutput | Where-Object { $_ -match "RS0016.*'([^']+)'" } | ForEach-Object {
        if ($_ -match "RS0016.*'([^']+)'") {
            $matches[1]
        }
    } | Sort-Object -Unique
    
    # 提取 RS0017 無效 API (language-agnostic pattern)
    $invalidApis = $buildOutput | Where-Object { $_ -match "RS0017.*'([^']+)'" } | ForEach-Object {
        if ($_ -match "RS0017.*'([^']+)'") {
            $matches[1]
        }
    } | Sort-Object -Unique
    
    $hasChanges = $false
    
    # 處理新 API (RS0016)
    if ($newApis.Count -gt 0) {
        Write-Host "📝 發現 $($newApis.Count) 個新 API:"
        $newApis | ForEach-Object { Write-Host "  + $_" }
        
        if (-not $CheckOnly) {
            # 將新 API 加入到 Unshipped
            $currentContent = Get-Content $publicApiUnshipped -ErrorAction SilentlyContinue
            $updatedContent = $currentContent + $newApis | Sort-Object -Unique
            Set-Content $publicApiUnshipped $updatedContent -Encoding UTF8
            Write-Host "✅ 已將新 API 加入到 PublicAPI.Unshipped.txt"
            $hasChanges = $true
        }
    }
    
    # 處理無效 API (RS0017)
    if ($invalidApis.Count -gt 0) {
        Write-Host "🗑️  發現 $($invalidApis.Count) 個無效 API:"
        $invalidApis | ForEach-Object { Write-Host "  - $_" }
        
        if (-not $CheckOnly) {
            # 從兩個檔案中移除無效 API
            @($publicApiUnshipped, $publicApiShipped) | ForEach-Object {
                if (Test-Path $_) {
                    $content = Get-Content $_ | Where-Object { $_ -notin $invalidApis }
                    Set-Content $_ $content -Encoding UTF8
                }
            }
            Write-Host "✅ 已從 PublicAPI 檔案中移除無效 API"
            $hasChanges = $true
        }
    }
    
    return @{
        HasNewApis = $newApis.Count -gt 0
        HasInvalidApis = $invalidApis.Count -gt 0
        HasChanges = $hasChanges
        NewApis = $newApis
        InvalidApis = $invalidApis
    }
}

# 主邏輯
$projects = @(
    "src/Kooco.Pikachu.Application.Contracts",
    "src/Kooco.Pikachu.HttpApi", 
    "src/Kooco.Pikachu.Domain.Shared"
)

$totalNewApis = 0
$totalInvalidApis = 0
$allResults = @()

foreach ($project in $projects) {
    if (Test-Path $project) {
        $result = Update-PublicApiForProject $project
        $allResults += $result
        $totalNewApis += $result.NewApis.Count
        $totalInvalidApis += $result.InvalidApis.Count
    }
}

Write-Host ""
Write-Host "📊 總結:"
Write-Host "  新 API: $totalNewApis"
Write-Host "  無效 API: $totalInvalidApis"

if ($CheckOnly) {
    # 檢查模式：如果有任何 API 問題就失敗
    if ($totalNewApis -gt 0 -or $totalInvalidApis -gt 0) {
        Write-Host "❌ 發現 API 問題 - CI/CD 應該失敗" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "✅ 沒有 API 問題" -ForegroundColor Green
        exit 0
    }
} else {
    # 修復模式：報告是否有變更
    $hasAnyChanges = $allResults | Where-Object { $_.HasChanges } | Measure-Object | Select-Object -ExpandProperty Count
    if ($hasAnyChanges -gt 0) {
        Write-Host "🔄 PublicAPI 檔案已更新，需要提交變更" -ForegroundColor Yellow
        exit 2  # 特殊退出碼表示有變更
    } else {
        Write-Host "✅ 沒有需要更新的 API" -ForegroundColor Green
        exit 0
    }
}