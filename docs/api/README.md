# Pikachu API 文檔

## API 概覽

Pikachu 電商平台提供完整的 RESTful API，涵蓋團購、訂單、商品、會員等所有業務功能。所有 API 遵循統一的設計規範，支援標準的 HTTP 方法和狀態碼。

### 基礎資訊

- **基礎 URL**: `https://api.pikachu.com`
- **API 版本**: v1
- **認證方式**: Bearer Token (JWT)
- **內容類型**: `application/json`
- **字元編碼**: UTF-8

### API 設計原則

1. **RESTful 風格**：使用標準 HTTP 動詞（GET、POST、PUT、DELETE）
2. **統一路由**：`/api/app/{resource}`
3. **分頁支援**：列表 API 支援分頁參數
4. **錯誤處理**：統一的錯誤回應格式
5. **國際化**：支援多語言錯誤訊息

## 認證與授權

### 獲取 Token

```http
POST /api/account/login
Content-Type: application/json

{
  "userNameOrEmailAddress": "admin",
  "password": "1q2w3E*",
  "rememberMe": true
}
```

**回應範例**：
```json
{
  "result": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "encryptedAccessToken": "...",
    "expireInSeconds": 86400,
    "userId": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
  },
  "success": true
}
```

### 使用 Token

在所有需要認證的 API 請求中，加入 Authorization header：

```http
Authorization: Bearer {your-access-token}
```

## 核心 API 端點

### 1. 團購管理 API

#### 建立團購

```http
POST /api/app/group-buy
Authorization: Bearer {token}
Content-Type: application/json

{
  "groupBuyName": "2024 春季團購",
  "entryURL": "spring-2024",
  "entryUrl2": "alternate-url",
  "groupBuyNo": "GB202401001",
  "logoURL": "https://cdn.pikachu.com/logo.png",
  "bannerURL": "https://cdn.pikachu.com/banner.png",
  "startTime": "2024-03-01T00:00:00",
  "endTime": "2024-03-31T23:59:59",
  "freeShipping": true,
  "allowShippingMethodChanges": true,
  "paymentMethodForNormal": ["CreditCard", "ATM", "CVS"],
  "paymentMethodForFreeze": ["CreditCard"],
  "paymentMethodForFridge": ["CreditCard", "ATM"],
  "shippingMethodForNormal": ["HomeDelivery", "SevenEleven", "FamilyMart"],
  "customerInformationDescription": "請填寫正確收件資訊",
  "groupBuyConditionDescription": "本團購須滿 100 單才成團",
  "exchangePolicy": "商品售出概不退換",
  "notifyMessage": "團購成功將以 Email 通知"
}
```

**回應範例**：
```json
{
  "id": "3a001f5e-1234-5678-9012-123456789012",
  "groupBuyName": "2024 春季團購",
  "entryURL": "spring-2024",
  "groupBuyNo": "GB202401001",
  "status": "Draft",
  "creationTime": "2024-01-15T10:30:00"
}
```

#### 查詢團購列表

```http
GET /api/app/group-buy?skipCount=0&maxResultCount=20&sorting=creationTime desc
Authorization: Bearer {token}
```

**查詢參數**：
- `skipCount` (int): 跳過筆數，預設 0
- `maxResultCount` (int): 每頁筆數，預設 20，最大 100
- `sorting` (string): 排序欄位，如 `creationTime desc`
- `filter` (string): 搜尋關鍵字
- `status` (string): 狀態篩選 (Draft/Active/Ended)

**回應範例**：
```json
{
  "totalCount": 150,
  "items": [
    {
      "id": "3a001f5e-1234-5678-9012-123456789012",
      "groupBuyName": "2024 春季團購",
      "entryURL": "spring-2024",
      "status": "Active",
      "startTime": "2024-03-01T00:00:00",
      "endTime": "2024-03-31T23:59:59",
      "totalOrders": 85,
      "totalAmount": 125000
    }
  ]
}
```

#### 團購報表

```http
GET /api/app/group-buy/group-buy-report/{groupBuyId}
Authorization: Bearer {token}
```

