using System;

namespace Kooco.Pikachu.Members;

public class VipTierProgressModel
{
    public string? CurrentLevel { get; set; }
    public string? NextLevel { get; set; }
    public DateTime CalculationStartDate { get; set; }
    public VipTierResetConfig? ResetConfig { get; set; } = default!;
    public VipTierProgressToNextTier? ProgressToNextLevel { get; set; } = default!;
}

public class VipTierResetConfig
{
    public bool IsResetEnabled { get; set; }
    public int? FrequencyMonths { get; set; }
    public DateTime? NextResetDate { get; set; }
    public DateTime? LastResetDate { get; set; }
}

public class VipTierProgressToNextTier
{
    public bool? AreBothRequired { get; set; }
    public int? RequiredOrders { get; set; }
    public int CurrentOrders { get; set; }
    public decimal? RequiredAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? CountingSince { get; set; }
}