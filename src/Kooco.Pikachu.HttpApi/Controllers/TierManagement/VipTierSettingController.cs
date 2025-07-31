using Asp.Versioning;
using Kooco.Pikachu.TierManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Controllers.TierManagement;

[ControllerName("VipTierSettings")]
[Area("app")]
[Route("api/app/vip-tier-settings")]
public class VipTierSettingController(IVipTierSettingAppService vipTierSettingAppService) : PikachuController, IVipTierSettingAppService
{
    private readonly IVipTierSettingAppService _vipTierSettingAppService = vipTierSettingAppService;

    [HttpGet]
    public Task<VipTierSettingDto> FirstOrDefaultAsync()
    {
        return _vipTierSettingAppService.FirstOrDefaultAsync();
    }

    [HttpGet("tier-names")]
    public Task<List<string>> GetVipTierNamesAsync()
    {
        return _vipTierSettingAppService.GetVipTierNamesAsync();
    }

    [HttpPost]
    public Task<VipTierSettingDto> UpdateAsync(UpdateVipTierSettingDto input)
    {
        return _vipTierSettingAppService.UpdateAsync(input);
    }

    [HttpPost("member-tier/{tenantId}")]
    [RemoteService(IsEnabled = false)]
    public Task UpdateMemberTierAsync(Guid? tenantId, bool shouldConfigureRecurringJob = false, CancellationToken cancellationToken = default)
    {
        return _vipTierSettingAppService.UpdateMemberTierAsync(tenantId, shouldConfigureRecurringJob, cancellationToken);
    }
}
