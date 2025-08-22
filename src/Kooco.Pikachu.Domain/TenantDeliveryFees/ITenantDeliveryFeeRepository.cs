using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantDeliveryFees
{
    public interface ITenantDeliveryFeeRepository : IRepository<TenantDeliveryFee, Guid>
    {
        // Legacy helper (keep if you’re already using it)
        Task<List<TenantDeliveryFee>> GetByTenantIdAsync(
            Guid tenantId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Filtered + paged list. Use string sorting like "CreationTime DESC", "DeliveryProvider ASC", etc.
        /// </summary>
        Task<List<TenantDeliveryFee>> GetListAsync(
            Guid? tenantId = null,

            DeliveryProvider? deliveryProvider = null,
            bool? isEnabled = null,
            FeeKind? feeKind = null,
            string? sorting = null,      // fallback to "CreationTime DESC" in impl
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Total count for the same filters (for paging UIs).
        /// </summary>
        Task<long> GetCountAsync(
            Guid? tenantId = null,

            DeliveryProvider? deliveryProvider = null,
            bool? isEnabled = null,
            FeeKind? feeKind = null,
            CancellationToken cancellationToken = default
        );
        Task<(List<TenantLogisticsFeeOverviewItem> Items, long TotalCount)> GetTenantLogisticsFeeOverviewAsync(
       string? tenantNameFilter = null,         // optional: filter by tenant name (Contains)
       string? sorting = null,                  // e.g. "Name ASC" (defaults to Name ASC)
       int skipCount = 0,
       int maxResultCount = int.MaxValue,
       CancellationToken cancellationToken = default
   );
    }

}