**回應範例**：
```json
{
  "groupBuyId": "3a001f5e-1234-5678-9012-123456789012",
  "groupBuyName": "2024 春季團購",
  "reportDate": "2024-03-15T14:30:00",
  "summary": {
    "totalOrders": 150,
    "totalAmount": 450000,
    "totalItems": 890,
    "uniqueCustomers": 125
  },
  "itemSales": [
    {
      "itemId": "...",
      "itemName": "有機蔬菜箱",
      "sku": "VEG-001",
      "quantity": 120,
      "amount": 48000
    }
  ],
  "paymentMethodBreakdown": {
    "CreditCard": 85000,
    "ATM": 35000,
    "CVS": 5000
  }
}
```

### 2. 訂單管理 API

#### 建立訂單

```http
POST /api/app/orders
Authorization: Bearer {token}
Content-Type: application/json

{
  "groupBuyId": "3a001f5e-1234-5678-9012-123456789012",
  "customerName": "王小明",
  "customerPhone": "0912345678",
  "customerEmail": "customer@example.com",
  "deliveryMethod": "SevenEleven",
  "storeId": "123456",
  "paymentMethod": "CreditCard",
  "items": [
    {
      "itemId": "...",
      "itemDetailId": "...",
      "quantity": 2,
      "unitPrice": 500
    }
  ],
  "shippingFee": 60,
  "recipientName": "王小明",
  "recipientPhone": "0912345678",
  "recipientAddress": "台北市信義區..."
}
```

**回應範例**：
```json
{
  "id": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "orderNo": "ORD202401150001",
  "status": "Pending",
  "totalAmount": 1060,
  "paymentUrl": "https://payment.ecpay.com.tw/...",
  "creationTime": "2024-01-15T15:30:00"
}
```

#### 訂單狀態更新

```http
PUT /api/app/orders/order-shipped
Authorization: Bearer {token}
Content-Type: application/json

{
  "orderId": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "shippingNumber": "12345678901234",
  "shippingDate": "2024-01-16T10:00:00",
  "logisticsProvider": "BlackCat"
}
```

#### ECPay 支付回調

```http
POST /api/app/orders/callback
Content-Type: application/x-www-form-urlencoded

MerchantID=2000132&MerchantTradeNo=ORD202401150001&PaymentDate=2024/01/15 15:35:00&PaymentType=Credit_CreditCard&PaymentTypeChargeFee=25&RtnCode=1&RtnMsg=交易成功&SimulatePaid=0&TradeAmt=1060&TradeDate=2024/01/15 15:30:00&TradeNo=2401151530000001&CheckMacValue=...
```

### 3. 商品管理 API

#### 商品列表

```http
GET /api/app/items?skipCount=0&maxResultCount=20
Authorization: Bearer {token}
```

**查詢參數**：
- `filter` (string): 商品名稱或編號搜尋
- `categoryId` (guid): 分類篩選
- `isItemAvailable` (bool): 上架狀態
- `minPrice` (decimal): 最低價格
- `maxPrice` (decimal): 最高價格

#### 建立商品

```http
POST /api/app/items
Authorization: Bearer {token}
Content-Type: application/json

{
  "itemName": "有機蔬菜箱",
  "sku": "VEG-001",
  "description": "當季新鮮有機蔬菜組合",
  "itemDetails": [
    {
      "itemName": "標準箱",
      "sku": "VEG-001-S",
      "salePrice": 500,
      "inventoryQuantity": 100,
      "isSoldOut": false
    }
  ],
  "images": [
    {
      "name": "主圖",
      "imageUrl": "https://cdn.pikachu.com/veg-001.jpg",
      "imageType": "Primary",
      "sortNo": 0
    }
  ],
  "isItemAvailable": true,
  "shareProfit": 50,
  "isFreeShipping": false,
  "isReturnable": true
}
```

#### 更新庫存

```http
PUT /api/app/items/{itemId}/inventory
Authorization: Bearer {token}
Content-Type: application/json

{
  "itemDetailId": "...",
  "adjustment": -10,
  "reason": "訂單出貨",
  "referenceNo": "ORD202401150001"
}
```

### 4. 會員管理 API

#### 會員註冊

