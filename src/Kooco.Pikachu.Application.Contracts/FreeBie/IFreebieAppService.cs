using Kooco.Pikachu.Freebie.Dtos;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Freebie
{
    public interface IFreebieAppService : IApplicationService
    {
        Task<FreebieDto> CreateAsync(FreebieCreateDto input);
        
    }
}
