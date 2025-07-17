Write-Host "🔧 測試所有修復結果..." -ForegroundColor Yellow

Write-Host "建置專案..." -ForegroundColor Blue
dotnet build test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ 編譯成功！" -ForegroundColor Green
    Write-Host "執行測試..." -ForegroundColor Blue
    dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity normal
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "🎉 所有測試通過！現在可以開始API保護實作了！" -ForegroundColor Green
        Write-Host "執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
    } else {
        Write-Host "⚠️  還有一些測試失敗，但編譯成功，可以開始API保護實作" -ForegroundColor Yellow
    }
} else {
    Write-Host "❌ 仍有編譯錯誤需要修復" -ForegroundColor Red
}
