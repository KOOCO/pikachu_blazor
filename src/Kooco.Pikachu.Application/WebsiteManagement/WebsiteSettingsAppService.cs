using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Validators;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.WebsiteManagement.WebsiteSettings.Default)]
public class WebsiteSettingsAppService(IWebsiteSettingsRepository websiteSettingsRepository, WebsiteSettingsManager websiteSettingsManager) : PikachuAppService, IWebsiteSettingsAppService
{
    [Authorize(PikachuPermissions.WebsiteManagement.WebsiteSettings.Create)]
    public async Task<WebsiteSettingsDto> CreateAsync(CreateWebsiteSettingsDto input)
    {
        Check.NotNull(input, nameof(input));
        MyCheck.NotDefaultOrNull<WebsitePageType>(input.PageType, nameof(input.PageType));
        MyCheck.NotDefaultOrNull<GroupBuyTemplateType>(input.TemplateType, nameof(input.TemplateType));
        
        //var websiteSettings = await websiteSettingsManager.CreateAsync(input.PageTitle, input.LogoName, input.LogoUrl, input.PageDescription,
        //    input.TitleDisplayOption!.Value, input.Facebook, input.Instagram, input.Line, input.ReturnExchangePolicy);

        //return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(websiteSettings);
        return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(new WebsiteSettings());
    }

    [Authorize(PikachuPermissions.WebsiteManagement.WebsiteSettings.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var websiteSettings = await websiteSettingsRepository.GetAsync(id);
        await websiteSettingsRepository.DeleteAsync(websiteSettings);
    }

    public async Task<WebsiteSettingsDto> GetAsync(Guid id)
    {
        var websiteSettings = await websiteSettingsRepository.GetAsync(id);
        return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(websiteSettings);
    }

    public async Task<PagedResultDto<WebsiteSettingsDto>> GetListAsync(GetWebsiteSettingsListDto input)
    {
        if (input.Sorting.IsNullOrWhiteSpace())
        {
            input.Sorting = nameof(WebsiteSettings.StoreTitle);
        }

        var totalCount = await websiteSettingsRepository.GetCountAsync(input.Filter);

        var items = await websiteSettingsRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

        return new PagedResultDto<WebsiteSettingsDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<WebsiteSettings>, List<WebsiteSettingsDto>>(items)
        };
    }

    [Authorize(PikachuPermissions.WebsiteManagement.WebsiteSettings.Edit)]
    public async Task<WebsiteSettingsDto> UpdateAsync(Guid id, UpdateWebsiteSettingsDto input)
    {
        Check.NotNull(input, nameof(input));
        if (!Enum.IsDefined(typeof(WebsiteTitleDisplayOptions), input.TitleDisplayOption))
        {
            throw new InvalidEnumValueException(nameof(input.TitleDisplayOption));
        }

        var websiteSettings = await websiteSettingsRepository.GetAsync(id);

        await websiteSettingsManager.UpdateAsync(websiteSettings, input.NotificationBar, input.LogoName, input.LogoUrl, input.StoreTitle,
            input.TitleDisplayOption!.Value, input.Facebook, input.Instagram, input.Line, input.ReturnExchangePolicy);

        return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(websiteSettings);
    }
}
