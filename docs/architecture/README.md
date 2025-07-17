# Pikachu 系統架構文檔

## 架構概述

Pikachu 電商平台採用基於 ABP Framework 的分層架構設計，遵循領域驅動設計（DDD）原則。系統使用 Blazor Server 作為前端技術，提供即時互動的使用者體驗，後端則採用 .NET 9 構建高效能的 API 服務。

### 架構特點

- **分層架構**：清晰的層次劃分，各層職責明確
- **領域驅動**：業務邏輯集中在領域層
- **模組化設計**：功能模組獨立，易於擴展
- **多租戶支援**：原生支援 SaaS 模式
- **微服務就緒**：可輕易拆分為微服務架構

## 系統架構圖

### 整體架構

```mermaid
graph TB
    subgraph "客戶端"
        Browser[瀏覽器]
        Mobile[手機 APP]
        POS[POS 系統]
    end
    
    subgraph "前端層"
        BlazorUI[Blazor Server UI]
        BlazorStore[Blazor Store UI]
    end
    
    subgraph "API 層"
        HttpApi[HTTP API Controllers]
        SignalR[SignalR Hub]
    end
    
    subgraph "應用層"
        AppService[Application Services]
        BackgroundJobs[Background Jobs]
        EventHandlers[Domain Event Handlers]
    end
    
    subgraph "領域層"
        Entities[領域實體]
        DomainServices[領域服務]
        DomainEvents[領域事件]
        Specifications[規約模式]
    end
    
    subgraph "基礎設施層"
        EFCore[Entity Framework Core]
        Repositories[倉儲實現]
        ExternalServices[外部服務整合]
    end
    
    subgraph "資料存儲"
        SQLServer[(SQL Server)]
        Redis[(Redis Cache)]
        BlobStorage[Blob Storage]
    end
    
    subgraph "外部服務"
        ECPay[ECPay 金流]
        LinePay[Line Pay]
        Logistics[物流 API]
        SMS[簡訊服務]
        Email[郵件服務]
    end
    
    Browser --> BlazorUI
    Mobile --> HttpApi
    POS --> HttpApi
    
    BlazorUI --> SignalR
    BlazorUI --> HttpApi
    BlazorStore --> HttpApi
    
    HttpApi --> AppService
    SignalR --> AppService
    
    AppService --> DomainServices
    AppService --> Entities
    AppService --> EventHandlers
    BackgroundJobs --> AppService
    
    DomainServices --> Entities
    DomainServices --> DomainEvents
    
    AppService --> Repositories
    Repositories --> EFCore
    
    EFCore --> SQLServer
    AppService --> Redis
    AppService --> BlobStorage
    
    ExternalServices --> ECPay
    ExternalServices --> LinePay
    ExternalServices --> Logistics
    ExternalServices --> SMS
    ExternalServices --> Email
```

### DDD 分層架構

```mermaid
graph LR
    subgraph "展現層 (Presentation)"
        UI[Blazor UI]
        API[HTTP API]
    end
    
    subgraph "應用層 (Application)"
        AppServices[應用服務]
        DTOs[資料傳輸物件]
        AppContracts[應用契約]
    end
    
    subgraph "領域層 (Domain)"
        AggregateRoots[聚合根]
        Entities[實體]
        ValueObjects[值物件]
        DomainServices[領域服務]
        DomainEvents[領域事件]
        Repositories[倉儲介面]
    end
    
    subgraph "基礎設施層 (Infrastructure)"
        DataAccess[資料存取]
        ExternalIntegrations[外部整合]
        CrossCutting[橫切關注點]
    end
    
    UI --> AppServices
    API --> AppServices
    AppServices --> DTOs
    AppServices --> AppContracts
    AppServices --> AggregateRoots
    AppServices --> DomainServices
    DomainServices --> Entities
    DomainServices --> ValueObjects
    AggregateRoots --> DomainEvents
    AppServices --> Repositories
    DataAccess --> Repositories
```

## 主要元件說明

### 1. 展現層 (Presentation Layer)

