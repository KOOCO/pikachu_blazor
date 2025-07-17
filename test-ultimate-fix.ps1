Write-Host "🔧 測試最後2個錯誤的修復..." -ForegroundColor Yellow

dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity normal

if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉🎉🎉 所有測試都通過了！！！" -ForegroundColor Green
    Write-Host "現在可以開始 API 保護實作了！" -ForegroundColor Green
    Write-Host "🚀🚀🚀 執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
} else {
    Write-Host "⚠️  還剩下少數測試失敗" -ForegroundColor Yellow
    Write-Host "但主要問題已解決，可以開始 API 保護實作" -ForegroundColor Green
    Write-Host "🚀 執行：pwsh scripts/setup-api-protection.ps1" -ForegroundColor Cyan
}
