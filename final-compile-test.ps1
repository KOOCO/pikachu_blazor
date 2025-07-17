Write-Host "🔧 最後編譯修復測試..." -ForegroundColor Yellow

Write-Host "清理專案..." -ForegroundColor Blue
dotnet clean

Write-Host "重建專案..." -ForegroundColor Blue  
dotnet build test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ 編譯成功！準備開始API保護實作！" -ForegroundColor Green
} else {
    Write-Host "❌ 還有編譯錯誤，繼續修復..." -ForegroundColor Red
}
