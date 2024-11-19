using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Orders;

public class CreateUpdateOrderDto
{
    public Guid Id { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid GroupBuyId { get; set; }
    public bool IsIndividual { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }
    public PaymentMethods? PaymentMethod { get; set; }
    public InvoiceType? InvoiceType { get; set; }
    public string? UniformNumber { get; set; }
    public string? CarrierId { get; set; }
    public string? TaxTitle { get; set; }
    public bool IsAsSameBuyer { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }
    public string? RecipientEmail { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? AddressDetails { get; set; }
    public string? Remarks { get; set; }
    public ReceivingTime? ReceivingTime { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderReturnStatus ReturnStatus { get; set; }
    public OrderType? OrderType { get; set; }
    public Guid? UserId { get; set; }
    public int CreditDeductionAmount { get; set; }
    public Guid? CreditDeductionRecordId { get; set; }
    public Guid? DiscountCodeId { get; set; }
    public int? DiscountCodeAmount { get; set; }
    public decimal cashback_amount { get; set; }
    public Guid? cashback_record_id { get; set; }
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
    public string? StoreId { get; set; }
    public string? CVSStoreOutSide { get; set; }
    public ShippingStatus ShippingStatus { get; set; }
    public decimal? DeliveryCostForNormal { get; set; }
    public decimal? DeliveryCostForFreeze { get; set; }
    public decimal? DeliveryCostForFrozen { get; set; }
    public ReceivingTime? ReceivingTimeNormal { get; set; }
    public ReceivingTime? ReceivingTimeFreeze { get; set; }
    public ReceivingTime? ReceivingTimeFrozen { get; set; }
    public decimal? DeliveryCost { get; set; }
    public List<CreateUpdateOrderItemDto> OrderItems { get; set; }
}
