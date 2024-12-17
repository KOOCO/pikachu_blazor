using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.WebsiteManagement.WebsiteBasicSettings)]
public class WebsiteBasicSettingAppService(IRepository<WebsiteBasicSetting, Guid> websiteBasicSettingRepository,
    WebsiteBasicSettingManager websiteBasicSettingManager) : PikachuAppService, IWebsiteBasicSettingAppService
{
    public async Task<WebsiteBasicSettingDto> FirstOrDefaultAsync()
    {
        var websiteBasicSettings = await websiteBasicSettingRepository.FirstOrDefaultAsync();
        return ObjectMapper.Map<WebsiteBasicSetting, WebsiteBasicSettingDto>(websiteBasicSettings);
    }

    public async Task<WebsiteBasicSettingDto> UpdateAsync(UpdateWebsiteBasicSettingDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNull(input.TitleDisplayOption, nameof(input.TitleDisplayOption));
        Check.NotNull(input.TemplateType, nameof(input.TemplateType));
        Check.NotNull(input.ColorScheme, nameof(input.ColorScheme));

        var websiteBasicSettings = await websiteBasicSettingManager.UpdateAsync(input.IsEnabled, input.TitleDisplayOption.Value, input.StoreTitle, input.Description,
            input.LogoName, input.LogoUrl, input.TemplateType.Value, input.ColorScheme.Value, input.PrimaryColor, input.SecondaryColor, input.BackgroundColor,
            input.SecondaryColor, input.AlertColor);

        return ObjectMapper.Map<WebsiteBasicSetting, WebsiteBasicSettingDto>(websiteBasicSettings);
    }

    public async Task<WebsiteBasicSettingDto> SetIsEnabledAsync(bool isEnabled)
    {
        var websiteBasicSettings = await websiteBasicSettingManager.SetIsEnabledAsync(isEnabled);
        return ObjectMapper.Map<WebsiteBasicSetting, WebsiteBasicSettingDto>(websiteBasicSettings);
    }
}
