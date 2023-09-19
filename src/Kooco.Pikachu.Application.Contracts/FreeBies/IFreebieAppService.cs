using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebies
{
    public interface IFreebieAppService : IApplicationService
    {
        Task<FreebieDto> CreateAsync(FreebieCreateDto input);
        Task<List<KeyValueDto>> GetGroupBuyLookupAsync();
    }
}
