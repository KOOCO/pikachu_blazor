using Kooco.Pikachu.Members;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;

namespace Kooco.Pikachu.TierManagement;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.VipTierSettings.Default)]
public class VipTierSettingAppService : PikachuAppService, IVipTierSettingAppService
{
    private readonly VipTierSettingManager _vipTierSettingManager;
    private readonly IMemberRepository _memberRepository;
    private readonly IBackgroundJobManager _backgroundJobManager;

    public VipTierSettingAppService(
        VipTierSettingManager vipTierSettingManager,
        IMemberRepository memberRepository,
        IBackgroundJobManager backgroundJobManager
        )
    {
        _vipTierSettingManager = vipTierSettingManager;
        _memberRepository = memberRepository;
        _backgroundJobManager = backgroundJobManager;
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

        await _backgroundJobManager.EnqueueAsync(new UpdateMemberTierArgs { TenantId = CurrentTenant?.Id }, BackgroundJobPriority.High);

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

    [AllowAnonymous]
    public async Task UpdateMemberTierAsync(Guid? tenantId)
    {
        using (CurrentTenant.Change(tenantId))
        {
            await _memberRepository.UpdateMemberTierAsync();
        }
    }
}
