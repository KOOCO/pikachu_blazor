using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.TenantManagement;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.TenantSettings.Default)]
public class TenantSettingsAppService(TenantSettingsManager tenantSettingsManager,
    IRepository<TenantSettings, Guid> tenantSettingsRepository) : PikachuAppService, ITenantSettingsAppService
{
    public async Task<TenantSettingsDto?> FirstOrDefaultAsync()
    {
        var tenantSettings = await tenantSettingsRepository.FirstOrDefaultAsync();
        return ObjectMapper.Map<TenantSettings, TenantSettingsDto>(tenantSettings);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantSettingsDto> UpdateAsync(UpdateTenantSettingsDto input)
    {
        Check.NotNull(input, nameof(input));

        var tenantSettings = await tenantSettingsRepository.FirstOrDefaultAsync();

        if (tenantSettings is null)
        {
            tenantSettings = await tenantSettingsManager.CreateAsync(input.FaviconUrl, input.WebpageTitle, input.PrivacyPolicy,
                input.CompanyName, input.BusinessRegistrationNumber, input.ContactPhone, input.CustomerServiceEmail, input.ServiceHoursFrom,
                input.ServiceHoursTo);
        }
        else
        {
            await tenantSettingsManager.UpdateAsync(tenantSettings, input.FaviconUrl, input.WebpageTitle, input.PrivacyPolicy,
                input.CompanyName, input.BusinessRegistrationNumber, input.ContactPhone, input.CustomerServiceEmail, input.ServiceHoursFrom,
                input.ServiceHoursTo);
        }

        return ObjectMapper.Map<TenantSettings, TenantSettingsDto>(tenantSettings);
    }
}
