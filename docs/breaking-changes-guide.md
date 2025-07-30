# 破壞性變更管理指引

## 🎯 什麼是破壞性變更

### ❌ **破壞性變更（避免）**
```csharp
// 移除方法
// public Task<Order> GetOrderAsync(Guid id)  ← 刪除

// 改變方法簽名
public Task<Order> GetOrderAsync(string id)  // Guid → string

// 改變返回類型
public Order GetOrderAsync(Guid id)  // Task<Order> → Order

// 移除屬性
public class OrderDto {
    // public string Status { get; set; }  ← 刪除
    public DateTime CreatedAt { get; set; }
}
```

### ✅ **非破壞性變更（安全）**
```csharp
// 新增方法多載
public Task<Order> GetOrderAsync(Guid id)  // 保持原有
public Task<Order> GetOrderAsync(Guid id, bool includeDetails)  // 新增

// 新增屬性
public class OrderDto {
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string NewProperty { get; set; }  // 新增
}

// 新增方法
public Task<Order> GetOrderWithDetailsAsync(Guid id)  // 全新方法
```

## 🛠️ 處理策略

### **策略一：API 棄用 (Deprecation)**

#### 適用情境
- 需要改變 API 行為
- 有更好的 API 設計
- 準備移除舊功能

#### 實作步驟
```csharp
// 步驟一：標記舊 API 為過時
[Obsolete("此方法已過時，請使用 GetOrderDetailsAsync 替代")]
public Task<OrderDto> GetOrderAsync(Guid id)
{
    // 保持原有實作或委派給新方法
    return GetOrderDetailsAsync(id.ToString());
}

// 步驟二：提供新 API
public Task<OrderDto> GetOrderDetailsAsync(string orderId)
{
    // 新的實作
}
```

#### PublicAPI 處理
```
PublicAPI.Shipped.txt:
GetOrderAsync  ← 保持不變

PublicAPI.Unshipped.txt:
GetOrderDetailsAsync  ← 新方法加入
```

### **策略二：API 版本化**

#### 適用情境
- 需要同時支援新舊版本
- 漸進式遷移
- 向後相容性很重要

#### 實作步驟
```csharp
// 保留原版本
public Task<OrderDto> GetOrderAsync(Guid id) { ... }

// 新版本
public Task<OrderDto> GetOrderV2Async(string orderId) { ... }
public Task<OrderDto> GetOrderV3Async(OrderRequest request) { ... }
```

### **策略三：主版本升級**

#### 適用情境
- 大規模重構
- 全新的 API 設計
- 可以接受破壞性變更

#### 實作步驟
```bash
# 1. 備份當前 PublicAPI
cp PublicAPI.Shipped.txt PublicAPI.Shipped.v1.backup.txt

# 2. 執行主版本升級
pwsh scripts/api-protection/handle-breaking-change.ps1 -ProjectPath "src/Kooco.Pikachu.HttpApi" -Strategy "major-version" -Version "v2.0.0"

# 3. 重新建立 API 基線
dotnet build src/Kooco.Pikachu.HttpApi
pwsh scripts/api-protection/auto-update-public-api.ps1
```

## 📋 決策流程圖

```
需要修改 Shipped API？
      ↓
是否為破壞性變更？
      ↓              ↓
    是               否
      ↓              ↓
影響範圍大嗎？      直接修改
      ↓              ↓
  大     小         完成
      ↓     ↓
主版本升級  API棄用
      ↓     ↓
   完成    完成
```

## 🚦 最佳實踐

### ✅ **推薦做法**
1. **優先使用棄用策略**：逐步遷移，向後相容
2. **清晰的遷移指引**：提供詳細的升級文檔
3. **版本規劃**：預告何時移除過時 API
4. **測試覆蓋**：確保新舊 API 都有充分測試

### ❌ **避免做法**
1. **直接刪除 API**：沒有過渡期
2. **靜默變更行為**：相同簽名但不同行為
3. **頻繁破壞性變更**：影響開發者信任

## 📊 實際範例

### 情境：訂單查詢 API 需要支援更多參數

#### 原始 API
```csharp
// 在 PublicAPI.Shipped.txt
public Task<OrderDto> GetOrderAsync(Guid orderId)
```

#### 方案一：棄用 + 新 API
```csharp
[Obsolete("請使用 GetOrderDetailsAsync")]
public Task<OrderDto> GetOrderAsync(Guid orderId)
{
    return GetOrderDetailsAsync(orderId.ToString(), false);
}

// 新 API 會自動加入 PublicAPI.Unshipped.txt
public Task<OrderDto> GetOrderDetailsAsync(string orderId, bool includeItems = false)
```

#### 方案二：多載（非破壞性）
```csharp
// 保持原有 API
public Task<OrderDto> GetOrderAsync(Guid orderId)

// 新增多載（會加入 PublicAPI.Unshipped.txt）
public Task<OrderDto> GetOrderAsync(Guid orderId, bool includeItems)
```

## 🔧 相關工具

- `scripts/api-protection/handle-breaking-change.ps1` - 破壞性變更處理腳本
- `scripts/api-protection/release-api.ps1` - API 發布管理
- PublicApiAnalyzers - 自動變更偵測

## 📖 延伸閱讀

- [Microsoft API 設計指引](https://docs.microsoft.com/dotnet/standard/design-guidelines/)
- [語義化版本規範](https://semver.org/lang/zh-TW/)
- [API 版本控制最佳實踐](https://docs.microsoft.com/aspnet/core/web-api/advanced/versioning)