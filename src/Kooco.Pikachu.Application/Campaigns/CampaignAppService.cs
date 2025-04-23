using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
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
        await Validate(input);

        var builder = new CampaignBuilder(input);

        var campaign = await builder.BuildAsync(_campaignManager);

        await _campaignRepository.UpdateAsync(campaign);

        return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
    }

    private async Task Validate(CreateCampaignDto input)
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
    }

    public async Task<CampaignDto> GetAsync(Guid id)
    {
        var campaign = await _campaignRepository.GetAsync(id);
        return ObjectMapper.Map<Campaign, CampaignDto>(campaign);
    }
}
