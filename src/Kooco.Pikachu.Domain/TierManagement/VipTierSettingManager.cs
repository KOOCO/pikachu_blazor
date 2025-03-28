using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.TierManagement;

public class VipTierSettingManager(IRepository<VipTierSetting, Guid> repository) : DomainService
{
    private readonly IRepository<VipTierSetting, Guid> _repository = repository;

    public async Task<VipTierSetting> AddOrUpdateAsync(bool basedOnCount, bool basedOnAmount, VipTierCondition? tierCondition)
    {
        if (!basedOnCount && !basedOnAmount)
        {
            throw new VipTierConditionCheckedException();
        }

        if (basedOnCount && basedOnAmount && !tierCondition.HasValue)
        {
            throw new VipTierConditionException();
        }

        var vipTierSetting = await _repository.FirstOrDefaultAsync();
        if (vipTierSetting == null)
        {
            vipTierSetting = new VipTierSetting(
                GuidGenerator.Create(),
                basedOnAmount,
                basedOnCount,
                tierCondition
                );
            await _repository.InsertAsync(vipTierSetting);
        }
        else
        {
            await _repository.EnsureCollectionLoadedAsync(vipTierSetting, s => s.Tiers);
            vipTierSetting.BasedOnCount = basedOnCount;
            vipTierSetting.BasedOnAmount = basedOnAmount;
            vipTierSetting.TierCondition = tierCondition;
            await _repository.UpdateAsync(vipTierSetting);
        }

        return vipTierSetting;
    }

    public void AddOrUpdateVipTier(VipTierSetting vipTierSetting, Guid id, Tier tier,
        string? tierName, int ordersAmount, int ordersCount)
    {
        var vipTier = vipTierSetting.Tiers.FirstOrDefault(t => t.Id == id);
        if (vipTier == null)
        {
            vipTierSetting.AddTier(
                GuidGenerator.Create(),
                tier,
                tierName,
                ordersAmount,
                ordersCount
                );
        }
        else
        {
            vipTier.Tier = tier;
            vipTier.TierName = tierName;
            vipTier.SetOrdersAmount(ordersAmount);
            vipTier.SetOrdersCount(ordersCount);
        }
    }

    public async Task<VipTierSetting?> FirstOrDefaultAsync()
    {
        var vipTierSetting = await _repository.FirstOrDefaultAsync();
        
        if (vipTierSetting != null)
        {
            await _repository.EnsureCollectionLoadedAsync(vipTierSetting, tier => tier.Tiers);
        }
        
        return vipTierSetting;
    }
}
