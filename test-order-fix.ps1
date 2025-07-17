Write-Host "🔧 修復 Order 命名空間後的編譯測試..." -ForegroundColor Yellow

dotnet build test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉 編譯成功！準備開始API保護實作！" -ForegroundColor Green
    Write-Host "🚀 執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
} else {
    Write-Host "❌ 仍有編譯錯誤，檢查輸出" -ForegroundColor Red
}
