using Asp.Versioning;
using Kooco.Pikachu.WebsiteManagement;
using Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.WebsiteManagement;

[RemoteService(IsEnabled = true)]
[ControllerName("WebsiteSettings")]
[Area("app")]
[Route("api/app/website-settings")]
public class WebsiteSettingsController(IWebsiteSettingsAppService websiteSettingsAppService) : PikachuController, IWebsiteSettingsAppService
{
    [HttpPost]
    public Task<WebsiteSettingsDto> CreateAsync(CreateWebsiteSettingsDto input)
    {
        return websiteSettingsAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return websiteSettingsAppService.DeleteAsync(id);
    }

    [HttpDelete("instructions-module/{id}")]
    public Task DeleteInstructionsModuleAsync(Guid id)
    {
        return websiteSettingsAppService.DeleteInstructionsModuleAsync(id);
    }

    [HttpDelete("module/{id}/{moduleId}")]
    public Task DeleteModuleAsync(Guid id, Guid moduleId)
    {
        return websiteSettingsAppService.DeleteModuleAsync(id, moduleId);
    }

    [HttpDelete("overview-module/{id}")]
    public Task DeleteOverviewModuleAsync(Guid id)
    {
        return websiteSettingsAppService.DeleteOverviewModuleAsync(id);
    }

    [HttpDelete("product-ranking-module/{id}")]
    public Task DeleteProductRankingModuleAsync(Guid id)
    {
        return websiteSettingsAppService.DeleteProductRankingModuleAsync(id);
    }

    [HttpGet("{id}")]
    public Task<WebsiteSettingsDto> GetAsync(Guid id, bool includeDetails = false)
    {
        return websiteSettingsAppService.GetAsync(id, includeDetails);
    }

    [HttpGet]
    public Task<PagedResultDto<WebsiteSettingsDto>> GetListAsync(GetWebsiteSettingsListDto input)
    {
        return websiteSettingsAppService.GetListAsync(input);
    }

    [HttpGet("module/{moduleId}")]
    public Task<WebsiteSettingsModuleDto> GetModuleAsync(Guid moduleId)
    {
        return websiteSettingsAppService.GetModuleAsync(moduleId);
    }

    [HttpGet("modules/{id}")]
    public Task<List<WebsiteSettingsModuleDto>> GetModulesAsync(Guid id)
    {
        return websiteSettingsAppService.GetModulesAsync(id);
    }

    [HttpPut("{id}")]
    public Task<WebsiteSettingsDto> UpdateAsync(Guid id, UpdateWebsiteSettingsDto input)
    {
        return websiteSettingsAppService.UpdateAsync(id, input);
    }
}
