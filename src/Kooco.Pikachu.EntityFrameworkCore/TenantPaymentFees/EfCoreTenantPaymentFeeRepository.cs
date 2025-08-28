using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.TenantPaymentFees
{
    public class EfCoreTenantPaymentFeeRepository : EfCoreRepository<PikachuDbContext, TenantPaymentFee, Guid>, ITenantPaymentFeeRepository
    {
        public EfCoreTenantPaymentFeeRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<TenantPaymentFee?> FindAsync(
            Guid tenantId, 
            PaymentFeeType feeType, 
            PaymentFeeSubType feeSubType, 
            PaymentMethods paymentMethod
            )
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            }

            var queryable = (await GetQueryableAsync()).AsNoTracking();

            return queryable
                .Where(t => t.TenantId == tenantId && t.FeeType == feeType
                && t.FeeSubType == feeSubType && t.PaymentMethod == paymentMethod)
                .FirstOrDefault();
        }
    }
}
