# PublicAPI 自動化工作流程

## 🎯 總覽

我們實作了一個智慧的 PublicAPI 管理系統，結合了本地開發便利性和 CI/CD 嚴格性。

## 🔄 工作流程

### 本地開發
- ✅ **開發者體驗**: `dotnet run` / `dotnet build` 只會顯示警告，不會阻斷開發
- ⚠️ **警告提醒**: RS0016/RS0017 警告會提醒 API 變更

### CI/CD 管道
1. **自動修復**: 嘗試自動將新 API 加入 PublicAPI.Unshipped.txt
2. **變更檢測**: 檢查是否有未提交的 PublicAPI 變更
3. **嚴格驗證**: 如果仍有 RS0016/RS0017 則 CI/CD 失敗

## 📝 開發者指南

### 當你新增 API 時

#### 方法一：本地自動修復
```bash
# 執行自動修復腳本
./scripts/update-api.sh

# 或直接執行 PowerShell
pwsh scripts/auto-update-public-api.ps1
```

#### 方法二：手動處理警告
1. 執行 `dotnet build` 查看 RS0016 警告
2. 將新 API 手動加入對應的 `PublicAPI.Unshipped.txt`
3. 再次建置確認無警告

### 提交變更
```bash
# 加入 PublicAPI 變更
git add **/*PublicAPI*.txt

# 提交
git commit -m "chore: update PublicAPI files for new APIs"
```

## 🚫 CI/CD 失敗情境

### 情境一：未提交 PublicAPI 變更
```
❌ 偵測到 PublicAPI 檔案變更:
  M src/Kooco.Pikachu.HttpApi/PublicAPI.Unshipped.txt

請執行以下步驟修復:
1. 在本地執行: pwsh scripts/auto-update-public-api.ps1
2. 檢查並提交 PublicAPI 檔案變更
3. 重新推送到分支
```

**解決方案**:
```bash
# 本地執行修復
pwsh scripts/auto-update-public-api.ps1

# 提交變更
git add **/*PublicAPI*.txt
git commit -m "chore: update PublicAPI files"
git push
```

### 情境二：仍有 API 問題
```
❌ 發現 API 問題 - CI/CD 應該失敗
  新 API: 0
  無效 API: 3
```

**解決方案**: 手動檢查並修復 PublicAPI 檔案中的無效項目

## 📁 相關檔案

- `scripts/auto-update-public-api.ps1` - 自動化腳本
- `scripts/update-api.sh` - 本地便利腳本
- `azure-pipelines.yml` - CI/CD 配置
- `common.props` - 編譯器設定

## ⚙️ 技術細節

### 環境區分
- **本地**: `CI != 'true'` → RS0016/RS0017 為警告
- **CI/CD**: `CI == 'true'` → RS0016/RS0017 為錯誤

### 自動化邏輯
1. 建置專案並擷取 RS0016 (新 API) 和 RS0017 (無效 API)
2. 將新 API 加入 `PublicAPI.Unshipped.txt`
3. 從 PublicAPI 檔案移除無效 API
4. 重新驗證確保沒有剩餘問題

## 🎁 優點

✅ **開發者友善**: 本地開發不會被阻斷
✅ **自動化修復**: CI/CD 會嘗試自動解決問題
✅ **嚴格品質**: 確保 API 變更不會被忽略
✅ **清晰回饋**: 明確告知開發者如何修復問題
✅ **版本追蹤**: 所有 API 變更都有歷史記錄