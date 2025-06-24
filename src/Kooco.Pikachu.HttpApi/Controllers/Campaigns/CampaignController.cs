using Asp.Versioning;
using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.Campaigns;

[RemoteService]
[ControllerName("Campaings")]
[Area("app")]
[Route("api/app/campaigns")]
public class CampaignController(ICampaignAppService campaignAppService) : PikachuController, ICampaignAppService
{
    [HttpPost]
    public Task<CampaignDto> CreateAsync(CreateCampaignDto input)
    {
        return campaignAppService.CreateAsync(input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return campaignAppService.DeleteAsync(id);
    }

    [HttpGet("active-count")]
    public Task<long> GetActiveCampaignsCountAsync()
    {
        return campaignAppService.GetActiveCampaignsCountAsync();
    }

    [HttpGet("{id}")]
    public Task<CampaignDto> GetAsync(Guid id, bool withDetails = false)
    {
        return campaignAppService.GetAsync(id, withDetails);
    }

    [HttpGet("list")]
    public Task<PagedResultDto<CampaignDto>> GetListAsync(GetCampaignListDto input)
    {
        return campaignAppService.GetListAsync(input);
    }

    [HttpGet("lookup")]
    public Task<List<KeyValueDto>> GetLookupAsync()
    {
        return campaignAppService.GetLookupAsync();
    }

    [HttpPost("enabled/{id}/{isEnabled}")]
    public Task SetIsEnabledAsync(Guid id, bool isEnabled)
    {
        return campaignAppService.SetIsEnabledAsync(id, isEnabled);
    }

    [HttpPut("{id}")]
    public Task<CampaignDto> UpdateAsync(Guid id, CreateCampaignDto input)
    {
        return campaignAppService.UpdateAsync(id, input);
    }

    [HttpGet("lookup-with-module")]
    public Task<List<CampaignLookupWithModuleDto>> GetCampaignLookupWithModuleAsync()
    {
        return campaignAppService.GetCampaignLookupWithModuleAsync();
    }
}
