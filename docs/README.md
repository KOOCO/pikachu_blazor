# Pikachu Blazor 電商平台

## 專案概述

Pikachu Blazor 是一個基於 ABP Framework 構建的現代化電商平台，專注於團購業務模式。系統採用領域驅動設計（DDD）架構，提供完整的電商解決方案，包含商品管理、訂單處理、會員系統、支付整合、物流配送等核心功能。

### 核心特色

- 🛍️ **團購功能**：完整的團購管理系統，支援多階段促銷、限時限量購買
- 💳 **多元支付**：整合 ECPay、LinePay、中國信託等主流支付網關
- 🚚 **智慧物流**：支援黑貓、7-11、全家等物流商，含冷鏈配送
- 👥 **會員系統**：完善的會員管理、積分、購物金、VIP 等級制度
- 📊 **數據分析**：即時儀表板、銷售報表、會員行為分析
- 🌐 **多租戶**：原生支援多租戶架構，適合 SaaS 模式營運

## 技術棧

### 後端技術
- **.NET 9.0** - 最新的 .NET 平台
- **ABP Framework 9.0.4** - 企業級應用框架
- **Entity Framework Core 9.0.1** - ORM 框架
- **SQL Server / Azure SQL** - 資料庫
- **Hangfire 1.8.14** - 背景作業處理
- **Serilog** - 結構化日誌

### 前端技術
- **Blazor Server** - 互動式 Web UI 框架
- **AntDesign Blazor 1.1.4** - 企業級 UI 組件
- **MudBlazor 8.1.0** - Material Design 組件
- **Blazorise 1.7.3** - Blazor 組件庫
- **TinyMCE** - 富文本編輯器

### 第三方整合
- **ECPay** - 綠界金流與電子發票
- **LinePay** - LINE Pay 支付
- **中國信託** - 信用卡支付
- **黑貓宅急便** - 物流服務
- **7-11、全家** - 超商取貨

## 快速開始指南

### 環境需求

- .NET 9.0 SDK 或更高版本
- SQL Server 2019 或 Azure SQL Database
- Visual Studio 2022 / VS Code / Rider
- Node.js 18+ (僅前端開發需要)

### 開發環境設定

1. **克隆專案**
   ```bash
   git clone https://github.com/your-org/pikachu_blazor.git
   cd pikachu_blazor
   ```

2. **還原套件**
   ```bash
   dotnet restore
   ```

3. **設定資料庫連線**
   
   編輯 `src/Kooco.Pikachu.DbMigrator/appsettings.json`：
   ```json
   {
     "ConnectionStrings": {
       "Default": "Server=localhost;Database=PikachuDb;Trusted_Connection=True;TrustServerCertificate=True"
     }
   }
   ```

4. **執行資料庫遷移**
   ```bash
   dotnet run --project src/Kooco.Pikachu.DbMigrator
   ```

5. **啟動應用程式**
   ```bash
   dotnet run --project src/Kooco.Pikachu.Blazor
   ```

6. **訪問應用程式**
   - 主應用：https://localhost:44320
   - 預設管理員：admin / 1q2w3E*

### 建置與發佈

#### 開發建置
```bash
dotnet build src/Kooco.Pikachu.Blazor
```

#### 生產建置
```bash
dotnet build src/Kooco.Pikachu.Blazor --configuration Release
```

#### 發佈應用程式
```bash
dotnet publish src/Kooco.Pikachu.Blazor --configuration Release --output ./published
```

## 專案結構

