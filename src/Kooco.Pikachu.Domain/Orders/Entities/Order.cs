using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.StoreComments;
using Kooco.Pikachu.UserShoppingCredits;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Orders.Entities;

public class Order : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [NotMapped]
    public Guid OrderId { get; set; }
    public Guid? TenantId { get; set; }
    public string OrderNo { get; set; }
    public bool IsIndividual { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? CarrierId { get; set; }
    public string? UniformNumber { get; set; }
    public string? TaxTitle { get; set; }
    public bool IsAsSameBuyer { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientEmail { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public string? ShippingNumber { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? District { get; set; }
    public string? Road { get; set; }
    public string? AddressDetails { get; set; }
    public string? Remarks { get; set; }
    public ReceivingTime? ReceivingTime { get; set; }
    public Guid GroupBuyId { get; set; }
    public Guid? SplitFromId { get; set; }
    [NotMapped]
    public string ItemDetail { get; set; }
    [ForeignKey(nameof(GroupBuyId))]
    public GroupBuy GroupBuy { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CheckMacValue { get; set; }
    public DateTime? PaymentDate { get; set; }
    public ShippingStatus ShippingStatus { get; set; }
    public DateTime? ShippingDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string PrepareShipmentBy { get; set; }
    public string ShippedBy { get; set; }
    public string? ExchangeBy { get; set; }
    public string ClosedBy { get; set; }
    public string CompletedBy { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? ExchangeTime { get; set; }
    public OrderReturnStatus? ReturnStatus { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<StoreComment> StoreComments { get; set; }
    public ICollection<OrderInvoice>? OrderInvoices { get; set; }
    public bool IsRefunded { get; set; }
    public bool IsVoidInvoice { get; set; }
    public DateTime? VoidDate { get; set; }
    public string? VoidReason { get; set; }
    public string? CreditNoteReason { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public IssueInvoiceStatus? IssueStatus { get; set; }
    public InvoiceStatus InvoiceStatus { get; set; }
    public string? VoidUser { get; set; }
    public DateTime? CreditNoteDate { get; set; }
    public string? CreditNoteUser { get; set; }
    public OrderType? OrderType { get; set; }
    public string? StoreId { get; set; }
    public string? StoreAddress { get; set; }

    /// <summary>
    /// Convenience Store Name
    /// </summary>
    public string? CVSStoreOutSide { get; set; }
    public string? TradeNo { get; set; }
    public string? MerchantTradeNo { get; set; }
    public decimal? DeliveryCostForNormal { get; set; }
    public decimal? DeliveryCostForFreeze { get; set; }
    public decimal? DeliveryCostForFrozen { get; set; }
    public decimal? DeliveryCost { get; set; }
    public int? GWSR { get; set; }
    public OrderRefundType? OrderRefundType { get; set; }

    public int CreditDeductionAmount { get; set; }

    public Guid? CreditDeductionRecordId { get; set; }

    [ForeignKey(nameof(CreditDeductionRecordId))]
    public UserShoppingCredit? CreditDeductionRecord { get; set; }

    public decimal RefundAmount { get; set; }

    public decimal cashback_amount { get; set; }

    public Guid? cashback_record_id { get; set; }

    [ForeignKey(nameof(cashback_record_id))]
    public UserShoppingCredit? RefundRecord { get; set; }

    public Guid? UserId { get; set; }
    public Guid? DiscountCodeId { get; set; }
    public int? DiscountAmount { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }
    [ForeignKey(nameof(DiscountCodeId))]
    public DiscountCode? DiscountCode { get; set; }

    public string? RecipientNameDbsNormal { get; set; }
    public string? RecipientNameDbsFreeze { get; set; }
    public string? RecipientNameDbsFrozen { get; set; }
    public string? RecipientPhoneDbsNormal { get; set; }
    public string? RecipientPhoneDbsFreeze { get; set; }
    public string? RecipientPhoneDbsFrozen { get; set; }
    public string? PostalCodeDbsNormal { get; set; }
    public string? PostalCodeDbsFreeze { get; set; }
    public string? PostalCodeDbsFrozen { get; set; }
    public string? CityDbsNormal { get; set; }
    public string? CityDbsFreeze { get; set; }
    public string? CityDbsFrozen { get; set; }
    public string? AddressDetailsDbsNormal { get; set; }
    public string? AddressDetailsDbsFreeze { get; set; }
    public string? AddressDetailsDbsFrozen { get; set; }
    public string? RemarksDbsNormal { get; set; }
    public string? RemarksDbsFreeze { get; set; }
    public string? RemarksDbsFrozen { get; set; }
    public string? StoreIdNormal { get; set; }
    public string? StoreIdFreeze { get; set; }
    public string? StoreIdFrozen { get; set; }
    public string? CVSStoreOutSideNormal { get; set; }
    public string? CVSStoreOutSideFreeze { get; set; }
    public string? CVSStoreOutSideFrozen { get; set; }
    public ReceivingTime? ReceivingTimeNormal { get; set; }
    public ReceivingTime? ReceivingTimeFreeze { get; set; }
    public ReceivingTime? ReceivingTimeFrozen { get; set; }
    public string? EcpayLogisticsStatus { get; set; }
    public int EcpayLogisticRtnCode { get; set; }
    public string? ReturnedOrderItemIds { get; set; }
    public byte[] RowVersion { get; set; } // Concurrency token

    public ICollection<OrderTransaction> OrderTransactions { get; set; } = new List<OrderTransaction>();

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

    public void AddOrderItem(
        Guid id,
        Guid? itemId,
        Guid? setItemId,
        Guid? freebieId,
        ItemType itemType,
        string? spec,
        decimal itemPrice,
        decimal totalAmount,
        int quantity,
        string? sku,
        ItemStorageTemperature temperature,
        decimal temperatureCost
        )
    {
        OrderItems.Add(new OrderItem(
                id,
                itemId,
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
                temperatureCost
                ));
    }
    public void UpdateOrderItem(
      List<OrderItem> items, Guid DeliveryOrderId


      )
    {
        foreach (OrderItem item in items)
        {

            item.DeliveryOrderId = DeliveryOrderId;


        }
    }
    internal void AddStoreComment(
        [NotNull] string comment
        )
    {
        StoreComments.AddIfNotContains(new StoreComment(comment));
    }
}
