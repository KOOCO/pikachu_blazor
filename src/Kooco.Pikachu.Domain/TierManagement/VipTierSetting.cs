using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.TierManagement;

public class VipTierSetting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public bool BasedOnAmount { get; set; }
    public bool BasedOnCount { get; set; }
    public VipTierCondition? TierCondition { get; set; }
    public DateTime StartDate { get; set; }
    public bool IsResetEnabled { get; set; }
    public VipTierResetFrequency? ResetFrequency { get; set; }
    public string? JobId { get; set; }
    public DateTime? LastResetDateUtc { get; set; }
    public Guid? TenantId { get; set; }

    public virtual ICollection<VipTier> Tiers { get; set; }

    private VipTierSetting()
    {
        Tiers = new List<VipTier>();
    }

    public VipTierSetting(
        Guid id,
        bool basedOnOrder,
        bool basedOnCount,
        VipTierCondition? tierCondition,
        DateTime startDate,
        bool isResetEnabled,
        VipTierResetFrequency? resetFrequency
        ) : base(id)
    {
        BasedOnAmount = basedOnOrder;
        BasedOnCount = basedOnCount;
        TierCondition = tierCondition;
        StartDate = startDate;
        SetIsResetEnabled(isResetEnabled, resetFrequency);
        Tiers = new List<VipTier>();
    }

    public void SetIsResetEnabled(bool isResetEnabled, VipTierResetFrequency? resetFrequency)
    {
        IsResetEnabled = isResetEnabled;
        if (IsResetEnabled)
        {
            ResetFrequency = Check.NotNull(resetFrequency, nameof(ResetFrequency));
        }
        else
        {
            ResetFrequency = null;
        }
    }

    public VipTier AddTier(Guid id, Tier tier, string? tierName, int ordersAmount, int ordersCount)
    {
        var vipTier = new VipTier(id, tier, tierName, ordersAmount, ordersCount, Id);
        Tiers.Add(vipTier);
        return vipTier;
    }
}
