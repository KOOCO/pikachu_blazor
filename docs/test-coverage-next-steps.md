# 測試覆蓋率提升計畫 - 下一步任務

## 當前進度總結
- **當前覆蓋率**: 15.15% (基準線)
- **已完成**: OrderAppService 測試實作
- **時間**: 2025-01-18

## 已完成工作
1. ✅ 設置測試覆蓋率工具 (Coverlet + ReportGenerator)
2. ✅ 建立測試基礎設施和工作流程
3. ✅ 實作 OrderAppService 完整測試套件
   - 建立 TestOrderDataBuilder 測試資料建構器
   - 解決 SQLite RowVersion 約束問題
   - 撰寫 5 個測試檔案，涵蓋付款、狀態、查詢、業務邏輯等面向

## 下一步任務清單

### Phase 1: 關鍵業務功能測試 (目標: 40% 覆蓋率)

#### 1. PaymentGatewayAppService 測試 (優先級: 高)
```
位置: src/Kooco.Pikachu.Application/PaymentGateways/PaymentGatewayAppService.cs
重要性: 處理所有支付相關邏輯，包含 ECPay 和 LinePay 整合
建議測試案例:
- ECPay 付款流程測試
- LinePay 付款流程測試
- 付款回調處理
- 退款處理
- 付款狀態查詢
```

#### 2. ShopCartAppService 測試 (優先級: 高)
```
位置: src/Kooco.Pikachu.Application/ShopCarts/ShopCartAppService.cs
重要性: 購物車是電商核心功能
建議測試案例:
- 加入購物車
- 更新購物車數量
- 移除購物車項目
- 購物車結算
- 庫存檢查
```

#### 3. GroupBuyAppService 測試 (優先級: 高)
```
位置: src/Kooco.Pikachu.Application/GroupBuys/GroupBuyAppService.cs
重要性: 團購是平台特色功能
建議測試案例:
- 團購建立與管理
- 團購狀態轉換
- 團購商品管理
- 團購訂單統計
```

### Phase 2: 核心商品功能測試 (目標: 55% 覆蓋率)

#### 4. ItemAppService 測試 (優先級: 中)
```
現有測試需要擴充，目前測試不完整
建議補充:
- 商品上下架測試
- 庫存管理測試
- 商品圖片處理測試
- 商品規格管理測試
```

#### 5. InventoryAppService 測試 (優先級: 中)
```
位置: src/Kooco.Pikachu.Application/InventoryManagement/
重要性: 庫存管理影響訂單處理
建議測試案例:
- 庫存增減
- 庫存預留
- 庫存釋放
- 庫存警告
```

### Phase 3: 會員與行銷功能測試 (目標: 70% 覆蓋率)

#### 6. MemberAppService 測試
#### 7. CampaignAppService 測試
#### 8. DiscountCodeAppService 測試

## 給下一個 Claude Code Session 的指示

### 開始指令
```
我需要繼續提升 Pikachu Blazor 專案的測試覆蓋率。當前覆蓋率是 15.15%，已經完成了 OrderAppService 的測試實作。

請幫我實作 PaymentGatewayAppService 的測試。這是處理 ECPay 和 LinePay 支付整合的關鍵服務。

相關檔案位置：
- 服務實作: src/Kooco.Pikachu.Application/PaymentGateways/PaymentGatewayAppService.cs
- 測試專案: test/Kooco.Pikachu.Application.Tests/
- 測試基礎設施: test/Kooco.Pikachu.Application.Tests/TestHelpers/TestOrderDataBuilder.cs

請參考已完成的 OrderAppService 測試模式，特別注意：
1. 使用 TestOrderDataBuilder 建立測試資料
2. 處理 SQLite RowVersion 約束問題
3. 遵循 AAA (Arrange-Act-Assert) 模式
4. 涵蓋正常流程和異常情況

專案使用 ABP Framework 9.0.4，測試框架是 xUnit + Shouldly。
```

### 重要提醒
1. **查看 CLAUDE.md**: 包含專案架構、測試工作流程、常用命令
2. **參考現有測試**: OrderAppService 測試可作為範本
3. **執行測試腳本**: 使用 `scripts/test-quick.ps1` 快速測試
4. **檢查覆蓋率**: 使用 `scripts/generate-coverage-report.ps1`

### 測試優先順序
1. PaymentGatewayAppService (支付是關鍵)
2. ShopCartAppService (購物車是核心)
3. GroupBuyAppService (團購是特色)
4. 其他服務按重要性排序

### 注意事項
- 所有測試使用 SQLite 記憶體資料庫
- 注意處理 RowVersion 並發控制欄位
- 外部服務 (ECPay/LinePay) 需要 Mock
- 保持測試獨立性和可重複性

## 預期成果
完成 Phase 1 後，測試覆蓋率應達到 40%，涵蓋所有關鍵業務流程。