```http
POST /api/app/members/register
Content-Type: application/json

{
  "name": "王小明",
  "phoneNumber": "0912345678",
  "email": "member@example.com",
  "password": "Pass123!",
  "confirmPassword": "Pass123!",
  "birthday": "1990-01-01",
  "acceptTerms": true
}
```

#### 會員登入

```http
POST /api/app/members/login
Content-Type: application/json

{
  "account": "0912345678",
  "password": "Pass123!"
}
```

#### 會員資料查詢

```http
GET /api/app/members/{memberId}
Authorization: Bearer {token}
```

**回應範例**：
```json
{
  "id": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "name": "王小明",
  "phoneNumber": "0912345678",
  "email": "member@example.com",
  "memberLevel": "Gold",
  "totalPoints": 5280,
  "shoppingCredits": 1000,
  "cumulativeOrders": 25,
  "cumulativeAmount": 85000,
  "registrationDate": "2023-01-15T10:00:00"
}
```

### 5. 購物車 API

#### 加入購物車

```http
POST /api/app/shop-carts/{userId}/{groupBuyId}/cart-items
Authorization: Bearer {token}
Content-Type: application/json

{
  "itemId": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "itemDetailId": "3a001f5e-yyyy-yyyy-yyyy-yyyyyyyyyyyy",
  "quantity": 2
}
```

#### 更新購物車項目

```http
PUT /api/app/shop-carts/cart-items/{cartItemId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "quantity": 3
}
```

#### 購物車驗證

```http
POST /api/app/shop-carts/cart-items/{userId}/{groupBuyId}/verify
Authorization: Bearer {token}
```

**回應範例**：
```json
{
  "isValid": true,
  "items": [
    {
      "cartItemId": "...",
      "itemId": "...",
      "itemName": "有機蔬菜箱",
      "quantity": 2,
      "unitPrice": 500,
      "subtotal": 1000,
      "isAvailable": true,
      "availableQuantity": 50
    }
  ],
  "summary": {
    "itemTotal": 1000,
    "shippingFee": 60,
    "discount": 0,
    "totalAmount": 1060
  },
  "warnings": []
}
```

## 支付網關 API

### ECPay 結帳

```http
GET /api/app/orders/ecpay-proceed-to-checkout?orderId={orderId}
Authorization: Bearer {token}
```

**回應**：重導向至 ECPay 支付頁面

### LinePay 請求

```http
POST /api/app/line-pay/request
Authorization: Bearer {token}
Content-Type: application/json

{
  "orderId": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "amount": 1060,
  "currency": "TWD",
  "confirmUrl": "https://yoursite.com/line-pay/confirm",
  "cancelUrl": "https://yoursite.com/line-pay/cancel"
}
```

### LinePay 確認

```http
POST /api/app/line-pay/confirm
Authorization: Bearer {token}
Content-Type: application/json

{
  "transactionId": "2024011500000001",
  "orderId": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

## 錯誤處理

### 錯誤回應格式

所有 API 錯誤都使用統一格式回應：

```json
{
  "error": {
    "code": "Pikachu:GroupBuyNotFound",
    "message": "找不到指定的團購活動",
    "details": "GroupBuy with id '3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx' was not found",
    "validationErrors": null
  }
}
```

### 常見錯誤碼

| HTTP 狀態碼 | 錯誤碼 | 說明 |
|------------|--------|------|
| 400 | Pikachu:InvalidInput | 輸入參數無效 |
| 401 | Unauthorized | 未授權存取 |
| 403 | Forbidden | 無權限執行此操作 |
| 404 | Pikachu:EntityNotFound | 找不到指定資源 |
| 409 | Pikachu:DuplicateEntity | 資源已存在 |
| 422 | Pikachu:BusinessRule | 違反業務規則 |
| 500 | InternalServerError | 伺服器內部錯誤 |

### 驗證錯誤範例

```json
{
  "error": {
    "code": "ValidationError",
    "message": "輸入驗證失敗",
    "details": null,
    "validationErrors": [
      {
        "message": "CustomerName 為必填欄位",
        "members": ["customerName"]
      },
      {
        "message": "CustomerPhone 格式不正確",
        "members": ["customerPhone"]
      }
    ]
  }
}
```

## 分頁與排序

### 分頁參數

所有列表 API 都支援標準分頁參數：

- `SkipCount` (int): 跳過筆數，預設 0
- `MaxResultCount` (int): 每頁筆數，預設 20，最大 100

### 排序參數

- `Sorting` (string): 排序欄位和方向，如 `creationTime desc`
- 支援多欄位排序：`name asc, creationTime desc`

### 分頁回應格式

```json
{
  "totalCount": 250,
  "items": [...]
}
```

## API 使用範例

### cURL 範例

```bash
# 登入取得 Token
curl -X POST https://api.pikachu.com/api/account/login \
  -H "Content-Type: application/json" \
  -d '{"userNameOrEmailAddress":"admin","password":"1q2w3E*"}'

