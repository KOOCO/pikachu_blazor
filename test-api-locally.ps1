# Simple Local Testing Script for Pikachu Blazor API
# Run this from the root of your project directory

Write-Host "🚀 Starting Pikachu API Local Testing..." -ForegroundColor Green

# Check if we're in the right directory
if (-not (Test-Path "Kooco.Pikachu.sln")) {
    Write-Host "❌ Please run this script from the root of your Pikachu project directory" -ForegroundColor Red
    exit 1
}

# Step 1: Build the solution
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

# Step 2: Run existing unit tests
Write-Host "🧪 Running existing unit tests..." -ForegroundColor Yellow
dotnet test test/Kooco.Pikachu.Application.Tests/Kooco.Pikachu.Application.Tests.csproj --logger "console;verbosity=normal"
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️  Unit tests failed, but continuing..." -ForegroundColor Yellow
}

# Step 3: Start Blazor app in background
Write-Host "🌐 Starting Blazor app..." -ForegroundColor Yellow
$blazorProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", "src/Kooco.Pikachu.Blazor/Kooco.Pikachu.Blazor.csproj", "--urls=http://localhost:5000" -PassThru

# Wait for the app to start
Write-Host "⏳ Waiting for app to start..." -ForegroundColor Yellow
$timeout = 60
for ($i = 0; $i -lt $timeout; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 5 -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            Write-Host "✅ App is running!" -ForegroundColor Green
            break
        }
    }
    catch {
        Start-Sleep -Seconds 2
        if ($i -eq ($timeout - 1)) {
            Write-Host "❌ App failed to start" -ForegroundColor Red
            Stop-Process -Id $blazorProcess.Id -Force -ErrorAction SilentlyContinue
            exit 1
        }
    }
}

# Step 4: Test API endpoints manually
Write-Host "🔍 Testing API endpoints..." -ForegroundColor Yellow

# Test health endpoint
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 10
    Write-Host "✅ Health endpoint working" -ForegroundColor Green
}
catch {
    Write-Host "❌ Health endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test API endpoints
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/app/item/get-items-lookup" -TimeoutSec 10
    Write-Host "✅ Items lookup endpoint working" -ForegroundColor Green
}
catch {
    Write-Host "❌ Items lookup endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/app/item/get-item-badges" -TimeoutSec 10
    Write-Host "✅ Item badges endpoint working" -ForegroundColor Green
}
catch {
    Write-Host "❌ Item badges endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 5: Get OpenAPI contract
Write-Host "📋 Retrieving OpenAPI contract..." -ForegroundColor Yellow
try {
    # Ensure directory exists
    if (-not (Test-Path "test/ApiContracts")) {
        New-Item -ItemType Directory -Path "test/ApiContracts" -Force
    }
    
    Invoke-WebRequest -Uri "http://localhost:5000/swagger/v1/swagger.json" -OutFile "test/ApiContracts/baseline-openapi.json"
    Write-Host "✅ OpenAPI contract saved to test/ApiContracts/baseline-openapi.json" -ForegroundColor Green
}
catch {
    Write-Host "❌ Failed to retrieve OpenAPI contract: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 6: Test enhanced API client (if dependencies are working)
Write-Host "🧪 Testing enhanced API client..." -ForegroundColor Yellow
try {
    $env:RemoteServices__Default__BaseUrl = "http://localhost:5000"
    dotnet run --project test/Kooco.Pikachu.HttpApi.Client.ConsoleTestApp/Kooco.Pikachu.HttpApi.Client.ConsoleTestApp.csproj
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ API client tests passed" -ForegroundColor Green
    } else {
        Write-Host "⚠️  API client tests failed - this might be due to missing dependencies" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "⚠️  API client tests failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Cleanup
Write-Host "🧹 Cleaning up..." -ForegroundColor Yellow
Stop-Process -Id $blazorProcess.Id -Force -ErrorAction SilentlyContinue

Write-Host "🎉 Testing completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📊 Results Summary:" -ForegroundColor Cyan
Write-Host "   - Unit tests: Check console output above"
Write-Host "   - API endpoints: Check individual results above"
Write-Host "   - OpenAPI contract: Saved to test/ApiContracts/baseline-openapi.json"
Write-Host ""
Write-Host "💡 Next steps:" -ForegroundColor Cyan
Write-Host "   1. Review any failed tests"
Write-Host "   2. Commit the baseline OpenAPI contract if it was created"
Write-Host "   3. Run the enhanced Azure Pipeline for full CI/CD testing"