using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public interface ITenantLogisticsFeeFileProcessingSummaryRepository : IRepository<TenantLogisticsFeeFileProcessingSummary, Guid>
    {
        Task<List<TenantLogisticsFeeFileProcessingSummary>> GetByFileImportIdAsync(
            Guid fileImportId,
            CancellationToken cancellationToken = default
        );

        Task<List<TenantLogisticsFeeFileProcessingSummary>> GetByTenantIdAsync(
            Guid tenantId,
            int skipCount,
            int maxResultCount,
            CancellationToken cancellationToken = default
        );
    }
}

