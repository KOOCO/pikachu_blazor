using System;
using System.Text.Json.Serialization;

namespace Kooco.Pikachu.Members;

public class VipTierProgressDto
{
    public string? CurrentLevel { get; set; }
    public string? NextLevel { get; set; }
    public DateTime CalculationStartDate { get; set; }
    public VipTierResetConfigDto ResetConfig { get; set; } = default!;
    public VipTierProgressToNextTierDto ProgressToNextLevel { get; set; } = default!;
}

public class VipTierResetConfigDto
{
    public bool IsResetEnabled { get; set; }
    public int? FrequencyMonths { get; set; }
    public DateTime? NextResetDate { get; set; }
    public DateTime? LastResetDate { get; set; }
}

public class VipTierProgressToNextTierDto
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AreBothRequired { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RequiredOrders { get; set; }
    public int CurrentOrders { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? RequiredAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? CountingSince { get; set; }
}