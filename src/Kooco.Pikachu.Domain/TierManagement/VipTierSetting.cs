using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.TierManagement;

public class VipTierSetting : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public bool BasedOnAmount { get; set; }
    public bool BasedOnCount { get; set; }
    public VipTierCondition? TierCondition { get; set; }
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
        VipTierCondition? tierCondition
        ) : base(id)
    {
        BasedOnAmount = basedOnOrder;
        BasedOnCount = basedOnCount;
        TierCondition = tierCondition;
        Tiers = new List<VipTier>();
    }

    public VipTier AddTier(Guid id, Tier tier, string? tierName, int ordersAmount, int ordersCount)
    {
        var vipTier = new VipTier(id, tier, tierName, ordersAmount, ordersCount, Id);
        Tiers.Add(vipTier);
        return vipTier;
    }
}
