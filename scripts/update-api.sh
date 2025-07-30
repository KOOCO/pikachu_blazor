#!/bin/bash
# Local script to update PublicAPI files

echo "🔄 更新 PublicAPI 檔案..."

# 執行 PowerShell 腳本
pwsh scripts/api-protection/auto-update-public-api.ps1

exit_code=$?

if [ $exit_code -eq 0 ]; then
    echo "✅ 沒有需要更新的 API"
elif [ $exit_code -eq 2 ]; then
    echo "🔄 PublicAPI 檔案已更新"
    echo "請檢查變更並提交:"
    echo "  git add **/*PublicAPI*.txt" 
    echo "  git commit -m 'chore: update PublicAPI files'"
else
    echo "❌ 發生錯誤"
    exit 1
fi