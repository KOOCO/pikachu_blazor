using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.WebsiteManagement;

public interface IWebsiteSettingsAppService : IApplicationService
{
    Task<WebsiteSettingsDto> CreateAsync(CreateWebsiteSettingsDto input);
    Task<WebsiteSettingsDto> UpdateAsync(Guid id, UpdateWebsiteSettingsDto input);
    Task<WebsiteSettingsDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<PagedResultDto<WebsiteSettingsDto>> GetListAsync(GetWebsiteSettingsListDto input);
}
