# ================================================================================
# Breaking API Changes Management Script - 破壞性 API 變更管理工具
# ================================================================================
#
# 📋 功能說明：
# 提供安全處理破壞性 API 變更的策略和指導，避免意外破壞客戶端程式碼，
# 同時幫助開發團隊以最佳實踐方式演進 API 設計
#
# 🎯 主要用途：
# 1. 破壞性變更指導：提供處理 Shipped API 變更的最佳實踐
# 2. 策略選擇幫助：根據情境推薦合適的變更策略
# 3. 風險評估：評估 API 變更對現有用戶的影響
# 4. 遷移規劃：協助制定 API 升級和遷移計畫
#
# ⏰ 使用時機：
# 1. 【需要修改 Shipped API】已發布的 API 需要變更行為或簽名
# 2. 【重大重構】大規模 API 設計變更，需要評估破壞性影響
# 3. 【版本規劃】計畫新版本時，需要處理累積的 API 技術債
# 4. 【向後相容性】平衡新功能需求與現有用戶體驗
# 5. 【程式碼審查】審查涉及 API 變更的拉取請求
#
# 🔧 處理策略：
# 1. 【deprecate】API 棄用策略
#    - 適用：小幅度變更，需要平滑遷移
#    - 方法：保留舊 API 標記 [Obsolete]，提供新 API
#    - 影響：最小，向後相容
#
# 2. 【version】API 版本化策略  
#    - 適用：需要同時支援新舊版本
#    - 方法：建立新版本 API (如 V2, V3)
#    - 影響：中等，需要維護多版本
#
# 3. 【major-version】主版本升級策略
#    - 適用：大規模重構，可接受破壞性變更
#    - 方法：清空 PublicAPI 檔案，重新建立基線
#    - 影響：最大，需要客戶端升級
#
# 📝 操作流程：
# 1. 分析變更影響範圍和破壞性程度
# 2. 根據策略提供具體實作指導
# 3. 生成必要的範例程式碼
# 4. 更新相關文檔和遷移指南
#
# 📤 回傳碼：
# - 0: 成功完成指導或操作
# - 1: 參數錯誤或操作失敗
#
# 💡 範例用法：
# pwsh scripts/api-protection/handle-breaking-change.ps1 -ProjectPath "src/HttpApi" -Strategy "deprecate"
# pwsh scripts/api-protection/handle-breaking-change.ps1 -ProjectPath "src/HttpApi" -Strategy "major-version" -Version "v2.0.0"
# pwsh scripts/api-protection/handle-breaking-change.ps1 -ProjectPath "src/HttpApi" -Strategy "version" -DryRun
# ================================================================================

param(
    [Parameter(Mandatory)]
    [string]$ProjectPath,
    
    [Parameter(Mandatory)]
    [ValidateSet("deprecate", "version", "major-version")]
    [string]$Strategy,
    
    [string]$ApiToRemove = "",
    [string]$ApiToAdd = "",
    [string]$Version = "",
    [switch]$DryRun = $false
)

$shipped = Join-Path $ProjectPath "PublicAPI.Shipped.txt"
$unshipped = Join-Path $ProjectPath "PublicAPI.Unshipped.txt"

if (-not (Test-Path $shipped) -or -not (Test-Path $unshipped)) {
    Write-Host "❌ PublicAPI 檔案不存在: $ProjectPath" -ForegroundColor Red
    exit 1
}

Write-Host "🔧 處理破壞性 API 變更" -ForegroundColor Yellow
Write-Host "專案: $ProjectPath" -ForegroundColor White
Write-Host "策略: $Strategy" -ForegroundColor White
Write-Host "$(if ($DryRun) { '(🔍 預覽模式)' } else { '(✍️ 實際執行)' })" -ForegroundColor Yellow
Write-Host ""

