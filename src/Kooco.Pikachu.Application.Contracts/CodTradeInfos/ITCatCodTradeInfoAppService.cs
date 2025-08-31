using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.CodTradeInfos;

public interface ITCatCodTradeInfoAppService : IApplicationService
{
    Task ImportAsync(List<TCatCodTradeInfoRecordDto> input);
    Task<List<TCatCodTradeInfoRecordDto>> ProcessFile(string fileName, byte[] fileBytes);
}
