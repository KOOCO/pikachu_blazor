﻿using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
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

namespace Kooco.Pikachu.Orders;

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

    public int RefundAmount { get; set; }

    public Guid? RefundRecordId { get; set; }

    [ForeignKey(nameof(RefundRecordId))]
    public UserShoppingCredit? RefundRecord { get; set; }

    public Guid? UserId { get; set; }
    public Guid? DiscountCodeId { get; set; }
    public int? DiscountAmount { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public IdentityUser? User { get; set; }
    [ForeignKey(nameof(DiscountCodeId))]
    public DiscountCode? DiscountCode { get; set; }
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
        string? carrierId,
        string? uniformNumber,
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
        int creditRefundAmount=0,
         Guid? creditRefundRecordId = null,
        Guid? discountCodeId=null,
        int? discountCodeAmount=null
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
        RefundAmount = creditRefundAmount;
        RefundRecordId = creditRefundRecordId;
        DiscountAmount = discountCodeAmount;
        DiscountCodeId = discountCodeId;
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
                this.Id,
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