#### Blazor Server UI
- **職責**：提供互動式的網頁介面
- **技術**：Blazor Server、SignalR
- **元件庫**：AntDesign Blazor、MudBlazor
- **特點**：
  - 即時 UI 更新
  - 減少客戶端負載
  - 統一的元件化開發

#### HTTP API Controllers
- **職責**：提供 RESTful API 端點
- **技術**：ASP.NET Core Web API
- **特點**：
  - 自動 API 文檔生成
  - 統一的錯誤處理
  - API 版本管理

### 2. 應用層 (Application Layer)

#### Application Services
- **職責**：協調業務流程，處理用例
- **範例**：
```csharp
public class OrderAppService : ApplicationService, IOrderAppService
{
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto input)
    {
        // 驗證輸入
        // 調用領域服務
        // 發布領域事件
        // 返回 DTO
    }
}
```

#### Background Jobs
- **職責**：處理非同步任務
- **技術**：Hangfire
- **任務類型**：
  - 訂單狀態同步
  - 庫存檢查
  - 報表生成
  - 郵件發送

### 3. 領域層 (Domain Layer)

#### 聚合根 (Aggregate Roots)
- **GroupBuy**：團購聚合根
- **Order**：訂單聚合根
- **Item**：商品聚合根
- **Member**：會員聚合根

#### 領域服務 (Domain Services)
- **OrderManager**：訂單業務邏輯
- **InventoryManager**：庫存管理
- **PricingService**：價格計算
- **PromotionEngine**：促銷引擎

#### 領域事件 (Domain Events)
```csharp
public class OrderCreatedEvent : DomainEventBase
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public decimal TotalAmount { get; set; }
}
```

### 4. 基礎設施層 (Infrastructure Layer)

#### Entity Framework Core
- **DbContext**：資料庫上下文
- **Migrations**：資料庫遷移
- **配置**：Fluent API 配置

#### 外部服務整合
- **支付整合**：ECPay、LinePay、中國信託
- **物流整合**：黑貓、7-11、全家
- **通知服務**：簡訊、Email

## 資料流程圖

### 訂單建立流程

```mermaid
sequenceDiagram
    participant User as 使用者
    participant UI as Blazor UI
    participant API as API Controller
    participant AppService as 應用服務
    participant Domain as 領域服務
    participant Repo as 倉儲
    participant DB as 資料庫
    participant Payment as 支付服務
    participant Event as 事件匯流排
    
    User->>UI: 提交訂單
    UI->>API: POST /api/orders
    API->>AppService: CreateOrderAsync(dto)
    
    AppService->>Domain: 驗證商品庫存
    Domain->>Repo: 查詢商品資訊
    Repo->>DB: SELECT Items
    DB-->>Repo: 商品資料
    Repo-->>Domain: 商品實體
    Domain-->>AppService: 庫存驗證結果
    
    alt 庫存充足
        AppService->>Domain: 建立訂單
        Domain->>Domain: 計算金額
        Domain->>Domain: 套用促銷
        Domain->>Repo: 保存訂單
        Repo->>DB: INSERT Order
        
        AppService->>Payment: 建立支付請求
        Payment-->>AppService: 支付 URL
        
        AppService->>Event: 發布 OrderCreated 事件
        Event-->>Event: 觸發後續處理
        
        AppService-->>API: OrderDto
        API-->>UI: 訂單資訊 + 支付 URL
        UI-->>User: 導向支付頁面
    else 庫存不足
        AppService-->>API: 錯誤訊息
        API-->>UI: 庫存不足錯誤
        UI-->>User: 顯示錯誤訊息
    end
```

### 支付回調流程

