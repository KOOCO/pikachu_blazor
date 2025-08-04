# 測試覆蓋率報告

## 📊 當前覆蓋率
![Line Coverage](./reports/badge_linecoverage.svg)
![Branch Coverage](./reports/badge_branchcoverage.svg)
![Method Coverage](./reports/badge_methodcoverage.svg)

最後更新：2025-07-18

## 📈 覆蓋率目標
- **3個月目標**: 60%
- **6個月目標**: 80%
- **新代碼標準**: >90%

## 🔗 查看詳細報告
1. 執行測試並生成報告：
   ```bash
   pwsh scripts/run-tests-with-coverage.ps1 -OpenReport
   ```

2. 本地查看報告：
   ```bash
   open coverage/reports/index.html
   ```

## 📝 覆蓋率追蹤

| 日期 | 整體覆蓋率 | Application | Domain | 備註 |
|------|-----------|-------------|---------|------|
| 2025-07-18 | 15.15% | 7.08% | 28.02% | 初始基準 |

## 🎯 優先改善區域
1. **OrderAppService** - 訂單處理邏輯
2. **PaymentGatewayAppService** - 支付相關
3. **ShopCartAppService** - 購物車功能
4. **InventoryAppService** - 庫存管理

---
更多資訊請參考 [測試覆蓋率提升計畫](../docs/test-coverage-improvement-plan.md)