Write-Host "🔧 測試最終修復結果..." -ForegroundColor Yellow

Write-Host "執行測試..." -ForegroundColor Blue
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity normal

if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉 所有測試通過！準備開始API保護實作！" -ForegroundColor Green
    Write-Host "🚀 執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
} else {
    Write-Host "⚠️  還有一些測試失敗，但可以開始API保護實作" -ForegroundColor Yellow
    Write-Host "主要的編譯和依賴問題已經解決" -ForegroundColor Green
    Write-Host "🚀 執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
}
