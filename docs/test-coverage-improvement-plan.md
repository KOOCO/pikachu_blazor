# Pikachu 測試覆蓋率提升計畫

## 📋 執行摘要

### 現況分析
- **當前覆蓋率**: ~5-10%
- **目標覆蓋率**: 60% (第一階段) → 80% (最終目標)
- **時程**: 8週
- **優先級**: 🔴 高

### 關鍵指標
| 指標 | 現況 | 3個月目標 | 6個月目標 |
|------|------|-----------|-----------|
| 整體覆蓋率 | 5-10% | 60% | 80% |
| 關鍵業務邏輯 | <5% | 80% | 90% |
| 新增代碼 | 無要求 | 90% | 95% |

## 🎯 Phase 1: 基礎建設 (第1-2週)

### 1.1 設置測試覆蓋率工具

```bash
# 安裝覆蓋率套件
cd test/Kooco.Pikachu.Application.Tests
dotnet add package coverlet.msbuild
dotnet add package coverlet.collector

# 安裝報告生成工具
dotnet tool install --global dotnet-reportgenerator-globaltool
```

### 1.2 建立覆蓋率基準線

創建 `scripts/test-coverage.ps1`:
```powershell
# 執行測試並收集覆蓋率
dotnet test /p:CollectCoverage=true `
    /p:CoverletOutputFormat="cobertura" `
    /p:CoverletOutput="./coverage/" `
    /p:Exclude="[*Test*]*" `
    /p:ExcludeByAttribute="GeneratedCodeAttribute"

# 生成HTML報告
reportgenerator `
    -reports:"./coverage/coverage.cobertura.xml" `
    -targetdir:"./coverage/report" `
    -reporttypes:Html

# 開啟報告
Start-Process "./coverage/report/index.html"
```

### 1.3 整合到CI/CD

更新 `azure-pipelines.yml`:
```yaml
- task: DotNetCoreCLI@2
  displayName: 'Run Tests with Coverage'
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
    arguments: '--configuration $(buildConfiguration) /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura'

- task: PublishCodeCoverageResults@1
  displayName: 'Publish Coverage Results'
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    failIfCoverageEmpty: true
```

## 🚀 Phase 2: 關鍵業務功能測試 (第3-4週)

### 2.1 訂單處理測試

**目標覆蓋率**: 90%

```csharp
// test/Kooco.Pikachu.Application.Tests/Orders/OrderProcessingTests.cs
public class OrderProcessingTests : PikachuApplicationTestBase
{
    [Fact]
    public async Task Should_Create_Order_With_Valid_Items()
    {
        // Test order creation flow
    }

    [Fact]
    public async Task Should_Validate_Inventory_Before_Order()
    {
        // Test inventory validation
    }

    [Fact]
    public async Task Should_Handle_Payment_Processing()
    {
        // Test payment integration
    }
}
```

### 2.2 支付服務測試

**目標覆蓋率**: 95%

- PaymentGatewayAppService
- LinePayAppService
- ECPay integration
- Refund processing

### 2.3 購物車測試

**目標覆蓋率**: 85%

- Add/Remove items
- Quantity validation
- Price calculation
- Discount application

## 📊 Phase 3: 領域層測試 (第5-6週)

### 3.1 核心聚合根測試

```csharp
// test/Kooco.Pikachu.Domain.Tests/Orders/OrderAggregateTests.cs
public class OrderAggregateTests : PikachuDomainTestBase
{
    [Fact]
    public void Should_Not_Allow_Order_Without_Items()
    {
        // Domain rule validation
    }

    [Fact]
    public void Should_Calculate_Total_Correctly()
    {
        // Business logic testing
    }
}
```

### 3.2 業務規則測試優先級

1. **Order Aggregate** - 訂單業務規則
2. **Item/Product** - 商品庫存規則
3. **GroupBuy** - 團購規則
4. **Member** - 會員等級和積分
5. **Discount/Campaign** - 折扣計算

## 🔧 Phase 4: 基礎設施和整合測試 (第7-8週)

### 4.1 Repository測試

```csharp
[Fact]
public async Task Should_Query_Orders_With_Specifications()
{
    // Repository pattern testing
}
```

### 4.2 Integration測試

```csharp
// test/Kooco.Pikachu.HttpApi.Tests/Orders/OrderApiTests.cs
public class OrderApiTests : PikachuHttpApiTestBase
{
    [Fact]
    public async Task Should_Create_Order_Via_API()
    {
        // Full API integration test
    }
}
```

## 📈 測試覆蓋率追蹤

### 每週檢查點

| 週次 | 目標 | 實際 | 新增測試數 | 關鍵完成項目 |
|------|------|------|-----------|------------|
| W1-2 | 15% | - | - | 工具設置 |
| W3-4 | 35% | - | - | 訂單/支付測試 |
| W5-6 | 50% | - | - | 領域層測試 |
| W7-8 | 60% | - | - | 整合測試 |

### 覆蓋率儀表板

創建覆蓋率追蹤看板：
- Azure DevOps Dashboard
- SonarQube integration (optional)
- 每日自動報告

## 🎯 關鍵成功因素

### 1. 團隊承諾
- 所有新代碼必須有測試 (>90%)
- Code Review必須檢查測試
- 每個Sprint分配20%時間寫測試

### 2. 漸進式改善
- 優先測試高風險區域
- 重構時補充測試
- 持續監控和改善

### 3. 自動化執行
- Pre-commit hooks
- PR自動檢查
- 覆蓋率趨勢報告

## 🚨 風險管理

| 風險 | 影響 | 緩解措施 |
|------|------|----------|
| 時程延誤 | 中 | 分階段執行，優先核心功能 |
| 技術債務 | 高 | 邊測試邊重構 |
| 團隊抗拒 | 中 | 培訓和激勵機制 |

## 📝 立即行動項目

### 本週必做 (Week 1)
1. [ ] 安裝測試覆蓋率工具
2. [ ] 執行第一次覆蓋率基準測試
3. [ ] 設置CI/CD覆蓋率報告
4. [ ] 召開團隊會議說明計畫

### 下週計畫 (Week 2)
1. [ ] 開始OrderAppService測試
2. [ ] 完成支付服務測試框架
3. [ ] 建立測試資料生成器

## 📊 預期成果

### 短期 (2個月)
- 覆蓋率達到40-50%
- 關鍵業務功能有保護
- 建立測試文化

### 長期 (6個月)
- 覆蓋率達到80%
- 自動化測試完整
- 顯著降低生產bug

---

**最後更新**: 2025-07-17
**負責人**: 開發團隊
**狀態**: 🟢 執行中