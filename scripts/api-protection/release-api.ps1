# ================================================================================
# Release PublicAPI Management Script - 發布 API 管理工具
# ================================================================================
#
# 📋 功能說明：
# 將 PublicAPI.Unshipped.txt 中的 API 簽名移動到 PublicAPI.Shipped.txt，
# 標記這些 API 為已發布並受到破壞性變更保護
#
# 🎯 主要用途：
# 1. 正式發布：將新開發的 API 標記為已發布版本
# 2. 版本管理：建立 API 的版本基線，防止意外破壞性變更
# 3. 變更追蹤：記錄哪些 API 在什麼版本發布
#
# ⏰ 使用時機：
# 1. 【版本發布前】準備發布新版本時，將 Unshipped API 移到 Shipped
# 2. 【里程碑達成】完成主要功能開發，想要鎖定 API 穩定性
# 3. 【破壞性變更保護】確保已發布的 API 不會被意外修改
# 4. 【發布管道】CI/CD 中的自動化發布流程
#
# 🔧 運作模式：
# - 預設模式：實際執行移動操作，更新 PublicAPI 檔案
# - DryRun 模式：預覽將要執行的操作，不實際修改檔案
#
# 📝 操作內容：
# 1. 讀取所有專案的 PublicAPI.Unshipped.txt 內容
# 2. 附加到對應的 PublicAPI.Shipped.txt 檔案
# 3. 清空 PublicAPI.Unshipped.txt（僅保留 #nullable enable）
# 4. 記錄版本資訊和操作時間
#
# 📤 回傳碼：
# - 0: 成功完成操作
# - 1: 發生錯誤（如缺少版本號）
#
# 💡 範例用法：
# pwsh scripts/api-protection/release-api.ps1 -Version "v1.1.0"          # 執行發布
# pwsh scripts/api-protection/release-api.ps1 -Version "v1.1.0" -DryRun  # 預覽操作
# ================================================================================

param(
    [string]$Version = "",
    [switch]$DryRun = $false
)

if (-not $Version) {
    Write-Host "❌ 請提供版本號" -ForegroundColor Red
    Write-Host "範例: pwsh scripts/release-api.ps1 -Version 'v1.1.0'" -ForegroundColor Yellow
    exit 1
}

$projects = @(
    "src/Kooco.Pikachu.Application.Contracts",
    "src/Kooco.Pikachu.HttpApi", 
    "src/Kooco.Pikachu.Domain.Shared"
)

Write-Host "🚀 準備發布 $Version - PublicAPI 轉移" -ForegroundColor Green
Write-Host "$(if ($DryRun) { '(🔍 預覽模式)' } else { '(✍️ 實際執行)' })" -ForegroundColor Yellow
Write-Host ""

$totalMoved = 0

foreach ($project in $projects) {
    if (-not (Test-Path $project)) {
        Write-Host "⚠️  專案不存在，跳過: $project" -ForegroundColor Yellow
        continue
    }
    
    $shipped = Join-Path $project "PublicAPI.Shipped.txt"
    $unshipped = Join-Path $project "PublicAPI.Unshipped.txt"
    
    if (-not (Test-Path $shipped) -or -not (Test-Path $unshipped)) {
        Write-Host "⚠️  PublicAPI 檔案不存在，跳過: $project" -ForegroundColor Yellow
        continue
    }
    
    Write-Host "📦 處理專案: $project" -ForegroundColor Cyan
    
    # 讀取檔案內容
    $shippedContent = Get-Content $shipped -ErrorAction SilentlyContinue | Where-Object { $_ -and $_ -ne '#nullable enable' }
    $unshippedContent = Get-Content $unshipped -ErrorAction SilentlyContinue | Where-Object { $_ -and $_ -ne '#nullable enable' }
    
    if ($unshippedContent.Count -eq 0) {
        Write-Host "  ✅ 沒有新 API 需要移動" -ForegroundColor Green
        continue
    }
    
    Write-Host "  📝 發現 $($unshippedContent.Count) 個新 API:" -ForegroundColor White
    $unshippedContent | ForEach-Object { Write-Host "    + $_" -ForegroundColor Green }
    
    if (-not $DryRun) {
        # 合併內容
        $newShippedContent = @('#nullable enable')
        if ($shippedContent) {
            $newShippedContent += $shippedContent
        }
        $newShippedContent += ""
        $newShippedContent += "# Added in $Version"
        $newShippedContent += $unshippedContent
        
        # 排序並去重（保留標頭）
        $header = $newShippedContent | Where-Object { $_ -match '^#' -or $_ -eq '' }
        $apis = $newShippedContent | Where-Object { $_ -notmatch '^#' -and $_ -ne '' } | Sort-Object -Unique
        
        $finalContent = $header + $apis
        
        # 寫入 Shipped 檔案
        Set-Content $shipped $finalContent -Encoding UTF8
        
        # 清空 Unshipped 檔案
        Set-Content $unshipped '#nullable enable' -Encoding UTF8
        
        Write-Host "  ✅ 已移動 $($unshippedContent.Count) 個 API 到 Shipped" -ForegroundColor Green
        Write-Host "  🧹 已清空 Unshipped 檔案" -ForegroundColor Green
    }
    
    $totalMoved += $unshippedContent.Count
    Write-Host ""
}

Write-Host "📊 總結:" -ForegroundColor White
Write-Host "  版本: $Version" -ForegroundColor White
Write-Host "  移動的 API: $totalMoved" -ForegroundColor White

if ($DryRun) {
    Write-Host "🔍 這是預覽模式，沒有進行實際變更" -ForegroundColor Yellow
    Write-Host "執行實際移動: pwsh scripts/release-api.ps1 -Version '$Version'" -ForegroundColor Yellow
} else {
    Write-Host "✅ PublicAPI 發布完成！" -ForegroundColor Green
    Write-Host ""
    Write-Host "下一步：" -ForegroundColor Yellow
    Write-Host "  1. 檢查變更: git status" -ForegroundColor Yellow
    Write-Host "  2. 提交變更: git add **/*PublicAPI*.txt && git commit -m 'chore: release PublicAPI for $Version'" -ForegroundColor Yellow
    Write-Host "  3. 建立版本標籤: git tag $Version" -ForegroundColor Yellow
}

if ($totalMoved -eq 0) {
    Write-Host "ℹ️  沒有 API 需要移動，可能是因為：" -ForegroundColor Blue
    Write-Host "  - 這個版本沒有新增 API" -ForegroundColor Blue
    Write-Host "  - 所有 API 都已經在之前移動了" -ForegroundColor Blue
}