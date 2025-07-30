# API 版本管理工作流程

## 🎯 核心概念

### PublicAPI 檔案職責
- **PublicAPI.Shipped.txt**: 已發布的穩定 API，受到破壞性變更保護
- **PublicAPI.Unshipped.txt**: 開發中的新 API，發布前可以自由修改

## 🔄 完整開發週期

### 階段一：開發新功能
```bash
# 開發者新增 API
dotnet run  # ⚠️ RS0016 警告

# 自動或手動將新 API 加入 Unshipped
./scripts/update-api.sh

# 結果：
# Shipped.txt:   [v1.0.0 穩定 API]
# Unshipped.txt: [v1.1.0 新功能 API] ← 持續累積
```

### 階段二：發布準備
```bash
# 預覽即將發布的 API
pwsh scripts/release-api.ps1 -Version "v1.1.0" -DryRun

# 輸出：
# 📦 處理專案: src/Kooco.Pikachu.HttpApi
#   📝 發現 5 個新 API:
#     + CreateNewOrderAsync
#     + UpdateInventoryAsync
#     + GetOrderStatusAsync
#     + DeletePaymentMethodAsync
#     + ProcessRefundRequestAsync
```

### 階段三：正式發布
```bash
# 執行 API 發布
pwsh scripts/release-api.ps1 -Version "v1.1.0"

# 提交變更
git add **/*PublicAPI*.txt
git commit -m "chore: release PublicAPI for v1.1.0"
git tag v1.1.0

# 結果：
# Shipped.txt:   [v1.0.0 API + v1.1.0 API] ← 合併所有已發布 API
# Unshipped.txt: [#nullable enable]        ← 清空，準備 v1.2.0
```

### 階段四：下一個開發週期
```bash
# v1.2.0 開發開始
# 新的 API 會再次累積到 Unshipped.txt
# Shipped.txt 保持不變，保護已發布的 API
```

## 📊 實際範例

### 發布前狀態
```
v1.1.0 準備發布

PublicAPI.Shipped.txt:
#nullable enable
CreateOrderAsync
GetOrderAsync
UpdateOrderAsync

PublicAPI.Unshipped.txt:
#nullable enable
DeleteOrderAsync       ← 新功能
ProcessPaymentAsync    ← 新功能
GetInventoryAsync      ← 新功能
```

### 發布後狀態
```
v1.1.0 已發布

PublicAPI.Shipped.txt:
#nullable enable
CreateOrderAsync
GetOrderAsync
UpdateOrderAsync

# Added in v1.1.0
DeleteOrderAsync       ← 移過來的
ProcessPaymentAsync    ← 移過來的
GetInventoryAsync      ← 移過來的

PublicAPI.Unshipped.txt:
#nullable enable       ← 只剩標頭
```

### v1.2.0 開發中
```
PublicAPI.Shipped.txt:
#nullable enable
CreateOrderAsync       ← 受保護，不能移除
GetOrderAsync         ← 受保護，不能移除
UpdateOrderAsync      ← 受保護，不能移除
DeleteOrderAsync      ← 受保護，不能移除
ProcessPaymentAsync   ← 受保護，不能移除
GetInventoryAsync     ← 受保護，不能移除

PublicAPI.Unshipped.txt:
#nullable enable
BulkCreateOrdersAsync  ← v1.2.0 新功能開始累積
GetAnalyticsAsync      ← v1.2.0 新功能
```

## ✅ 這樣設計的好處

1. **清晰的版本界線**: 每個版本的 API 都有明確記錄
2. **破壞性變更保護**: 已發布的 API 不能隨意移除
3. **開發彈性**: 未發布的 API 可以自由修改
4. **版本追蹤**: 可以清楚看到每個版本新增了什麼 API
5. **自動驗證**: PublicApiAnalyzers 會自動檢查一致性

## 🛠️ 相關腳本

- `scripts/release-api.ps1` - 發布時移動 API
- `scripts/update-api.sh` - 開發時更新 API
- `scripts/auto-update-public-api.ps1` - CI/CD 自動化

## 📝 最佳實踐

1. **開發期間**: 讓 Unshipped.txt 累積新 API
2. **發布前**: 使用 `-DryRun` 預覽變更
3. **發布時**: 一次性移動所有 Unshipped → Shipped
4. **發布後**: Unshipped.txt 清空，開始下一個週期