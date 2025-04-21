using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kooco.Pikachu.Campaigns;

public class CreateCampaignDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    [Required]
    public DateTime? EndDate { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public IEnumerable<string> TargetAudience { get; set; } = [];

    [Required]
    public PromotionModule? PromotionModule { get; set; }

    public bool? IsDiscountCodeRequired { get; set; }
    public string? DiscountCode { get; set; }
    public int AvailableQuantity { get; set; }
    public int MaximumUsePerPerson { get; set; }

    public bool? ApplyToAllGroupBuys { get; set; }
    public IEnumerable<Guid> GroupBuyIds { get; set; } = [];

    public bool? ApplyToAllProducts { get; set; }
    public IEnumerable<Guid> ProductIds { get; set; } = [];

    public DiscountMethod? DiscountMethod { get; set; }
    public int MinimumSpendAmount { get; set; }
    public bool? ApplyToAllShippingMethods { get; set; }
    public IEnumerable<DeliveryMethod> DeliveryMethods { get; set; } = [];

    public DiscountType? DiscountType { get; set; }
    public int? DiscountAmount { get; set; }
    public int? DiscountPercentage { get; set; }

    public bool? CanExpire { get; set; }
    public int? ValidForDays { get; set; }
    public CalculationMethod? CalculationMethod { get; set; }
    public double? CalculationPercentage { get; set; }
    public List<StageSettingsDto> StageSettings { get; set; } = [new()];
    public int StageSettingsCount { get { return StageSettings?.Count ?? 0; } }
    public bool IsAnyStageSettingSelected { get { return StageSettings?.Any(ss => ss.IsSelected) ?? false; } }
    public ApplicableItem? ApplicableItem { get; set; }
    public int? Budget { get; set; }


    public void AddStageSetting()
    {
        StageSettings ??= [];
        StageSettings.Add(new());
    }

    public void RemoveSelectedStageSettings()
    {
        StageSettings.RemoveAll(ss => ss.IsSelected);
    }
}

public class StageSettingsDto
{
    public bool IsSelected { get; set; }
    public int? Spend { get; set; }
    public int? PointsToReceive { get; set; }
}