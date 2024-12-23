using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Validators;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

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

        if (input.PageType == WebsitePageType.CustomPage)
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

    public async Task<WebsiteSettingsDto> GetAsync(Guid id, bool includeDetails = false)
    {
        var websiteSettings = await (includeDetails
            ? websiteSettingsRepository.GetWithDetailsAsync(id)
            : websiteSettingsRepository.GetAsync(id));
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

        var websiteSettings = await websiteSettingsRepository.GetWithDetailsAsync(id);

        await websiteSettingsManager.UpdateAsync(websiteSettings, input.PageTitle, input.PageDescription, input.PageLink, input.SetAsHomePage,
            input.PageType.Value, input.TemplateType, input.GroupBuyModuleType, input.ProductCategoryId, input.ArticleHtml);

        if (input.PageType == WebsitePageType.CustomPage)
        {
            foreach (var module in input.Modules)
            {
                if (module.Id == Guid.Empty)
                {
                    var newModule = websiteSettings.AddModule(GuidGenerator.Create(), module.SortOrder, module.GroupBuyModuleType, module.AdditionalInfo,
                    module.ProductGroupModuleTitle, module.ProductGroupModuleImageSize, module.ModuleNumber);

                    foreach (var moduleItem in module.ModuleItems ?? [])
                    {
                        newModule.AddModuleItem(GuidGenerator.Create(), moduleItem.ItemId, moduleItem.SetItemId, moduleItem.ItemType, moduleItem.SortOrder,
                            moduleItem.DisplayText, moduleItem.ModuleNumber);
                    }
                }
                else
                {
                    var existingModule = websiteSettings.Modules.First(x => x.Id == module.Id);
                    existingModule.SortOrder = module.SortOrder;
                    existingModule.GroupBuyModuleType = module.GroupBuyModuleType;
                    existingModule.AdditionalInfo = module.AdditionalInfo;
                    existingModule.ProductGroupModuleTitle = module.ProductGroupModuleTitle;
                    existingModule.ProductGroupModuleImageSize = module.ProductGroupModuleImageSize;
                    existingModule.ModuleNumber = module.ModuleNumber;

                    existingModule.ModuleItems.Clear();

                    foreach (var moduleItem in module.ModuleItems ?? [])
                    {
                        existingModule.AddModuleItem(GuidGenerator.Create(), moduleItem.ItemId, moduleItem.SetItemId, moduleItem.ItemType, moduleItem.SortOrder,
                            moduleItem.DisplayText, moduleItem.ModuleNumber);
                    }
                }
            }

            foreach (var om in input.OverviewModules ?? [])
            {
                if (om.Id == Guid.Empty)
                {
                    websiteSettings.AddOverviewModule(GuidGenerator.Create(), om.Title, om.Image, om.SubTitle, om.BodyText,
                        om.IsButtonEnable, om.ButtonText, om.ButtonLink);
                }
                else
                {
                    var overview = websiteSettings.OverviewModules.First(x => x.Id == om.Id);
                    overview.Title = om.Title;
                    overview.Image = om.Image;
                    overview.SubTitle = om.SubTitle;
                    overview.BodyText = om.BodyText;
                    overview.IsButtonEnable = om.IsButtonEnable;
                    overview.ButtonText = om.ButtonText;
                    overview.ButtonLink = om.ButtonLink;
                }
            }

            foreach (var im in input.InstructionModules ?? [])
            {
                if (im.Id == Guid.Empty)
                {
                    websiteSettings.AddInstructionModule(GuidGenerator.Create(), im.Title, im.Image, im.BodyText);
                }
                else
                {
                    var instruction = websiteSettings.InstructionModules.First(x => x.Id == im.Id);
                    instruction.Title = im.Title;
                    instruction.Image = im.Image;
                    instruction.BodyText = im.BodyText;
                }
            }

            foreach (var prm in input.ProductRankingModules ?? [])
            {
                if (prm.Id == Guid.Empty)
                {
                    websiteSettings.AddProductRankingModule(GuidGenerator.Create(), prm.Title, prm.SubTitle, prm.Content, prm.ModuleNumber);
                    foreach (var image in prm.Images)
                    {
                        image.TargetId = websiteSettings.Id;
                        await imageAppService.CreateAsync(image);
                    }
                }
                else
                {
                    var product = websiteSettings.ProductRankingModules.First(x => x.Id == prm.Id);
                    product.Title = prm.Title;
                    product.SubTitle = prm.SubTitle;
                    product.Content = prm.Content;
                    product.ModuleNumber = prm.ModuleNumber;
                }
            }
        }
        else
        {
            websiteSettings.Modules.Clear();
            websiteSettings.OverviewModules.Clear();
            websiteSettings.InstructionModules.Clear();
            websiteSettings.ProductRankingModules.Clear();
        }

        return ObjectMapper.Map<WebsiteSettings, WebsiteSettingsDto>(websiteSettings);
    }

    public async Task<List<WebsiteSettingsModuleDto>> GetModulesAsync(Guid id)
    {
        var modules = await websiteSettingsRepository.GetModulesAsync(id);
        return ObjectMapper.Map<List<WebsiteSettingsModule>, List<WebsiteSettingsModuleDto>>(modules);
    }

    public async Task<WebsiteSettingsModuleDto> GetModuleAsync(Guid moduleId)
    {
        var itemGroup = await websiteSettingsRepository.GetModuleAsync(moduleId);
        return ObjectMapper.Map<WebsiteSettingsModule, WebsiteSettingsModuleDto>(itemGroup);
    }

    public async Task DeleteModuleAsync(Guid id, Guid moduleId)
    {
        var websiteSettings = await websiteSettingsRepository.GetWithDetailsAsync(id);

        var module = websiteSettings.Modules.FirstOrDefault(m => m.Id == moduleId)
            ?? throw new EntityNotFoundException(typeof(WebsiteSettingsModule), moduleId);

        websiteSettings.Modules.Remove(module);

        await websiteSettingsRepository.UpdateAsync(websiteSettings);
    }

    public async Task DeleteOverviewModuleAsync(Guid id)
    {
        var websiteSettings = await websiteSettingsRepository.GetWithDetailsAsync(id);

        websiteSettings.OverviewModules.Clear();

        await websiteSettingsRepository.UpdateAsync(websiteSettings);
    }

    public async Task DeleteInstructionsModuleAsync(Guid id)
    {
        var websiteSettings = await websiteSettingsRepository.GetWithDetailsAsync(id);

        websiteSettings.InstructionModules.Clear();

        await websiteSettingsRepository.UpdateAsync(websiteSettings);
    }

    public async Task DeleteProductRankingModuleAsync(Guid id)
    {
        var websiteSettings = await websiteSettingsRepository.GetWithDetailsAsync(id);

        websiteSettings.ProductRankingModules.Clear();

        await websiteSettingsRepository.UpdateAsync(websiteSettings);
    }
}
