# 更新覆蓋率徽章供團隊分享
Write-Host "更新覆蓋率徽章..." -ForegroundColor Green

# 確保徽章目錄存在
$badgeDir = "./coverage"
if (!(Test-Path $badgeDir)) {
    New-Item -ItemType Directory -Path $badgeDir | Out-Null
}

# 複製最新的徽章
$badges = @(
    "badge_linecoverage.svg",
    "badge_branchcoverage.svg", 
    "badge_methodcoverage.svg",
    "badge_combined.svg"
)

foreach ($badge in $badges) {
    $source = "./coverage/reports/$badge"
    $dest = "./coverage/$badge"
    
    if (Test-Path $source) {
        Copy-Item $source $dest -Force
        Write-Host "  ✓ 更新 $badge" -ForegroundColor Green
    }
}

# 更新 README 的日期
$readmePath = "./coverage/README.md"
if (Test-Path $readmePath) {
    $content = Get-Content $readmePath -Raw
    $today = Get-Date -Format "yyyy-MM-dd"
    $content = $content -replace "最後更新：\d{4}-\d{2}-\d{2}", "最後更新：$today"
    Set-Content $readmePath $content -NoNewline
    Write-Host "  ✓ 更新 README 日期" -ForegroundColor Green
}

Write-Host "`n✅ 徽章更新完成！" -ForegroundColor Green
Write-Host "請記得 commit 這些變更：" -ForegroundColor Yellow
Write-Host "  git add coverage/*.svg coverage/README.md" -ForegroundColor Cyan
Write-Host "  git commit -m 'chore: 更新測試覆蓋率徽章'" -ForegroundColor Cyan