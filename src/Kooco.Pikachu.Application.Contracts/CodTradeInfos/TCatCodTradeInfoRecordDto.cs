using Kooco.Pikachu.TenantPayouts;
using System;

namespace Kooco.Pikachu.CodTradeInfos;

public class TCatCodTradeInfoRecordDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
    public decimal FeeRate { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal NetAmount { get; set; }

    // Import file fields   
    public string CustomerID { get; set; }
    public DateTime? CollectionDate { get; set; }
    public string? CollectionSite { get; set; }
    public DateTime? DeliveryCompletionDate { get; set; }
    public string? DeliveryCompletionSite { get; set; }
    public string MerchantTradeNo { get; set; }
    public string? ShippingNo { get; set; }
    public decimal? ShippingFee { get; set; }
    public decimal? ExtraShippingFee { get; set; }
    public string? ExtraServiceItems { get; set; }
    public string? CashCollected { get; set; }
    public string? ReturnedGoods { get; set; }
    public string? SameDayDelivery { get; set; }
    public string? ShipmentType { get; set; }
    public decimal? CODAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public TenantPayoutRecordDto PayoutRecord { get; set; }
}
