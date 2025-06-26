using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignShoppingCreditDto : EntityDto<Guid>
{
    public Guid CampaignId { get; set; }
    public bool CanExpire { get; set; }
    public int? ValidForDays { get; set; }
    public CalculationMethod CalculationMethod { get; set; }
    public double? CalculationPercentage { get; set; }
    public double? CapAmount { get; private set; }
    public bool ApplicableToAddOnProducts { get; set; }
    public bool ApplicableToShippingFees { get; set; }
    public int Budget { get; set; }
    public CampaignSpendCondition SpendCondition { get; set; }
    public int? Threshold { get; set; }
    public ICollection<CampaignStageSettingDto> StageSettings { get; set; }
}
