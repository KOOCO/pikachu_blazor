using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.TierManagement;

[RemoteService(IsEnabled = false)]
public class VipTierSettingAppService : PikachuAppService, IVipTierSettingAppService
{
    private readonly VipTierSettingManager _vipTierSettingManager;

    public VipTierSettingAppService(
        VipTierSettingManager vipTierSettingManager
        )
    {
        _vipTierSettingManager = vipTierSettingManager;
    }

    public async Task<VipTierSettingDto> FirstOrDefaultAsync()
    {
        var vipTierSetting = await _vipTierSettingManager.FirstOrDefaultAsync();
        return vipTierSetting == null
            ? new VipTierSettingDto()
            : ObjectMapper.Map<VipTierSetting, VipTierSettingDto>(vipTierSetting);
    }

    public async Task<VipTierSettingDto> UpdateAsync(UpdateVipTierSettingDto input)
    {
        var vipTierSetting = await _vipTierSettingManager.AddOrUpdateAsync(
            input.BasedOnCount,
            input.BasedOnAmount,
            input.TierCondition
        );

        var validTiers = input.Tiers
            .Where(tier => !string.IsNullOrWhiteSpace(tier.TierName))
            .ToList();

        if (validTiers.GroupBy(t => t.Tier).Any(g => g.Count() > 1))
        {
            throw new DuplicateTierException();
        }

        vipTierSetting.Tiers.RemoveAll(tier => !validTiers.Any(t => t.Id == tier.Id));

        foreach (var tier in validTiers)
        {
            _vipTierSettingManager.AddOrUpdateVipTier(vipTierSetting, tier.Id, tier.Tier,
                tier.TierName, tier.OrdersAmount, tier.OrdersCount);
        }

        return ObjectMapper.Map<VipTierSetting, VipTierSettingDto>(vipTierSetting);
    }

    public async Task<List<string>> GetVipTierNamesAsync()
    {
        var vipTierSetting = await _vipTierSettingManager.FirstOrDefaultAsync();
        return [.. vipTierSetting?.Tiers?
            .Where(tier => !string.IsNullOrWhiteSpace(tier.TierName))
            .OrderBy(tier => tier.TierName)
            .Select(tier => tier.TierName) 
            ?? []
            ];
    }
}