# 查詢團購列表
curl -X GET "https://api.pikachu.com/api/app/group-buy?maxResultCount=10" \
  -H "Authorization: Bearer {token}"

# 建立訂單
curl -X POST https://api.pikachu.com/api/app/orders \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d @order.json
```

### Postman Collection

我們提供完整的 Postman Collection，包含所有 API 端點和範例：

[下載 Postman Collection](https://api.pikachu.com/docs/pikachu-api.postman_collection.json)

### JavaScript/Axios 範例

```javascript
// 設定 axios 預設值
const apiClient = axios.create({
  baseURL: 'https://api.pikachu.com',
  headers: {
    'Content-Type': 'application/json'
  }
});

// 登入並設定 token
async function login() {
  const response = await apiClient.post('/api/account/login', {
    userNameOrEmailAddress: 'admin',
    password: '1q2w3E*'
  });
  
  const token = response.data.result.accessToken;
  apiClient.defaults.headers.common['Authorization'] = `Bearer ${token}`;
}

// 查詢團購
async function getGroupBuys() {
  const response = await apiClient.get('/api/app/group-buy', {
    params: {
      skipCount: 0,
      maxResultCount: 20
    }
  });
  
  return response.data;
}

// 建立訂單
async function createOrder(orderData) {
  try {
    const response = await apiClient.post('/api/app/orders', orderData);
    return response.data;
  } catch (error) {
    if (error.response && error.response.data.error) {
      console.error('API Error:', error.response.data.error.message);
    }
    throw error;
  }
}
```

## API 限制與配額

### 請求限制

- 每個 IP 每分鐘最多 60 次請求
- 每個用戶每小時最多 1000 次請求
- 批量操作每次最多處理 100 筆資料

### 回應大小限制

- 單一回應最大 10MB
- 檔案上傳最大 50MB
- 圖片上傳支援格式：JPG、PNG、GIF、WebP

### 逾時設定

- API 請求逾時：30 秒
- 檔案上傳逾時：5 分鐘
- 長時間運行的操作會回傳工作 ID 供查詢進度

## Webhook 事件

Pikachu API 支援 Webhook 通知重要事件：

### 支援的事件類型

- `order.created` - 訂單建立
- `order.paid` - 訂單付款完成
- `order.shipped` - 訂單出貨
- `order.completed` - 訂單完成
- `order.cancelled` - 訂單取消
- `groupbuy.started` - 團購開始
- `groupbuy.ended` - 團購結束

### Webhook 格式

```json
{
  "id": "evt_1234567890",
  "type": "order.paid",
  "created": "2024-01-15T15:30:00Z",
  "data": {
    "orderId": "3a001f5e-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "orderNo": "ORD202401150001",
    "amount": 1060,
    "paymentMethod": "CreditCard"
  }
}
```

### 設定 Webhook

請在管理後台的「系統設定」>「Webhook」中設定接收端點。

## 更新日誌

### v1.2.0 (2024-01-15)
- 新增 LinePay 支付整合
- 支援批量更新商品庫存
- 優化團購報表效能

### v1.1.0 (2023-12-01)
- 新增購物金功能 API
- 支援會員等級查詢
- 改善錯誤訊息國際化

### v1.0.0 (2023-10-01)
- 初始版本發布
- 核心電商功能 API

## 聯絡支援

如有 API 相關問題，請聯絡：

- 技術支援：api-support@pikachu.com
- API 文檔：https://api.pikachu.com/docs
- 開發者論壇：https://forum.pikachu.com/api