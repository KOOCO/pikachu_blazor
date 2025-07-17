#!/usr/bin/env pwsh

Write-Host "🔧 Running Tests with Fixed Dependencies" -ForegroundColor Yellow

cd /Users/seco/Documents/GitHub/pikachu_blazor

# Build first to ensure all dependencies are in place
Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build --configuration Debug --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed! Please fix build errors first." -ForegroundColor Red
    exit 1
}

# Run a few specific tests to see if our dependency fixes worked
Write-Host "Testing fixed StoreLogisticsOrderAppServiceTest..." -ForegroundColor Cyan
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~StoreLogisticsOrderAppServiceTest" --verbosity normal

Write-Host "Testing fixed MemberAppServiceTests..." -ForegroundColor Cyan  
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~MemberAppServiceTests" --verbosity normal

Write-Host "Testing a few ItemAppServiceTests..." -ForegroundColor Cyan
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~ItemAppServiceTests.Should_Create_Item_If_Name_Is_Within_Limit" --verbosity normal

Write-Host "✅ Tests completed! Check output above for results." -ForegroundColor Green
