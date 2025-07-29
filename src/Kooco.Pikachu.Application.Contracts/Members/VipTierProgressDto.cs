using System;

namespace Kooco.Pikachu.Members;

public class VipTierProgressDto
{
    public string? CurrentLevel { get; set; }
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
    public int? RequiredOrders { get; set; }
    public int CurrentOrders { get; set; }
    public decimal? RequiredAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? CountingSince { get; set; }
}