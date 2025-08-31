using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.CodTradeInfos;

public class TCatCodTradeInfoRecord : Entity<Guid>, IHasCreationTime, ISoftDelete
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
    public decimal FeeRate { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal NetAmount { get; set; }
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
}
