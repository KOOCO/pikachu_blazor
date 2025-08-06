using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticsFeeManagements
{
    public interface ILogisticsFeeFileImportRepository : IRepository<LogisticsFeeFileImport, Guid>
    {
        Task<List<LogisticsFeeFileImport>> GetListAsync(
            int skipCount,
            int maxResultCount,
            string sorting = null,
            string filter = null,
            LogisticsFileType? fileType = null,
            FileProcessingStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filter = null,
            LogisticsFileType? fileType = null,
            FileProcessingStatus? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default
        );

        Task<LogisticsFeeFileImport> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
