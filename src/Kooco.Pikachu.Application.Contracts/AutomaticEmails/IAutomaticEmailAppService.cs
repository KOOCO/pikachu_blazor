using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.AutomaticEmails
{
    public interface IAutomaticEmailAppService : IApplicationService
    {
        Task CreateAsync(AutomaticEmailCreateUpdateDto input);
        Task UpdateAsync(Guid id, AutomaticEmailCreateUpdateDto input);
        Task<AutomaticEmailDto> GetAsync(Guid id);
        Task<PagedResultDto<AutomaticEmailDto>> GetListAsync(GetAutomaticEmailListDto input);
    }
}
