using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;

public interface IWebsiteBasicSettingAppService : IApplicationService
{
    Task<WebsiteBasicSettingDto> FirstOrDefaultAsync();
    Task<WebsiteBasicSettingDto> UpdateAsync(UpdateWebsiteBasicSettingDto input);
    Task<WebsiteBasicSettingDto> SetIsEnabledAsync(bool isEnabled);
}
