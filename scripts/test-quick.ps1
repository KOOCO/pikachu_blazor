# 快速測試腳本 - 只執行特定測試
param(
    [Parameter(Mandatory=$true)]
    [string]$Filter,
    
    [switch]$Watch
)

Write-Host "執行測試: $Filter" -ForegroundColor Green

if ($Watch) {
    # 監視模式 - 檔案變更時自動重新執行
    Push-Location test/Kooco.Pikachu.Application.Tests
    dotnet watch test --filter "FullyQualifiedName~$Filter"
    Pop-Location
} else {
    # 單次執行
    dotnet test test/Kooco.Pikachu.Application.Tests --filter "FullyQualifiedName~$Filter" --logger "console;verbosity=minimal"
}