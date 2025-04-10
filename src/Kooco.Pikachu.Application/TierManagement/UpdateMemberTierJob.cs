﻿using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.TierManagement;

public class UpdateMemberTierJob(IVipTierSettingAppService vipTierSettingAppService) : AsyncBackgroundJob<UpdateMemberTierArgs>, ITransientDependency
{
    private readonly IVipTierSettingAppService _vipTierSettingAppService = vipTierSettingAppService;

    public override async Task ExecuteAsync(UpdateMemberTierArgs args)
    {
        await _vipTierSettingAppService.UpdateMemberTierAsync(args.TenantId);
    }
}
