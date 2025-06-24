using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Campaigns;

public interface ICampaignAppService : IApplicationService
{
    Task<CampaignDto> CreateAsync(CreateCampaignDto input);
    Task<CampaignDto> UpdateAsync(Guid id, CreateCampaignDto input);
    Task<CampaignDto> GetAsync(Guid id, bool withDetails = false);
    Task<PagedResultDto<CampaignDto>> GetListAsync(GetCampaignListDto input);
    Task DeleteAsync(Guid id);
    Task SetIsEnabledAsync(Guid id, bool isEnabled);
    Task<long> GetActiveCampaignsCountAsync();
    Task<List<KeyValueDto>> GetLookupAsync();
    Task<List<CampaignLookupWithModuleDto>> GetCampaignLookupWithModuleAsync();
}
