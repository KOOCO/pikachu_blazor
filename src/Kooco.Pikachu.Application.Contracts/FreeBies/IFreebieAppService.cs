using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebies
{
    public interface IFreebieAppService : IApplicationService
    {
        Task<FreebieDto> CreateAsync(FreebieCreateDto input);
        Task<List<KeyValueDto>> GetGroupBuyLookupAsync();
        Task<List<FreebieDto>> GetListAsync();

    }
}