```mermaid
sequenceDiagram
    participant Payment as 支付網關
    participant API as API Controller
    participant AppService as 應用服務
    participant Domain as 領域服務
    participant Repo as 倉儲
    participant DB as 資料庫
    participant Event as 事件匯流排
    participant Queue as 訊息佇列
    
    Payment->>API: POST /api/orders/callback
    API->>API: 驗證回調簽名
    
    alt 簽名有效
        API->>AppService: ProcessPaymentCallback(data)
        AppService->>Repo: 查詢訂單
        Repo->>DB: SELECT Order
        DB-->>Repo: 訂單資料
        Repo-->>AppService: 訂單實體
        
        AppService->>Domain: 更新支付狀態
        Domain->>Domain: 檢查訂單狀態
        Domain->>Domain: 更新為已付款
        Domain->>Repo: 保存變更
        Repo->>DB: UPDATE Order
        
        AppService->>Event: 發布 OrderPaid 事件
        Event->>Queue: 訊息入列
        
        Queue-->>Queue: 庫存扣減任務
        Queue-->>Queue: 發票開立任務
        Queue-->>Queue: 通知發送任務
        
        AppService-->>API: 成功回應
        API-->>Payment: HTTP 200 OK
    else 簽名無效
        API-->>Payment: HTTP 400 Bad Request
    end
```

## 時序圖

### 團購商品查詢

```mermaid
sequenceDiagram
    participant User as 使用者
    participant Cache as Redis Cache
    participant API as API
    participant Service as 應用服務
    participant DB as 資料庫
    
    User->>API: GET /api/group-buy/{id}
    API->>Service: GetGroupBuyAsync(id)
    Service->>Cache: 查詢快取
    
    alt 快取命中
        Cache-->>Service: 團購資料
    else 快取未命中
        Service->>DB: 查詢團購
        DB-->>Service: 團購資料
        Service->>Cache: 寫入快取
    end
    
    Service->>Service: 載入商品列表
    Service->>Service: 計算即時銷量
    Service-->>API: GroupBuyDto
    API-->>User: 團購資訊
```

### 購物車結帳

```mermaid
sequenceDiagram
    participant User as 使用者
    participant UI as UI
    participant API as API
    participant Cart as 購物車服務
    participant Order as 訂單服務
    participant Payment as 支付服務
    
    User->>UI: 點擊結帳
    UI->>API: POST /api/shop-carts/checkout
    API->>Cart: ValidateCart(userId, groupBuyId)
    
    Cart->>Cart: 驗證商品可用性
    Cart->>Cart: 驗證庫存
    Cart->>Cart: 計算金額
    
    alt 驗證通過
        Cart-->>API: 購物車資訊
        API->>Order: CreateFromCart(cartInfo)
        Order->>Order: 建立訂單
        Order->>Order: 套用優惠
        Order->>Payment: 建立支付
        Payment-->>Order: 支付資訊
        Order-->>API: 訂單資訊
        API-->>UI: 訂單 + 支付 URL
        UI-->>User: 導向支付
    else 驗證失敗
        Cart-->>API: 錯誤訊息
        API-->>UI: 錯誤資訊
        UI-->>User: 顯示錯誤
    end
```

## 模組依賴關係

```mermaid
graph TD
    subgraph "UI 模組"
        Blazor[Kooco.Pikachu.Blazor]
        BlazorStore[Kooco.Pikachu.Blazor.Store]
    end
    
    subgraph "API 模組"
        HttpApi[Kooco.Pikachu.HttpApi]
        HttpApiClient[Kooco.Pikachu.HttpApi.Client]
    end
    
    subgraph "應用模組"
        Application[Kooco.Pikachu.Application]
        AppContracts[Kooco.Pikachu.Application.Contracts]
    end
    
    subgraph "領域模組"
        Domain[Kooco.Pikachu.Domain]
        DomainShared[Kooco.Pikachu.Domain.Shared]
    end
    
    subgraph "基礎設施模組"
        EFCore[Kooco.Pikachu.EntityFrameworkCore]
        DbMigrator[Kooco.Pikachu.DbMigrator]
    end
    
    Blazor --> HttpApiClient
    Blazor --> AppContracts
    BlazorStore --> Blazor
    
    HttpApi --> Application
    HttpApiClient --> AppContracts
    
    Application --> AppContracts
    Application --> Domain
    Application --> EFCore
    
    AppContracts --> DomainShared
    Domain --> DomainShared
    
    EFCore --> Domain
    DbMigrator --> EFCore
```

## 部署架構

### 生產環境部署

