using FluentValidation;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Kooco.Pikachu.Campaigns;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.Campaigns.Default)]
public class CampaignAppService : PikachuAppService, ICampaignAppService
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly CampaignManager _campaignManager;
    private readonly IValidator<CreateCampaignDto> _createCampaignValidator;

    public CampaignAppService(
        ICampaignRepository campaignRepository,
        CampaignManager campaignManager,
        IValidator<CreateCampaignDto> createCampaignValidator
        )
    {
        _campaignRepository = campaignRepository;
        _campaignManager = campaignManager;
        _createCampaignValidator = createCampaignValidator;
    }

    [Authorize(PikachuPermissions.Campaigns.Create)]
    public async Task<CampaignDto> CreateAsync(CreateCampaignDto input)
    {
        Check.NotNull(input, nameof(input));

        var validations = await _createCampaignValidator.ValidateAsync(input);
        if (validations.Errors.Count > 0)
        {
            var errors = validations.Errors
                .Select(val => new ValidationResult(val.ErrorMessage, [val.PropertyName]))
                .ToList();
            throw new AbpValidationException(errors);
        }

        var builder = new CampaignBuilder(input);

        var campaign = await builder.CreateAsync(_campaignManager);

        await _campaignRepository.UpdateAsync(campaign);

        return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
    }

    [Authorize(PikachuPermissions.Campaigns.Edit)]
    public async Task<CampaignDto> UpdateAsync(Guid id, CreateCampaignDto input)
    {
        Check.NotNull(input, nameof(input));

        var validations = await _createCampaignValidator.ValidateAsync(input);
        if (validations.Errors.Count > 0)
        {
            var errors = validations.Errors
                .Select(val => new ValidationResult(val.ErrorMessage, [val.PropertyName]))
                .ToList();
            throw new AbpValidationException(errors);
        }

        var campaign = await _campaignRepository.GetWithDetailsAsync(id);

        var builder = new CampaignBuilder(input);

        await builder.UpdateAsync(campaign, _campaignManager);

        await _campaignRepository.UpdateAsync(campaign);

        return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
    }

    public async Task<CampaignDto> GetAsync(Guid id, bool withDetails = false)
    {
        var campaign = withDetails
            ? await _campaignRepository.GetWithDetailsAsync(id)
            : await _campaignRepository.GetAsync(id);

        return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
    }

    public async Task<PagedResultDto<CampaignDto>> GetListAsync(GetCampaignListDto input)
    {
        var totalCount = await _campaignRepository.CountAsync(input.Filter, input.IsEnabled, input.StartDate, input.EndDate);
        var items = await _campaignRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting,
            input.Filter, input.IsEnabled, input.StartDate, input.EndDate);

        return new PagedResultDto<CampaignDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Campaign>, List<CampaignDto>>(items)
        };
    }

    [Authorize(PikachuPermissions.Campaigns.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var campaign = await _campaignRepository.GetWithDetailsAsync(id);
        await _campaignManager.RemoveModulesAsync(campaign);
        await _campaignRepository.DeleteAsync(campaign);
    }

    [Authorize(PikachuPermissions.Campaigns.Edit)]
    public async Task SetIsEnabledAsync(Guid id, bool isEnabled)
    {
        var campaign = await _campaignRepository.GetAsync(id);
        campaign.SetIsEnabled(isEnabled);
        await _campaignRepository.UpdateAsync(campaign);
    }

    public async Task<long> GetActiveCampaignsCountAsync()
    {
        return await _campaignRepository.GetActiveCampaignsCountAsync();
    }

    public async Task<List<KeyValueDto>> GetLookupAsync()
    {
        return [.. (await _campaignRepository.GetQueryableAsync())
            .Select(q => new KeyValueDto
            {
                Id = q.Id,
                Name = q.Name
            })];
    }
}
