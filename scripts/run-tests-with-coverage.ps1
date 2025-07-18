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
        -reports:"./test/**/coverage/*.xml" `
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
