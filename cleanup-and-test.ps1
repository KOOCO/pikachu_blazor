#!/usr/bin/env pwsh

Write-Host "🧹 Cleaning solution..." -ForegroundColor Yellow
dotnet clean

Write-Host "🏗️ Building solution..." -ForegroundColor Blue
dotnet build --verbosity minimal

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build succeeded! Running tests..." -ForegroundColor Green
    dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --verbosity normal
} else {
    Write-Host "❌ Build failed. Please check the errors above." -ForegroundColor Red
}
