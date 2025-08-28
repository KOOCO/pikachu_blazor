using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.CodTradeInfos;

public class EcPayCodTradeInfoDto : EntityDto<Guid>, IMultiTenant
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
    public decimal? ActualWeight { get; set; }
    public string AllPayLogisticsID { get; set; } = default!;
    public string BookingNote { get; set; } = default!;
    public decimal CollectionAllocateAmount { get; set; }
    public DateTime? CollectionAllocateDate { get; set; }
    public decimal CollectionAmount { get; set; }
    public decimal CollectionChargeFee { get; set; }
    public string? CVSPaymentNo { get; set; }
    public string? CVSValidationNo { get; set; }
    public decimal GoodsAmount { get; set; }
    public string? GoodsName { get; set; }
    public decimal? GoodsWeight { get; set; }
    public decimal HandlingCharge { get; set; }
    public string LogisticsStatus { get; set; } = default!;
    public string LogisticsType { get; set; } = default!;
    public string MerchantID { get; set; } = default!;
    public string MerchantTradeNo { get; set; } = default!;
    public string? SenderCellPhone { get; set; }
    public string SenderName { get; set; } = default!;
    public string SenderPhone { get; set; } = default!;
    public DateTime? ShipChargeDate { get; set; }
    public string? ShipmentNo { get; set; }
    public DateTime? TradeDate { get; set; }
    public string CheckMacValue { get; set; } = default!;
}
