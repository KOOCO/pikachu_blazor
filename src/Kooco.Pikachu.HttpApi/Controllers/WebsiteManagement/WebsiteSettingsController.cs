﻿using Asp.Versioning;
using Kooco.Pikachu.WebsiteManagement;
using Microsoft.AspNetCore.Mvc;
using System;
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

    [HttpGet("{id}")]
    public Task<WebsiteSettingsDto> GetAsync(Guid id)
    {
        return websiteSettingsAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<WebsiteSettingsDto>> GetListAsync(GetWebsiteSettingsListDto input)
    {
        return websiteSettingsAppService.GetListAsync(input);
    }

    [HttpPut("{id}")]
    public Task<WebsiteSettingsDto> UpdateAsync(Guid id, UpdateWebsiteSettingsDto input)
    {
        return websiteSettingsAppService.UpdateAsync(id, input);
    }
}
