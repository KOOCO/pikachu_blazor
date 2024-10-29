using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.StoreComments;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders;

public class OrderDto : FullAuditedEntityDto<Guid>
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public bool IsIndividual { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? UniformNumber { get; set; }
    public string? TaxTitle { get; set; }
    public bool IsAsSameBuyer { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientEmail { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public string? ShippingNumber { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? Road { get; set; }
    public string? PostalCode { get; set; }
    public string? AddressDetails { get; set; }
    public string? Remarks { get; set; }
    public ReceivingTime? ReceivingTime { get; set; }
    public Guid GroupBuyId { get; set; }
    public GroupBuyDto GroupBuy { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CheckMacValue { get; set; }
    public DateTime? PaymentDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    public List<StoreCommentDto> StoreComments { get; set; }
    public bool IsSelected { get; set; } = false;
    public ShippingStatus ShippingStatus { get; set; }
    public DateTime? ShippingDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public bool IsRefunded { get; set; }
    public OrderReturnStatus ReturnStatus { get; set; }
    public InvoiceStatus InvoiceStatus { get; set; }
    public OrderType? OrderType { get; set; }
    public string PrepareShipmentBy { get; set; }
    public string ShippedBy { get; set; }
    public string ExchangeBy { get; set; }
    public string ClosedBy { get; set; }
    public string CompletedBy { get; set; }
    public string? VoidReason { get; set; }
    public string? CreditNoteReason { get; set; }
    public bool IsVoidInvoice { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public DateTime? CreditNoteDate { get; set; }
    public string? CreditNoteUser { get; set; }
    public string? VoidUser { get; set; }
    public string? CarrierId { get; set; }
    public DateTime? VoidDate { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? ExchangeTime { get; set; }
    public IssueInvoiceStatus? IssueStatus { get; set; }
    public string? StoreId { get; set; }
    public string? CVSStoreOutSide { get; set; }
    public string MerchantTradeNo { get; set; }
    public decimal? DeliveryCostForNormal { get; set; }
    public decimal? DeliveryCostForFreeze { get; set; }
    public decimal? DeliveryCostForFrozen { get; set; }
    public decimal? DeliveryCost { get; set; }
    public string? TradeNo { get; set; }
    public int? GWSR { get; set; }
    public OrderRefundType? OrderRefundType { get; set; }
    public decimal RefundAmount { get; set; }
    public Guid? UserId { get; set; }
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
    public ItemStorageTemperature? TemperatureControlDbs { get; set; }
}
