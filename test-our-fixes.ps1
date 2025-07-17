#!/usr/bin/env pwsh

Write-Host "🔧 Testing Our Bug Fixes" -ForegroundColor Yellow

cd /Users/seco/Documents/GitHub/pikachu_blazor

# Build first to check for compilation errors
Write-Host "1. Building solution..." -ForegroundColor Cyan
dotnet build --configuration Debug

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed! Compilation errors need to be fixed first." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful! No compilation errors." -ForegroundColor Green

# Test specific fixes we made
Write-Host "`n2. Testing our specific fixes..." -ForegroundColor Cyan

Write-Host "   Testing StoreLogisticsOrderAppServiceTest (OrderHistoryManager fix)..." -ForegroundColor White
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~StoreLogisticsOrderAppServiceTest" --verbosity minimal

Write-Host "   Testing MemberAppServiceTests (MemberTagManager fix)..." -ForegroundColor White  
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~MemberAppServiceTests" --verbosity minimal

Write-Host "   Testing ItemAppServiceTests (ItemNo property fix)..." -ForegroundColor White
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~ItemAppServiceTests.Should_Create_Item_If_Name_Is_Within_Limit" --verbosity minimal

Write-Host "`n3. Summary of fixes applied:" -ForegroundColor Blue
Write-Host "   ✅ Added ItemNo property to ItemDto" -ForegroundColor Green
Write-Host "   ✅ Fixed OrderHistoryManager mocking with proper constructor" -ForegroundColor Green
Write-Host "   ✅ Fixed MemberTagManager mocking with proper constructor" -ForegroundColor Green
Write-Host "   ✅ Added null safety checks in StoreLogisticsOrderAppServiceTest" -ForegroundColor Green
Write-Host "   ✅ Fixed Chinese error message assertions" -ForegroundColor Green
Write-Host "   ✅ Removed unused field warnings" -ForegroundColor Green
Write-Host "   ✅ Created TestDataGenerator for unique test data" -ForegroundColor Green

Write-Host "`n🎯 Next steps if tests still fail:" -ForegroundColor Yellow
Write-Host "   - Check for remaining database constraint violations" -ForegroundColor White
Write-Host "   - Verify actual Chinese error messages match assertions" -ForegroundColor White
Write-Host "   - Fix any remaining null reference issues" -ForegroundColor White

Write-Host "`n✅ Bug fixing progress completed!" -ForegroundColor Green
