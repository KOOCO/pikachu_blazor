using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.StoreComments;
using Kooco.Pikachu.UserShoppingCredits;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Orders.Entities;

/// <summary>
/// 訂單
/// </summary>
public class Order : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    [NotMapped]
    public Guid OrderId { get; set; }

    /// <summary>
    /// 租戶識別碼
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderNo { get; set; }

    /// <summary>
    /// 是否為個人訂單
    /// </summary>
    public bool IsIndividual { get; set; }

    /// <summary>
    /// 顧客姓名
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// 顧客電話
    /// </summary>
    public string? CustomerPhone { get; set; }

    /// <summary>
    /// 顧客電子郵件
    /// </summary>
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public PaymentMethods? PaymentMethod { get; set; }

    /// <summary>
    /// 發票類型
    /// </summary>
    public InvoiceType? InvoiceType { get; set; }

    /// <summary>
    /// 發票號碼
    /// </summary>
    public string? InvoiceNumber { get; set; }

    /// <summary>
    /// 載具識別碼
    /// </summary>
    public string? CarrierId { get; set; }

    /// <summary>
    /// 統一編號
    /// </summary>
    public string? UniformNumber { get; set; }

    /// <summary>
    /// 發票抬頭
    /// </summary>
    public string? TaxTitle { get; set; }

    /// <summary>
    /// 收件人與購買人相同
    /// </summary>
    public bool IsAsSameBuyer { get; set; }

    /// <summary>
    /// 收件人姓名
    /// </summary>
    public string? RecipientName { get; set; }

    /// <summary>
    /// 收件人電話
    /// </summary>
    public string? RecipientPhone { get; set; }

    /// <summary>
    /// 收件人電子郵件
    /// </summary>
    public string? RecipientEmail { get; set; }

    /// <summary>
    /// 配送方式
    /// </summary>
    public DeliveryMethod? DeliveryMethod { get; set; }

    /// <summary>
    /// 運送編號
    /// </summary>
    public string? ShippingNumber { get; set; }

    /// <summary>
    /// 城市
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// 區域
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// 道路/街道
    /// </summary>
    public string? Road { get; set; }

    /// <summary>
    /// 詳細地址
    /// </summary>
    public string? AddressDetails { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// 收貨時間
    /// </summary>
    public ReceivingTime? ReceivingTime { get; set; }

    /// <summary>
    /// 團購識別碼
    /// </summary>
    public Guid GroupBuyId { get; set; }

    /// <summary>
    /// 拆分來源識別碼
    /// </summary>
    public Guid? SplitFromId { get; set; }

    /// <summary>
    /// 商品詳情
    /// </summary>
    [NotMapped]
    public string ItemDetail { get; set; }

    /// <summary>
    /// 團購
    /// </summary>
    [ForeignKey(nameof(GroupBuyId))]
    public GroupBuy GroupBuy { get; set; }

    /// <summary>
    /// 總數量
    /// </summary>
    public int TotalQuantity { get; set; }

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 檢查碼
    /// </summary>
    public string? CheckMacValue { get; set; }

    /// <summary>
    /// 付款日期
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// 運送狀態
    /// </summary>
    public ShippingStatus ShippingStatus { get; set; }

    /// <summary>
    /// 出貨日期
    /// </summary>
    public DateTime? ShippingDate { get; set; }

    /// <summary>
    /// 取消日期
    /// </summary>
    public DateTime? CancellationDate { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// 備貨人員
    /// </summary>
    public string PrepareShipmentBy { get; set; }

    /// <summary>
    /// 出貨人員
    /// </summary>
    public string ShippedBy { get; set; }

    /// <summary>
    /// 換貨人員
    /// </summary>
    public string? ExchangeBy { get; set; }

    /// <summary>
    /// 關閉訂單人員
    /// </summary>
    public string ClosedBy { get; set; }

    /// <summary>
    /// 完成訂單人員
    /// </summary>
    public string CompletedBy { get; set; }

    /// <summary>
    /// 完成時間
    /// </summary>
    public DateTime? CompletionTime { get; set; }

    /// <summary>
    /// 換貨時間
    /// </summary>
    public DateTime? ExchangeTime { get; set; }

    /// <summary>
    /// 退貨狀態
    /// </summary>
    public OrderReturnStatus? ReturnStatus { get; set; }

    /// <summary>
    /// 訂單商品列表
    /// </summary>
    public ICollection<OrderItem> OrderItems { get; set; }

    /// <summary>
    /// 商店評論列表
    /// </summary>
    public ICollection<StoreComment> StoreComments { get; set; }

    /// <summary>
    /// 訂單發票列表
    /// </summary>
    public ICollection<OrderInvoice>? OrderInvoices { get; set; }

    /// <summary>
    /// 是否已退款
    /// </summary>
    public bool IsRefunded { get; set; }

    /// <summary>
    /// 是否作廢發票
    /// </summary>
    public bool IsVoidInvoice { get; set; }

    /// <summary>
    /// 作廢日期
    /// </summary>
    public DateTime? VoidDate { get; set; }

    /// <summary>
    /// 作廢原因
    /// </summary>
    public string? VoidReason { get; set; }

    /// <summary>
    /// 折讓單原因
    /// </summary>
    public string? CreditNoteReason { get; set; }

    /// <summary>
    /// 發票日期
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// 開立發票狀態
    /// </summary>
    public IssueInvoiceStatus? IssueStatus { get; set; }

    /// <summary>
    /// 發票狀態
    /// </summary>
    public InvoiceStatus InvoiceStatus { get; set; }

    /// <summary>
    /// 作廢使用者
    /// </summary>
    public string? VoidUser { get; set; }

    /// <summary>
    /// 折讓單日期
    /// </summary>
    public DateTime? CreditNoteDate { get; set; }

    /// <summary>
    /// 折讓單使用者
    /// </summary>
    public string? CreditNoteUser { get; set; }

    /// <summary>
    /// 訂單類型
    /// </summary>
    public OrderType? OrderType { get; set; }

    /// <summary>
    /// 商店識別碼
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 商店地址
    /// </summary>
    public string? StoreAddress { get; set; }

    /// <summary>
    /// 便利商店名稱
    /// </summary>
    public string? CVSStoreOutSide { get; set; }

    /// <summary>
    /// 交易編號
    /// </summary>
    public string? TradeNo { get; set; }

    /// <summary>
    /// 商家交易編號
    /// </summary>
    public string? MerchantTradeNo { get; set; }

    public decimal? EcPayNetAmount { get; set; }
    /// <summary>
    /// 常溫配送費用
    /// </summary>
    public decimal? DeliveryCostForNormal { get; set; }

    /// <summary>
    /// 冷藏配送費用
    /// </summary>
    public decimal? DeliveryCostForFreeze { get; set; }

    /// <summary>
    /// 冷凍配送費用
    /// </summary>
    public decimal? DeliveryCostForFrozen { get; set; }

    /// <summary>
    /// 配送費用
    /// </summary>
    public decimal? DeliveryCost { get; set; }

    /// <summary>
    /// 團購序號
    /// </summary>
    public int? GWSR { get; set; }

    /// <summary>
    /// 訂單退款類型
    /// </summary>
    public OrderRefundType? OrderRefundType { get; set; }

    /// <summary>
    /// 購物金抵扣金額
    /// </summary>
    public int CreditDeductionAmount { get; set; }

    /// <summary>
    /// 購物金抵扣記錄識別碼
    /// </summary>
    public Guid? CreditDeductionRecordId { get; set; }

    /// <summary>
    /// 購物金抵扣記錄
    /// </summary>
    [ForeignKey(nameof(CreditDeductionRecordId))]
    public UserShoppingCredit? CreditDeductionRecord { get; set; }

    /// <summary>
    /// 退款金額
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// 購物金回饋金額
    /// </summary>
    public decimal cashback_amount { get; set; }

    /// <summary>
    /// 購物金回饋記錄識別碼
    /// </summary>
    public Guid? cashback_record_id { get; set; }

    /// <summary>
    /// 退款記錄
    /// </summary>
    [ForeignKey(nameof(cashback_record_id))]
    public UserShoppingCredit? RefundRecord { get; set; }

    /// <summary>
    /// 使用者識別碼
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 折扣碼識別碼
    /// </summary>
    public Guid? DiscountCodeId { get; set; }

    /// <summary>
    /// 折扣金額
    /// </summary>
    public int? DiscountAmount { get; set; }

    /// <summary>
    /// 使用者
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }

    /// <summary>
    /// 折扣碼
    /// </summary>
    [ForeignKey(nameof(DiscountCodeId))]
    public DiscountCode? DiscountCode { get; set; }

    /// <summary>
    /// 常溫收件人姓名
    /// </summary>
    public string? RecipientNameDbsNormal { get; set; }

    /// <summary>
    /// 冷藏收件人姓名
    /// </summary>
    public string? RecipientNameDbsFreeze { get; set; }

    /// <summary>
    /// 冷凍收件人姓名
    /// </summary>
    public string? RecipientNameDbsFrozen { get; set; }

    /// <summary>
    /// 常溫收件人電話
    /// </summary>
    public string? RecipientPhoneDbsNormal { get; set; }

    /// <summary>
    /// 冷藏收件人電話
    /// </summary>
    public string? RecipientPhoneDbsFreeze { get; set; }

    /// <summary>
    /// 冷凍收件人電話
    /// </summary>
    public string? RecipientPhoneDbsFrozen { get; set; }

    /// <summary>
    /// 常溫郵遞區號
    /// </summary>
    public string? PostalCodeDbsNormal { get; set; }

    /// <summary>
    /// 冷藏郵遞區號
    /// </summary>
    public string? PostalCodeDbsFreeze { get; set; }

    /// <summary>
    /// 冷凍郵遞區號
    /// </summary>
    public string? PostalCodeDbsFrozen { get; set; }

    /// <summary>
    /// 常溫城市
    /// </summary>
    public string? CityDbsNormal { get; set; }

    /// <summary>
    /// 冷藏城市
    /// </summary>
    public string? CityDbsFreeze { get; set; }

    /// <summary>
    /// 冷凍城市
    /// </summary>
    public string? CityDbsFrozen { get; set; }

    /// <summary>
    /// 常溫詳細地址
    /// </summary>
    public string? AddressDetailsDbsNormal { get; set; }

    /// <summary>
    /// 冷藏詳細地址
    /// </summary>
    public string? AddressDetailsDbsFreeze { get; set; }

    /// <summary>
    /// 冷凍詳細地址
    /// </summary>
    public string? AddressDetailsDbsFrozen { get; set; }

    /// <summary>
    /// 常溫備註
    /// </summary>
    public string? RemarksDbsNormal { get; set; }

    /// <summary>
    /// 冷藏備註
    /// </summary>
    public string? RemarksDbsFreeze { get; set; }

    /// <summary>
    /// 冷凍備註
    /// </summary>
    public string? RemarksDbsFrozen { get; set; }

    /// <summary>
    /// 常溫商店識別碼
    /// </summary>
    public string? StoreIdNormal { get; set; }

    /// <summary>
    /// 冷藏商店識別碼
    /// </summary>
    public string? StoreIdFreeze { get; set; }

    /// <summary>
    /// 冷凍商店識別碼
    /// </summary>
    public string? StoreIdFrozen { get; set; }

    /// <summary>
    /// 常溫便利商店外部名稱
    /// </summary>
    public string? CVSStoreOutSideNormal { get; set; }

    /// <summary>
    /// 冷藏便利商店外部名稱
    /// </summary>
    public string? CVSStoreOutSideFreeze { get; set; }

    /// <summary>
    /// 冷凍便利商店外部名稱
    /// </summary>
    public string? CVSStoreOutSideFrozen { get; set; }

    /// <summary>
    /// 常溫收貨時間
    /// </summary>
    public ReceivingTime? ReceivingTimeNormal { get; set; }

    /// <summary>
    /// 冷藏收貨時間
    /// </summary>
    public ReceivingTime? ReceivingTimeFreeze { get; set; }

    /// <summary>
    /// 冷凍收貨時間
    /// </summary>
    public ReceivingTime? ReceivingTimeFrozen { get; set; }

    /// <summary>
    /// 綠界物流狀態
    /// </summary>
    public string? EcpayLogisticsStatus { get; set; }

    /// <summary>
    /// 綠界物流回傳碼
    /// </summary>
    public int EcpayLogisticRtnCode { get; set; }

    /// <summary>
    /// 已退貨訂單項目識別碼
    /// </summary>
    public string? ReturnedOrderItemIds { get; set; }

    /// <summary>
    /// 行版本 (並行處理權杖)
    /// </summary>
    public byte[] RowVersion { get; set; } // Concurrency token

    public ShippingStatus? ShippingStatusBeforeReturn { get; set; }
    /// <summary>
    /// 訂單交易列表
    /// </summary>
    public ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

    public ICollection<AppliedCampaign> AppliedCampaigns { get; set; } = [];

    public virtual ManualBankTransferRecord? ManualBankTransferRecord { get; set; }

    public Order() { }

    public Order(
        [NotNull] Guid id,
        [NotNull] Guid groupBuyId,
        string orderNo,
        bool isIndividual,
        string? customerName,
        string? customerPhone,
        string? customerEmail,
        PaymentMethods? paymentMethods,
        InvoiceType? invoiceType,
        string? invoiceNumber,
        string? uniformNumber,
        string? carrierId,
        string? taxTitle,
        bool isAsSameBuyer,
        string? recipientName,
        string? recipientPhone,
        string? recipientEmail,
        DeliveryMethod? deliveryMethod,
        string? postalCode,
        string? city,
        string? district,
        string? road,
        string? addressDetails,
        string? remarks,
        ReceivingTime? receivingTime,
        int totalQuantity,
        decimal totalAmount,
        OrderReturnStatus? orderReturnStatus,
        OrderType? orderType,
        Guid? splitFromId = null,
        Guid? userId = null,
        int creditDeductionAmount = 0,
        Guid? creditDeductionRecordId = null,
        decimal creditRefundAmount = 0,
        Guid? creditRefundRecordId = null,
        Guid? discountCodeId = null,
        int? discountCodeAmount = null,
        string? recipientNameDbsNormal = null,
        string? recipientNameDbsFreeze = null,
        string? recipientNameDbsFrozen = null,
        string? recipientPhoneDbsNormal = null,
        string? recipientPhoneDbsFreeze = null,
        string? recipientPhoneDbsFrozen = null,
        string? postalCodeDbsNormal = null,
        string? postalCodeDbsFreeze = null,
        string? postalCodeDbsFrozen = null,
        string? cityDbsNormal = null,
        string? cityDbsFreeze = null,
        string? cityDbsFrozen = null,
        string? addressDetailsDbsNormal = null,
        string? addressDetailsDbsFreeze = null,
        string? addressDetailsDbsFrozen = null,
        string? remarksDbsNormal = null,
        string? remarksDbsFreeze = null,
        string? remarksDbsFrozen = null,
        string? storeIdNormal = null,
        string? storeIdFreeze = null,
        string? storeIdFrozen = null,
        string? cVSStoreOutSideNormal = null,
        string? cVSStoreOutSideFreeze = null,
        string? cVSStoreOutSideFrozen = null,
        ReceivingTime? receivingTimeNormal = null,
        ReceivingTime? receivingTimeFreeze = null,
        ReceivingTime? receivingTimeFrozen = null
    )
    {
        Id = id;
        GroupBuyId = groupBuyId;
        OrderNo = orderNo;
        IsIndividual = isIndividual;
        CustomerName = customerName;
        CustomerPhone = customerPhone;
        CustomerEmail = customerEmail;
        PaymentMethod = paymentMethods;
        InvoiceType = invoiceType;
        InvoiceNumber = invoiceNumber;
        UniformNumber = uniformNumber;
        CarrierId = carrierId;
        TaxTitle = taxTitle;
        IsAsSameBuyer = isAsSameBuyer;
        RecipientName = recipientName;
        RecipientEmail = recipientEmail;
        RecipientPhone = recipientPhone;
        DeliveryMethod = deliveryMethod;
        City = city;
        District = district;
        Road = road;
        AddressDetails = addressDetails;
        Remarks = remarks;
        ReceivingTime = receivingTime;
        TotalQuantity = totalQuantity;
        TotalAmount = totalAmount;
        OrderStatus = OrderStatus.Open;
        ShippingStatus = ShippingStatus.WaitingForPayment;
        OrderItems = new List<OrderItem>();
        StoreComments = new List<StoreComment>();
        IsRefunded = false;
        ReturnStatus = orderReturnStatus;
        OrderType = orderType;
        SplitFromId = splitFromId;
        PostalCode = postalCode;
        ClosedBy = "";
        CompletedBy = "";
        PrepareShipmentBy = "";
        ShippedBy = "";
        UserId = userId;
        CreditDeductionAmount = creditDeductionAmount;
        CreditDeductionRecordId = creditDeductionRecordId;
        cashback_amount = creditRefundAmount;
        cashback_record_id = creditRefundRecordId;
        DiscountAmount = discountCodeAmount;
        DiscountCodeId = discountCodeId;
        RecipientNameDbsNormal = recipientNameDbsNormal;
        RecipientNameDbsFreeze = recipientNameDbsFreeze;
        RecipientNameDbsFrozen = recipientNameDbsFrozen;
        RecipientPhoneDbsNormal = recipientPhoneDbsNormal;
        RecipientPhoneDbsFreeze = recipientPhoneDbsFreeze;
        RecipientPhoneDbsFrozen = recipientPhoneDbsFrozen;
        PostalCodeDbsNormal = postalCodeDbsNormal;
        PostalCodeDbsFreeze = postalCodeDbsFreeze;
        PostalCodeDbsFrozen = postalCodeDbsFrozen;
        CityDbsNormal = cityDbsNormal;
        CityDbsFreeze = cityDbsFreeze;
        CityDbsFrozen = cityDbsFrozen;
        AddressDetailsDbsNormal = addressDetailsDbsNormal;
        AddressDetailsDbsFreeze = addressDetailsDbsFreeze;
        AddressDetailsDbsFrozen = addressDetailsDbsFrozen;
        RemarksDbsNormal = remarksDbsNormal;
        RemarksDbsFreeze = remarksDbsFreeze;
        RemarksDbsFrozen = remarksDbsFrozen;
        StoreIdNormal = storeIdNormal;
        StoreIdFreeze = storeIdFreeze;
        StoreIdFrozen = storeIdFrozen;
        CVSStoreOutSideNormal = cVSStoreOutSideNormal;
        CVSStoreOutSideFreeze = cVSStoreOutSideFreeze;
        CVSStoreOutSideFrozen = cVSStoreOutSideFrozen;
        ReceivingTimeNormal = receivingTimeNormal;
        ReceivingTimeFreeze = receivingTimeFreeze;
        ReceivingTimeFrozen = receivingTimeFrozen;
    }

    /// <summary>
    /// 新增訂單項目
    /// </summary>
    public OrderItem AddOrderItem(
        Guid id,
        Guid? itemId,
        Guid? itemDetailId,
        Guid? setItemId,
        Guid? freebieId,
        ItemType itemType,
        string? spec,
        decimal itemPrice,
        decimal totalAmount,
        int quantity,
        string? sku,
        ItemStorageTemperature temperature,
        decimal temperatureCost,
        bool isAddOnProduct = false
        )
    {
        var orderItem = new OrderItem(
                id,
                itemId,
                itemDetailId,
                setItemId,
                freebieId,
                itemType,
                Id,
                spec,
                itemPrice,
                totalAmount,
                quantity,
                sku,
                temperature,
                temperatureCost,
                isAddOnProduct: isAddOnProduct
                );
        OrderItems.Add(orderItem);
        return orderItem;
    }

    /// <summary>
    /// 更新訂單項目
    /// </summary>
    public void UpdateOrderItem(
      List<OrderItem> items, Guid DeliveryOrderId
      )
    {
        foreach (OrderItem item in items)
        {
            item.DeliveryOrderId = DeliveryOrderId;
        }
    }

    /// <summary>
    /// 新增商店評論
    /// </summary>
    internal void AddStoreComment(
        [NotNull] string comment
        )
    {
        StoreComments.AddIfNotContains(new StoreComment(comment));
    }

    public AppliedCampaign AddAppliedCampaign(Guid campaignId, PromotionModule module, int amount)
    {
        var appliedCampaign = new AppliedCampaign(Guid.NewGuid(), Id, campaignId, module, amount);
        return AddAppliedCampaign(appliedCampaign);
    }

    public AppliedCampaign AddAppliedCampaign(AppliedCampaign appliedCampaign)
    {
        AppliedCampaigns ??= [];
        AppliedCampaigns.Add(appliedCampaign);
        return appliedCampaign;
    }
}
