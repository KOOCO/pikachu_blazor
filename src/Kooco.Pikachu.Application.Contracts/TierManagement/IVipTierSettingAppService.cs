using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TierManagement;

public interface IVipTierSettingAppService : IApplicationService
{
    Task<VipTierSettingDto> FirstOrDefaultAsync();
    Task<VipTierSettingDto> UpdateAsync(UpdateVipTierSettingDto input);
    Task<List<string>> GetVipTierNamesAsync();
    Task UpdateMemberTierAsync(Guid? tenantId, bool shouldConfigureRecurringJob = false, CancellationToken cancellationToken = default);
}
