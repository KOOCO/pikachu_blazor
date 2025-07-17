Write-Host "🧹 清理並重建以查看真正的編譯錯誤..." -ForegroundColor Yellow

Write-Host "清理專案..." -ForegroundColor Blue
dotnet clean

Write-Host "重建專案..." -ForegroundColor Blue
dotnet build test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity normal

Write-Host "編譯完成！" -ForegroundColor Green
