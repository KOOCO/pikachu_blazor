using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantPaymentFees;
using System;

namespace Kooco.Pikachu.TenantPayouts;

public class TenantPayoutRecordDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNo { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public FeeKind? FeeKind { get; set; }
    public decimal GrossOrderAmount { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal FeeRate { get; set; }
    public decimal ProcessingFee { get; set; }
    public decimal NetAmount { get; set; }
    public PaymentFeeType FeeType { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
}
