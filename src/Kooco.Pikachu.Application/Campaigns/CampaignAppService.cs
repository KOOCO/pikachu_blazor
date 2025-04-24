using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;

namespace Kooco.Pikachu.Campaigns;

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

        var campaign = await builder.BuildAsync(_campaignManager);

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
}