```mermaid
graph TB
    subgraph "Azure"
        subgraph "前端"
            AppService1[App Service - Blazor UI]
            AppService2[App Service - API]
        end
        
        subgraph "應用層"
            AppService3[App Service - Background Jobs]
        end
        
        subgraph "資料層"
            AzureSQL[(Azure SQL Database)]
            AzureRedis[(Azure Redis Cache)]
            BlobStorage[Azure Blob Storage]
        end
        
        subgraph "監控"
            AppInsights[Application Insights]
            LogAnalytics[Log Analytics]
        end
        
        CDN[Azure CDN]
        WAF[Web Application Firewall]
    end
    
    subgraph "外部服務"
        ECPay[ECPay]
        LinePay[Line Pay]
        LogisticsAPI[物流 API]
    end
    
    Internet[網際網路] --> CDN
    CDN --> WAF
    WAF --> AppService1
    WAF --> AppService2
    
    AppService1 --> AppService2
    AppService2 --> AzureSQL
    AppService2 --> AzureRedis
    AppService2 --> BlobStorage
    
    AppService3 --> AzureSQL
    AppService3 --> AzureRedis
    
    AppService2 --> ECPay
    AppService2 --> LinePay
    AppService2 --> LogisticsAPI
    
    AppService1 --> AppInsights
    AppService2 --> AppInsights
    AppService3 --> AppInsights
    AppInsights --> LogAnalytics
```

### 容器化部署

```mermaid
graph TB
    subgraph "Kubernetes Cluster"
        subgraph "Ingress"
            Nginx[Nginx Ingress]
        end
        
        subgraph "應用 Pods"
            BlazorPod[Blazor UI Pod x3]
            APIPod[API Pod x5]
            JobsPod[Jobs Pod x2]
        end
        
        subgraph "資料服務"
            RedisPod[Redis Pod]
            SQLProxy[Cloud SQL Proxy]
        end
        
        subgraph "監控"
            Prometheus[Prometheus]
            Grafana[Grafana]
        end
    end
    
    subgraph "外部資源"
        CloudSQL[(Cloud SQL)]
        CloudStorage[Cloud Storage]
    end
    
    Internet[網際網路] --> Nginx
    Nginx --> BlazorPod
    Nginx --> APIPod
    
    BlazorPod --> APIPod
    APIPod --> RedisPod
    APIPod --> SQLProxy
    SQLProxy --> CloudSQL
    APIPod --> CloudStorage
    
    JobsPod --> RedisPod
    JobsPod --> SQLProxy
    
    Prometheus --> BlazorPod
    Prometheus --> APIPod
    Prometheus --> JobsPod
    Grafana --> Prometheus
```

## 安全架構

### 安全層級

```mermaid
graph TB
    subgraph "網路安全"
        WAF[Web 應用程式防火牆]
        DDoS[DDoS 保護]
        SSL[SSL/TLS 加密]
    end
    
    subgraph "應用安全"
        Auth[身份驗證 - JWT]
        Authz[授權 - 權限系統]
        CSRF[CSRF 防護]
        XSS[XSS 防護]
    end
    
    subgraph "API 安全"
        RateLimit[速率限制]
        APIKey[API Key 管理]
        OAuth[OAuth 2.0]
    end
    
    subgraph "資料安全"
        Encryption[資料加密]
        Backup[備份策略]
        Audit[稽核日誌]
    end
    
    Internet[外部請求] --> WAF
    WAF --> DDoS
    DDoS --> SSL
    SSL --> Auth
    Auth --> Authz
    Authz --> CSRF
    CSRF --> XSS
    XSS --> RateLimit
    RateLimit --> APIKey
    APIKey --> OAuth
    OAuth --> Encryption
    Encryption --> Backup
    Backup --> Audit
```

## 效能優化架構

### 快取策略

```mermaid
graph LR
    subgraph "多層快取"
        Browser[瀏覽器快取]
        CDN[CDN 快取]
        Redis[Redis 快取]
        Memory[記憶體快取]
        DB[資料庫快取]
    end
    
    Request[請求] --> Browser
    Browser -->|未命中| CDN
    CDN -->|未命中| Memory
    Memory -->|未命中| Redis
    Redis -->|未命中| DB
```

