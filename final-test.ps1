Write-Host "🔧 最終編譯測試..." -ForegroundColor Yellow

dotnet build test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉 編譯成功！現在可以開始API保護實作了！" -ForegroundColor Green
    Write-Host "🚀 執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
} else {
    Write-Host "❌ 仍有編譯錯誤" -ForegroundColor Red
}
