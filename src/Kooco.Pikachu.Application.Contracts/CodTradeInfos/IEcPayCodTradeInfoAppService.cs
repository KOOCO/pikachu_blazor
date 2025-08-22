using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.CodTradeInfos;

public interface IEcPayCodTradeInfoAppService : IApplicationService
{
    Task<List<EcPayCodTradeInfoDto>> QueryTradeInfoAsync(CancellationToken cancellationToken = default);
}