### 非同步處理

```mermaid
graph TB
    subgraph "同步處理"
        API1[API 請求]
        Validate[驗證]
        Save[保存資料]
        Response1[回應]
    end
    
    subgraph "非同步處理"
        Queue[訊息佇列]
        Worker1[庫存扣減]
        Worker2[發票開立]
        Worker3[通知發送]
        Worker4[報表生成]
    end
    
    API1 --> Validate
    Validate --> Save
    Save --> Response1
    Save --> Queue
    
    Queue --> Worker1
    Queue --> Worker2
    Queue --> Worker3
    Queue --> Worker4
```

## 擴展性設計

### 水平擴展

1. **無狀態設計**
   - Session 存儲在 Redis
   - 檔案存儲在 Blob Storage
   - 分散式快取

2. **負載均衡**
   - Application Gateway
   - 健康檢查端點
   - 自動擴展規則

3. **資料庫擴展**
   - 讀寫分離
   - 分片策略
   - 連線池管理

### 垂直擴展

1. **模組化架構**
   - 獨立部署單元
   - 清晰的介面定義
   - 鬆耦合設計

2. **插件系統**
   - 動態載入模組
   - 擴展點設計
   - 事件驅動架構

## 監控與維運

### 監控架構

```mermaid
graph TB
    subgraph "應用監控"
        APM[Application Insights]
        Logs[集中式日誌]
        Metrics[效能指標]
        Traces[分散式追蹤]
    end
    
    subgraph "基礎設施監控"
        Server[伺服器監控]
        Network[網路監控]
        Database[資料庫監控]
    end
    
    subgraph "業務監控"
        Orders[訂單監控]
        Payment[支付監控]
        Inventory[庫存監控]
    end
    
    subgraph "告警系統"
        Rules[告警規則]
        Notification[通知管道]
        Escalation[升級機制]
    end
    
    APM --> Rules
    Logs --> Rules
    Metrics --> Rules
    Server --> Rules
    Orders --> Rules
    
    Rules --> Notification
    Notification --> Escalation
```

### 健康檢查

```csharp
// 健康檢查端點配置
services.AddHealthChecks()
    .AddSqlServer(connectionString, name: "sql")
    .AddRedis(redisConnection, name: "redis")
    .AddUrlGroup(new Uri("https://api.ecpay.com.tw/health"), name: "ecpay")
    .AddCheck<CustomHealthCheck>("custom");
```

## 災難復原

### 備份策略

1. **資料備份**
   - 自動備份：每日
   - 保留期限：30 天
   - 異地備份：啟用

2. **應用備份**
   - 程式碼：Git 版控
   - 配置：Azure Key Vault
   - 容器映像：Container Registry

3. **復原計劃**
   - RTO：< 4 小時
   - RPO：< 1 小時
   - 定期演練：每季

## 技術債管理

### 識別與追蹤

1. **程式碼品質**
   - SonarQube 分析
   - 程式碼覆蓋率
   - 技術債指標

2. **架構適應性**
   - 定期架構審查
   - 效能基準測試
   - 擴展性評估

3. **優先級管理**
   - 影響評估矩陣
   - 改善計劃
   - 資源分配

## 未來展望

### 短期目標（3-6 個月）

1. **微服務轉型**
   - 訂單服務獨立
   - 支付服務獨立
   - API Gateway 導入

2. **效能優化**
   - 資料庫查詢優化
   - 快取策略改進
   - 前端載入優化

### 長期目標（6-12 個月）

1. **雲原生架構**
   - Kubernetes 部署
   - Service Mesh
   - 無伺服器函數

2. **AI/ML 整合**
   - 推薦系統
   - 需求預測
   - 智慧定價

## 結論

Pikachu 電商平台的架構設計充分考慮了可擴展性、可維護性和高可用性。透過分層架構和領域驅動設計，系統能夠靈活應對業務需求的變化，同時保持程式碼的清晰和可測試性。未來將持續優化架構，朝向更加雲原生和智慧化的方向發展。