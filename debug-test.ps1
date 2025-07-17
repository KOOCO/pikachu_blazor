#!/usr/bin/env pwsh

Write-Host "🔍 Debugging Test - Getting Actual Error Messages" -ForegroundColor Yellow

cd /Users/seco/Documents/GitHub/pikachu_blazor

# Run just one specific failing test to see the actual error message
Write-Host "Running single test to capture error message..." -ForegroundColor Cyan
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --filter "FullyQualifiedName~ItemAppServiceTests.Should_Not_Allow_Null_Item_Name" --verbosity detailed --logger "console;verbosity=detailed" 2>&1 | Tee-Object -FilePath test-output.txt

Write-Host "✅ Test output saved to test-output.txt" -ForegroundColor Green
Write-Host "Please check the file for actual error messages" -ForegroundColor Yellow
