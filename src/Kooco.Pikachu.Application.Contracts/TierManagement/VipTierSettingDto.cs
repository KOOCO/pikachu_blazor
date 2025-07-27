using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.TierManagement;

public class VipTierSettingDto : EntityDto<Guid>
{
    public bool BasedOnAmount { get; set; }
    public bool BasedOnCount { get; set; }
    public VipTierCondition? TierCondition { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsResetEnabled { get; set; }
    public VipTierResetFrequency? ResetFrequency { get; set; }
    public string? JobId { get; set; }
    public DateTime? LastResetDateUtc { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? TenantId { get; set; }
    public virtual List<VipTierDto> Tiers { get; set; }
}
