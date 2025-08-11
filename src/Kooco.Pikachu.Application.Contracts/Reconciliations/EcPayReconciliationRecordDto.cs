using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Reconciliations;

public class EcPayReconciliationRecordDto : CreationAuditedEntityDto<Guid>, IMultiTenant
{
    public DateTime OrderDate { get; set; }
    public string MerchantTradeNo { get; set; } = string.Empty;
    public string EcPayTradeNo { get; set; } = string.Empty;
    public string? StoreCode { get; set; }
    public string? MID { get; set; }
    public string? PlatformName { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string? FeeRate { get; set; }
    public string? CreditCardAuthCode { get; set; }
    public string? CreditCardLast4 { get; set; }
    public string? ConvenienceStoreInfo { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TransactionAmount { get; set; }
    public string? RefundDate { get; set; }
    public decimal? RefundAmount { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal PlatformFee { get; set; }
    public decimal NetAmount { get; set; }
    public string PayoutStatus { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? MerchantRemarks { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PayerName { get; set; }
    public string? PayerPhone { get; set; }
    public string? PayerEmail { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? ReceiverAddress { get; set; }
    public string? ReceiverEmail { get; set; }
    public string? UnifiedBusinessNumber { get; set; }
    public decimal ProcessingFee { get; set; }
    public Guid? TenantId { get; set; }
}
