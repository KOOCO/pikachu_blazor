using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Reconciliations;

public interface IEcPayReconciliationAppService : IApplicationService
{
    Task<List<EcPayReconciliationRecordDto>> QueryMediaFileAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<EcPayReconciliationRecordDto>> GetListAsync(EcPayReconciliationRecordListInput input, CancellationToken cancellationToken = default);
}
