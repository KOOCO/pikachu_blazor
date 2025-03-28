using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.TierManagement;

public class VipTier : Entity<Guid>, IMultiTenant
{
    public Tier Tier { get; set; }
    public string? TierName { get; set; }
    public int OrdersAmount { get; private set; }
    public int OrdersCount { get; private set; }
    public Guid TierSettingId { get; set; }
    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(TierSettingId))]
    public virtual VipTierSetting? TierSetting { get; set; }

    private VipTier() { }

    public VipTier(
        Guid id,
        Tier tier,
        string? tierName,
        int ordersAmount,
        int ordersCount,
        Guid tierSettingId
        ) : base(id)
    {
        Tier = tier;
        SetTierName(tierName);
        SetOrdersAmount(ordersAmount);
        SetOrdersCount(ordersCount);
        TierSettingId = tierSettingId;
    }

    public void SetTierName(string? tierName)
    {
        TierName = Check.NotNullOrWhiteSpace(tierName, nameof(TierName), VipTierConsts.MaxTierNameLength);
    }

    public void SetOrdersAmount(int ordersAmount)
    {
        OrdersAmount = Check.Range(ordersAmount, nameof(OrdersAmount), 0);
    }

    public void SetOrdersCount(int ordersCount)
    {
        OrdersCount = Check.Range(ordersCount, nameof(OrdersCount), 0);
    }
}
