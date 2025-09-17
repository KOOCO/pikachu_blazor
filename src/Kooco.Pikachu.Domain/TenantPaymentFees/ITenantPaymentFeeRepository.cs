using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantPaymentFees;

public interface ITenantPaymentFeeRepository : IRepository<TenantPaymentFee, Guid>
{
    Task<TenantPaymentFee?> FindAsync(
        Guid tenantId,
        PaymentFeeType feeType,
        PaymentFeeSubType feeSubType,
        PaymentMethods paymentMethod,
        bool isBaseFee
        );
}
