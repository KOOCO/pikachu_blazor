using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.TenantPaymentFees;

public class TenantPaymentFeeDto
{
    public Guid Id { get; set; }
    public PaymentFeeType FeeType { get; set; }
    public PaymentFeeSubType FeeSubType { get; set; }
    public PaymentMethods PaymentMethod { get; set; }
    public bool IsEnabled { get; set; }
    public FeeKind FeeKind { get; set; }
    public double Amount { get; set; }
    public bool IsBaseFee { get; set; }
    public Guid TenantId { get; set; }
}
