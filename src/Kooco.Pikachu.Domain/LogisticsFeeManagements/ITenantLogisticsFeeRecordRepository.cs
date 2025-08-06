using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public interface ITenantLogisticsFeeRecordRepository : IRepository<TenantLogisticsFeeRecord, Guid>
    {
        Task<List<TenantLogisticsFeeRecord>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string filter = null,
            Guid? tenantId = null,
            Guid? fileImportId = null,
            LogisticsFileType? fileType = null,
            WalletDeductionStatus? status = null,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filter = null,
            Guid? tenantId = null,
            Guid? fileImportId = null,
            LogisticsFileType? fileType = null,
            WalletDeductionStatus? status = null,
            CancellationToken cancellationToken = default
        );

        Task<List<TenantLogisticsFeeRecord>> GetByIdsAsync(
            List<Guid> ids,
            CancellationToken cancellationToken = default
        );

        Task<List<TenantLogisticsFeeRecord>> GetFailedRecordsAsync(
            Guid? fileImportId = null,
            Guid? tenantId = null,
            CancellationToken cancellationToken = default
        );
    }
}
