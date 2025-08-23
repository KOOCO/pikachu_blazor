using Kooco.Pikachu.Orders.Entities;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Reconciliations;

public class EcPayReconciliationRecord : Entity<Guid>, IHasCreationTime, ISoftDelete, IMultiTenant
{
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }

    [ForeignKey(nameof(OrderId))]
    public virtual Order? Order { get; set; }

    // API Response Fields
    public DateTime OrderDate { get; set; }
    public string MerchantTradeNo { get; set; } = string.Empty;
    public string EcPayTradeNo { get; set; } = string.Empty;
    public string? StoreCode { get; set; }
    public string? MID { get; set; }
    public string? PlatformName { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public string? CreditCardAuthCode { get; set; }
    public string? CreditCardLast4 { get; set; }
    public string? ConvenienceStoreInfo { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TransactionAmount { get; set; }
    public string? FeeRate { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal ProcessingFee { get; set; }
    public decimal TransactionHandlingFee { get; set; }
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
}
