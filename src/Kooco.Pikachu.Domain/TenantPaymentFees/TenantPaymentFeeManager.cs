using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.TenantPaymentFees;

public class TenantPaymentFeeManager : DomainService
{
    private readonly ITenantPaymentFeeRepository _tenantPaymentFeeRepository;
    public TenantPaymentFeeManager(ITenantPaymentFeeRepository tenantPaymentFeeRepository)
    {
        _tenantPaymentFeeRepository = tenantPaymentFeeRepository;
    }

    public async Task<TenantPaymentFee> CreateAsync(
        PaymentFeeType feeType,
        PaymentFeeSubType feeSubType,
        PaymentMethods paymentMethod,
        bool isEnabled,
        FeeKind feeKind,
        decimal amount,
        bool isBaseFee,
        Guid tenantId
        )
    {
        Check.NotDefaultOrNull<Guid>(tenantId, nameof(tenantId));

        var existing = await _tenantPaymentFeeRepository
            .FindAsync(
                tenantId,
                feeType,
                feeSubType,
                paymentMethod
            );

        if (existing != null)
        {
            throw new UserFriendlyException("Duplicate record with same combination");
        }

        var tenantPaymentFee = new TenantPaymentFee(
            GuidGenerator.Create(),
            feeType,
            feeSubType,
            paymentMethod,
            isEnabled,
            feeKind,
            amount,
            isBaseFee,
            tenantId
            );

        await _tenantPaymentFeeRepository.InsertAsync(tenantPaymentFee);

        return tenantPaymentFee;
    }

    public async Task<TenantPaymentFee> UpdateAsync(
        TenantPaymentFee tenantPaymentFee,
        bool isEnabled,
        FeeKind feeKind,
        decimal amount
        )
    {
        Check.NotNull(tenantPaymentFee, nameof(TenantPaymentFee));

        tenantPaymentFee.SetIsEnabled(isEnabled);
        tenantPaymentFee.FeeKind = feeKind;
        tenantPaymentFee.SetAmount(amount);

        await _tenantPaymentFeeRepository.UpdateAsync(tenantPaymentFee);

        return tenantPaymentFee;
    }
}
