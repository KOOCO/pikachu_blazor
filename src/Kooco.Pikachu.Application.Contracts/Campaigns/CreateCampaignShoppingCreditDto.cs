using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.Campaigns;

public class CreateCampaignShoppingCreditDto
{
    public bool? CanExpire { get; set; }
    public int? ValidForDays { get; set; }
    public CalculationMethod? CalculationMethod { get; set; }
    public double? CalculationPercentage { get; set; }
    public List<CreateCampaignStageSettingDto> StageSettings { get; set; } = [new()];
    public ApplicableItem? ApplicableItem { get; set; }
    public int? Budget { get; set; }

    public int StageSettingsCount { get { return StageSettings?.Count ?? 0; } }
    public bool IsAnyStageSettingSelected { get { return StageSettings?.Any(ss => ss.IsSelected) ?? false; } }
    public void AddStageSetting() => StageSettings?.Add(new());
    public void RemoveSelectedStageSettings() => StageSettings?.RemoveAll(ss => ss.IsSelected);
}