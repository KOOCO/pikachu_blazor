using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.TenantPaymentFees;

public class TenantPaymentFee : FullAuditedEntity<Guid>
{
    public PaymentFeeType FeeType { get; private set; }
    public PaymentFeeSubType FeeSubType { get; private set; }
    public PaymentMethods PaymentMethod { get; private set; }
    public bool IsEnabled { get; set; }
    public FeeKind FeeKind { get; set; }
    public double Amount { get; private set; }
    public bool IsBaseFee { get; private set; }
    public Guid TenantId { get; set; }

    private TenantPaymentFee() { }

    internal TenantPaymentFee(
        Guid id,
        PaymentFeeType feeType,
        PaymentFeeSubType feeSubType,
        PaymentMethods paymentMethod,
        bool isEnabled,
        FeeKind feeKind,
        double amount,
        bool isBaseFee,
        Guid tenantId
        ) : base(id)
    {
        SetFeeType(feeType, feeSubType, paymentMethod);
        SetIsEnabled(isEnabled);
        FeeKind = feeKind;
        SetAmount(amount);
        IsBaseFee = isBaseFee;
        TenantId = tenantId;
    }

    public void SetFeeType(PaymentFeeType feeType, PaymentFeeSubType feeSubType, PaymentMethods paymentMethod)
    {
        if (!TenantPaymentFeeMap.IsCombinationAllowed(feeType, feeSubType, paymentMethod))
        {
            throw new UserFriendlyException($"Combination of {feeType}, {feeSubType}, and {paymentMethod} is not allowed", "Invalid Combination");
        }

        FeeType = feeType;
        FeeSubType = feeSubType;
        PaymentMethod = paymentMethod;
    }

    public void SetAmount(double amount)
    {
        Amount = Check.Range(amount, nameof(Amount), 0);
    }

    public void SetIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}