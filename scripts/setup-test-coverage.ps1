# Pikachu Test Coverage Setup Script
# 快速設置測試覆蓋率環境

Write-Host "=== Pikachu 測試覆蓋率環境設置 ===" -ForegroundColor Green

# 1. 安裝覆蓋率工具
Write-Host "`n[1/4] 安裝覆蓋率工具..." -ForegroundColor Yellow
$testProjects = @(
    "test/Kooco.Pikachu.Application.Tests",
    "test/Kooco.Pikachu.Domain.Tests",
    "test/Kooco.Pikachu.EntityFrameworkCore.Tests"
)

foreach ($project in $testProjects) {
    if (Test-Path $project) {
        Write-Host "  - 為 $project 安裝 coverlet..." -ForegroundColor Cyan
        Push-Location $project
        dotnet add package coverlet.msbuild --version 6.0.0
        dotnet add package coverlet.collector --version 6.0.0
        Pop-Location
    }
}

# 2. 安裝全域工具
Write-Host "`n[2/4] 安裝報告生成工具..." -ForegroundColor Yellow
dotnet tool install --global dotnet-reportgenerator-globaltool

# 3. 創建覆蓋率目錄
Write-Host "`n[3/4] 創建覆蓋率目錄結構..." -ForegroundColor Yellow
$directories = @(
    "coverage",
    "coverage/reports",
    "coverage/history"
)

foreach ($dir in $directories) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir | Out-Null
        Write-Host "  ✓ 創建 $dir" -ForegroundColor Green
    }
}

# 4. 創建測試腳本
Write-Host "`n[4/4] 創建測試執行腳本..." -ForegroundColor Yellow

$testScript = @'
# 執行所有測試並生成覆蓋率報告
param(
    [string]$Configuration = "Debug",
    [switch]$OpenReport
)

Write-Host "執行測試並收集覆蓋率資料..." -ForegroundColor Green

# 清理舊的覆蓋率資料
Remove-Item -Path "./coverage/*.xml" -Force -ErrorAction SilentlyContinue

# 執行測試
dotnet test `
    --configuration $Configuration `
    --logger "trx" `
    --results-directory "./TestResults" `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat="cobertura" `
    /p:CoverletOutput="./coverage/" `
    /p:Exclude="[*Test*]*%2c[*.EntityFrameworkCore]*%2c[*.Blazor]*" `
    /p:ExcludeByAttribute="GeneratedCodeAttribute%2cExcludeFromCodeCoverageAttribute"

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n生成覆蓋率報告..." -ForegroundColor Green
    
    # 生成HTML報告
    reportgenerator `
        -reports:"./coverage/*.xml" `
        -targetdir:"./coverage/reports" `
        -reporttypes:"Html;Cobertura;Badges" `
        -historydir:"./coverage/history"
    
    # 顯示覆蓋率摘要
    Write-Host "`n覆蓋率報告已生成!" -ForegroundColor Green
    Write-Host "報告位置: ./coverage/reports/index.html" -ForegroundColor Cyan
    
    if ($OpenReport) {
        Start-Process "./coverage/reports/index.html"
    }
} else {
    Write-Host "`n測試失敗!" -ForegroundColor Red
    exit $LASTEXITCODE
}
'@

$testScript | Out-File -FilePath "scripts/run-tests-with-coverage.ps1" -Encoding UTF8
Write-Host "  ✓ 創建 scripts/run-tests-with-coverage.ps1" -ForegroundColor Green

# 5. 創建 .gitignore 條目
Write-Host "`n更新 .gitignore..." -ForegroundColor Yellow
$gitignoreEntries = @"

# Test Coverage
coverage/
TestResults/
*.opencover.xml
*.cobertura.xml
"@

if (Test-Path .gitignore) {
    Add-Content -Path .gitignore -Value $gitignoreEntries
    Write-Host "  ✓ 更新 .gitignore" -ForegroundColor Green
}

Write-Host "`n✅ 設置完成!" -ForegroundColor Green
Write-Host "`n下一步:" -ForegroundColor Yellow
Write-Host "1. 執行測試覆蓋率: " -NoNewline
Write-Host "pwsh scripts/run-tests-with-coverage.ps1 -OpenReport" -ForegroundColor Cyan
Write-Host "2. 查看報告: " -NoNewline
Write-Host "coverage/reports/index.html" -ForegroundColor Cyan
Write-Host "3. 開始撰寫測試!" -ForegroundColor Yellow