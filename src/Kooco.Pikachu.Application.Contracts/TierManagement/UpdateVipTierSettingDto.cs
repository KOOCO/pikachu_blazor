using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.TierManagement;

public class UpdateVipTierSettingDto
{
    public Guid Id { get; set; }
    public bool BasedOnAmount { get; set; }
    public bool BasedOnCount { get; set; }
    public VipTierCondition? TierCondition { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }
    public bool IsResetEnabled { get; set; }
    public VipTierResetFrequency? ResetFrequency { get; set; }
    public List<UpdateVipTierDto> Tiers { get; set; }
}