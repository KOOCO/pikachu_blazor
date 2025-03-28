using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.TierManagement;

public class UpdateVipTierSettingDto
{
    public Guid Id { get; set; }
    public bool BasedOnAmount { get; set; }
    public bool BasedOnCount { get; set; }
    public VipTierCondition? TierCondition { get; set; }
    public List<UpdateVipTierDto> Tiers { get; set; }
}