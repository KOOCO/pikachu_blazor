using Hangfire;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    private readonly IEmailAppService _emailAppService;

    public VipTierSettingAppService(
        VipTierSettingManager vipTierSettingManager,
        IMemberRepository memberRepository,
        IBackgroundJobManager backgroundJobManager,
        IEmailAppService emailAppService
        )
    {
        _vipTierSettingManager = vipTierSettingManager;
        _memberRepository = memberRepository;
        _backgroundJobManager = backgroundJobManager;
        _emailAppService = emailAppService;
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
        var existing = await _vipTierSettingManager.FirstOrDefaultAsync();
        bool shouldConfigureRecurringJob = existing?.IsResetConfigured != true || string.IsNullOrWhiteSpace(existing?.JobId);

        var vipTierSetting = await _vipTierSettingManager
            .AddOrUpdateAsync(
                input.BasedOnCount,
                input.BasedOnAmount,
                input.TierCondition,
                input.StartDate.Value,
                input.IsResetEnabled,
                input.ResetFrequency
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

        await _backgroundJobManager.EnqueueAsync(
            new UpdateMemberTierArgs
            {
                TenantId = CurrentTenant?.Id,
                ShouldConfigureRecurringJob = shouldConfigureRecurringJob
            },
            BackgroundJobPriority.High,
            TimeSpan.FromSeconds(10)
            );

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
    public async Task UpdateMemberTierAsync(Guid? tenantId, bool shouldConfigureRecurringJob = false, CancellationToken cancellationToken = default)
    {
        using (CurrentTenant.Change(tenantId))
        {
            var vipTierSetting = await _vipTierSettingManager.FirstOrDefaultAsync();
            if (vipTierSetting == null)
            {
                Logger.LogWarning("VipTierSetting not found. Cannot update member tier.");
                return;
            }

            if (shouldConfigureRecurringJob)
            {
                if (vipTierSetting.JobId != null)
                {
                    BackgroundJob.Delete(vipTierSetting.JobId);
                    vipTierSetting.JobId = null;
                }
                if (vipTierSetting.IsResetEnabled && vipTierSetting.ResetFrequency.HasValue)
                {
                    vipTierSetting.LastResetDate = TaipeiTime.Now();
                    vipTierSetting.NextResetDate = TaipeiTime.VipTierNextRun(vipTierSetting.ResetFrequency.Value);

                    var nextRunUtc = TaipeiTime.Utc(vipTierSetting.NextResetDate.Value);

                    vipTierSetting.JobId = BackgroundJob.Schedule<UpdateMemberTierJob>(
                        job => job.ExecuteAsync(new UpdateMemberTierArgs
                        {
                            TenantId = tenantId,
                            ShouldConfigureRecurringJob = true
                        }),
                        nextRunUtc - DateTime.UtcNow
                        );
                }
            }

            await CurrentUnitOfWork!.SaveChangesAsync(cancellationToken);
            var result = await _memberRepository.UpdateMemberTierAsync(cancellationToken);

            if (result.Count > 0)
            {
                await _emailAppService.SendVipTierUpgradeEmailAsync(ObjectMapper.Map<List<VipTierUpgradeEmailModel>, List<VipTierUpgradeEmailDto>>(result));
            }
        }
    }
}
