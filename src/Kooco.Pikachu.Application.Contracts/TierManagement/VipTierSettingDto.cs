using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TierManagement;

public class VipTierSettingDto : EntityDto<Guid>
{
    public bool BasedOnAmount { get; set; }
    public bool BasedOnCount { get; set; }
    public VipTierCondition? TierCondition { get; set; }
    public Guid? TenantId { get; set; }
    public virtual List<VipTierDto> Tiers { get; set; }
}