switch ($Strategy) {
    "deprecate" {
        Write-Host "📋 策略：標記 API 為過時" -ForegroundColor Cyan
        Write-Host "1. 在程式碼中加入 [Obsolete] 屬性" -ForegroundColor White
        Write-Host "2. 新 API 會自動加入 Unshipped.txt" -ForegroundColor White
        Write-Host "3. 舊 API 保留在 Shipped.txt" -ForegroundColor White
        Write-Host ""
        Write-Host "範例程式碼：" -ForegroundColor Yellow
        Write-Host @"
[Obsolete("請使用 NewMethodAsync 替代")]
public Task<Result> OldMethodAsync(int id) { ... }

public Task<Result> NewMethodAsync(string id) { ... }  // 會加入 Unshipped
"@ -ForegroundColor Gray
    }
    
    "version" {
        Write-Host "📋 策略：API 版本化" -ForegroundColor Cyan
        Write-Host "1. 保留原有 API (在 Shipped.txt)" -ForegroundColor White
        Write-Host "2. 建立新版本 API (加入 Unshipped.txt)" -ForegroundColor White
        Write-Host ""
        Write-Host "範例程式碼：" -ForegroundColor Yellow
        Write-Host @"
// 保留舊版本
public Task<Result> GetDataAsync(int id) { ... }

// 新版本
public Task<Result> GetDataV2Async(string id) { ... }  // 會加入 Unshipped
"@ -ForegroundColor Gray
    }
    
    "major-version" {
        Write-Host "📋 策略：主版本升級 (破壞性變更)" -ForegroundColor Red
        Write-Host "⚠️  這將清空 PublicAPI 檔案並重新開始！" -ForegroundColor Red
        Write-Host ""
        
        if (-not $Version) {
            Write-Host "❌ 主版本升級需要提供版本號 (-Version)" -ForegroundColor Red
            exit 1
        }
        
        $confirmation = Read-Host "確定要執行主版本升級嗎？這是破壞性操作！(yes/no)"
        if ($confirmation -ne "yes") {
            Write-Host "❌ 操作已取消" -ForegroundColor Yellow
            exit 0
        }
        
        if (-not $DryRun) {
            # 備份現有檔案
            $backupDir = "backup-$Version-$(Get-Date -Format 'yyyyMMdd-HHmmss')"
            New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
            Copy-Item $shipped "$backupDir/"
            Copy-Item $unshipped "$backupDir/"
            
            Write-Host "📦 已備份到: $backupDir" -ForegroundColor Green
            
            # 清空檔案
            Set-Content $shipped '#nullable enable' -Encoding UTF8
            Set-Content $unshipped '#nullable enable' -Encoding UTF8
            
            Write-Host "🧹 已清空 PublicAPI 檔案" -ForegroundColor Green
            Write-Host ""
            Write-Host "下一步：" -ForegroundColor Yellow
            Write-Host "1. 建置專案: dotnet build $ProjectPath" -ForegroundColor Yellow
            Write-Host "2. 執行: pwsh scripts/api-protection/auto-update-public-api.ps1" -ForegroundColor Yellow
            Write-Host "3. 所有當前 API 會重新加入 Unshipped.txt" -ForegroundColor Yellow
        } else {
            Write-Host "🔍 預覽：將會清空兩個 PublicAPI 檔案" -ForegroundColor Yellow
        }
    }
}

if ($Strategy -ne "major-version") {
    Write-Host ""
    Write-Host "🔄 建議的工作流程：" -ForegroundColor Green
    Write-Host "1. 修改程式碼（加入新 API，標記舊 API）" -ForegroundColor White
    Write-Host "2. 建置: dotnet build $ProjectPath" -ForegroundColor White
    Write-Host "3. 更新 PublicAPI: pwsh scripts/api-protection/auto-update-public-api.ps1" -ForegroundColor White
    Write-Host "4. 提交變更" -ForegroundColor White
}

Write-Host ""
Write-Host "📚 更多資訊：" -ForegroundColor Blue
Write-Host "- API 版本管理: docs/api-versioning-workflow.md" -ForegroundColor Blue
Write-Host "- 破壞性變更指引: https://docs.microsoft.com/dotnet/standard/library-guidance/breaking-changes" -ForegroundColor Blue