using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
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
public class WebsiteSettingsAppService(IWebsiteSettingsRepository websiteSettingsRepository, WebsiteSettingsManager websiteSettingsManager,
    IImageAppService imageAppService) : PikachuAppService, IWebsiteSettingsAppService
{
    [Authorize(PikachuPermissions.WebsiteManagement.WebsiteSettings.Create)]
    public async Task<WebsiteSettingsDto> CreateAsync(CreateWebsiteSettingsDto input)
    {
        Check.NotNull(input, nameof(input));
        MyCheck.NotUndefinedOrNull<WebsitePageType>(input.PageType, nameof(input.PageType));

        var websiteSettings = await websiteSettingsManager.CreateAsync(input.PageTitle, input.PageDescription, input.PageLink, input.SetAsHomePage,
            input.PageType.Value, input.TemplateType, input.GroupBuyModuleType, input.ProductCategoryId, input.ArticleHtml);

        if (input.GroupBuyModuleType == GroupBuyModuleType.ProductGroupModule)
        {
            if (input.Modules == null || input.Modules.Count == 0)
            {
                throw new UserFriendlyException("The field Page Type Module is required.");
            }

            foreach (var module in input.Modules)
            {
                var newModule = websiteSettings.AddModule(GuidGenerator.Create(), module.SortOrder, module.GroupBuyModuleType, module.AdditionalInfo,
                    module.ProductGroupModuleTitle, module.ProductGroupModuleImageSize, module.ModuleNumber);

                foreach (var moduleItem in module.ModuleItems ?? [])
                {
                    newModule.AddModuleItem(GuidGenerator.Create(), moduleItem.ItemId, moduleItem.SetItemId, moduleItem.ItemType, moduleItem.SortOrder,
                        moduleItem.DisplayText, moduleItem.ModuleNumber);
                }
            }

            foreach (var om in input.OverviewModules ?? [])
            {
                websiteSettings.AddOverviewModule(GuidGenerator.Create(), om.Title, om.Image, om.SubTitle, om.BodyText,
                    om.IsButtonEnable, om.ButtonText, om.ButtonLink);
            }

            foreach (var im in input.InstructionModules ?? [])
            {
                websiteSettings.AddInstructionModule(GuidGenerator.Create(), im.Title, im.Image, im.BodyText);
            }

            foreach (var prm in input.ProductRankingModules ?? [])
            {
                websiteSettings.AddProductRankingModule(GuidGenerator.Create(), prm.Title, prm.SubTitle, prm.Content, prm.ModuleNumber);
                foreach (var image in prm.Images)
                {
                    image.TargetId = websiteSettings.Id;
                    await imageAppService.CreateAsync(image);
                }
            }
        }

        return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(websiteSettings);
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
            input.Sorting = nameof(WebsiteSettings.PageTitle);
        }

        var totalCount = await websiteSettingsRepository.GetCountAsync(input.Filter, input.PageTitle, input.PageType,
            input.ProductCategoryId, input.TemplateType, input.SetAsHomePage);

        var items = await websiteSettingsRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter,
            input.PageTitle, input.PageType, input.ProductCategoryId, input.TemplateType, input.SetAsHomePage);

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
        MyCheck.NotUndefinedOrNull<WebsitePageType>(input.PageType, nameof(input.PageType));

        var websiteSettings = await websiteSettingsRepository.GetAsync(id);

        await websiteSettingsManager.UpdateAsync(websiteSettings, input.PageTitle, input.PageDescription, input.PageLink, input.SetAsHomePage,
            input.PageType.Value, input.TemplateType, input.GroupBuyModuleType, input.ProductCategoryId, input.ArticleHtml);

        return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(websiteSettings);
    }
}