```
pikachu_blazor/
├── src/                                    # 原始碼目錄
│   ├── Kooco.Pikachu.Domain.Shared/      # 共享領域層
│   ├── Kooco.Pikachu.Domain/             # 領域層
│   ├── Kooco.Pikachu.Application.Contracts/ # 應用層契約
│   ├── Kooco.Pikachu.Application/        # 應用層實現
│   ├── Kooco.Pikachu.EntityFrameworkCore/ # 資料存取層
│   ├── Kooco.Pikachu.HttpApi/            # HTTP API 層
│   ├── Kooco.Pikachu.Blazor/             # Blazor UI 層
│   ├── Kooco.Pikachu.Blazor.Store/       # 商店前台 UI
│   └── Kooco.Pikachu.DbMigrator/         # 資料庫遷移工具
├── test/                                   # 測試專案
├── lib/                                    # 第三方程式庫
│   ├── Kooco.ECPay/                       # ECPay 整合
│   └── Kooco.Core/                        # 核心工具庫
└── docs/                                   # 文檔目錄
```

## 核心功能模組

### 🛒 團購管理
- 團購活動建立與管理
- 多階段促銷設定
- 限時限量控制
- 團購報表與分析

### 📦 商品管理
- 商品資訊維護
- 庫存管理
- 商品分類與標籤
- 套裝商品組合

### 📋 訂單處理
- 訂單生命週期管理
- 多通路訂單整合
- 發票開立與管理
- 退貨退款處理

### 👤 會員系統
- 會員註冊與認證
- 會員等級制度
- 購物金與積分
- 會員標籤管理

### 💰 支付整合
- 信用卡支付
- 行動支付
- 超商代收
- 分期付款

### 🚛 物流配送
- 宅配服務
- 超商取貨
- 冷鏈配送
- 物流追蹤

## 開發指南

### 新增功能模組

1. 在 Domain 層建立實體
2. 在 Application.Contracts 定義 DTO
3. 在 Application 層實作服務
4. 在 HttpApi 層建立控制器
5. 在 Blazor 層建立頁面

### 資料庫遷移

```bash
# 新增遷移
dotnet ef migrations add YourMigrationName -p src/Kooco.Pikachu.EntityFrameworkCore -s src/Kooco.Pikachu.DbMigrator

# 更新資料庫
dotnet run --project src/Kooco.Pikachu.DbMigrator
```

### 執行測試

```bash
# 執行所有測試
dotnet test

# 執行特定專案測試
dotnet test test/Kooco.Pikachu.Application.Tests
```

## 部署說明

### Azure 部署

1. 建立 Azure SQL Database
2. 建立 App Service (Windows, .NET 9)
3. 設定連線字串與應用程式設定
4. 使用 GitHub Actions 或 Azure DevOps 進行 CI/CD

### Docker 部署

```dockerfile
# 建置映像
docker build -t pikachu-blazor .

# 執行容器
docker run -d -p 80:80 --name pikachu pikachu-blazor
```

## 設定說明

### 支付網關設定

在 `appsettings.json` 中設定：

```json
{
  "ECPay": {
    "MerchantId": "your-merchant-id",
    "HashKey": "your-hash-key",
    "HashIV": "your-hash-iv"
  },
  "LinePay": {
    "ChannelId": "your-channel-id",
    "ChannelSecret": "your-channel-secret"
  }
}
```

### 郵件服務設定

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "UserName": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true
  }
}
```

## 疑難排解

### 常見問題

1. **資料庫連線失敗**
   - 檢查連線字串設定
   - 確認 SQL Server 服務啟動
   - 檢查防火牆設定

2. **支付回調失敗**
   - 確認回調 URL 可公開存取
   - 檢查 HTTPS 憑證
   - 查看支付網關日誌

3. **背景作業未執行**
   - 檢查 Hangfire 儀表板
   - 確認資料庫連線正常
   - 查看應用程式日誌

## 參與貢獻

我們歡迎所有形式的貢獻！請查看 [CONTRIBUTING.md](CONTRIBUTING.md) 了解詳情。

## 授權

本專案採用 [MIT License](LICENSE) 授權。

## 聯絡資訊

- 專案維護者：Kooco Team
- Email：support@kooco.com
- 問題回報：[GitHub Issues](https://github.com/your-org/pikachu_blazor/issues)

## 更新日誌

查看 [CHANGELOG.md](CHANGELOG.md) 了解版本更新資訊。