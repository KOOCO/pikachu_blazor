using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantManagement;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.TenantSettings.Default)]
public class TenantSettingsAppService(TenantSettingsManager tenantSettingsManager, IRepository<TenantSettings, Guid> tenantSettingsRepository,
    ImageContainerManager imageContainerManager, IRepository<Tenant, Guid> tenantRepository) : PikachuAppService, ITenantSettingsAppService
{
    [AllowAnonymous]
    public async Task<TenantSettingsDto?> FirstOrDefaultAsync()
    {
        var tenantSettings = await tenantSettingsRepository.FirstOrDefaultAsync();

        if (tenantSettings != null)
        {
            await tenantSettingsRepository.EnsurePropertyLoadedAsync(tenantSettings, x => x.Tenant);
        }
        else
        {
            var tenant = await tenantRepository.FirstOrDefaultAsync(x => x.Id == CurrentTenant.Id);
            tenantSettings ??= new();
            tenantSettings.Tenant = tenant;
        }
        return ObjectMapper.Map<TenantSettings, TenantSettingsDto>(tenantSettings);
    }

    [Authorize(PikachuPermissions.TenantSettings.Edit)]
    public async Task<TenantSettingsDto> UpdateAsync(UpdateTenantSettingsDto input)
    {
        Check.NotNull(input, nameof(input));

        var tenantSettings = await tenantSettingsRepository.FirstOrDefaultAsync();

        if (tenantSettings is null)
        {
            tenantSettings = await tenantSettingsManager.CreateAsync(input.WebpageTitle, input.PrivacyPolicy,
                input.CompanyName, input.BusinessRegistrationNumber, input.ContactPhone, input.CustomerServiceEmail, input.ServiceHoursFrom,
                input.ServiceHoursTo, input.FaviconUrl, input.LogoUrl, input.BannerUrl, input.TenantContactTitle, input.TenantContactPerson,
                input.TenantContactEmail, input.Domain, input.ShortCode, input.Facebook, input.Instagram, input.Line, input.GtmEnabled, input.GtmContainerId);
        }
        else
        {
            await tenantSettingsManager.UpdateAsync(tenantSettings, input.WebpageTitle, input.PrivacyPolicy,
                input.CompanyName, input.BusinessRegistrationNumber, input.ContactPhone, input.CustomerServiceEmail, input.ServiceHoursFrom,
                input.ServiceHoursTo, input.FaviconUrl, input.LogoUrl, input.BannerUrl, input.TenantContactTitle, input.TenantContactPerson,
                input.TenantContactEmail, input.Domain, input.ShortCode, input.Facebook, input.Instagram, input.Line, input.GtmEnabled, input.GtmContainerId);
        }

        return ObjectMapper.Map<TenantSettings, TenantSettingsDto>(tenantSettings);
    }

    public async Task<string> UploadImageAsync(UploadImageDto input)
    {
        Check.NotNull(input, nameof(input));
        Check.NotNullOrWhiteSpace(input.Base64, nameof(input.Base64));
        Check.NotNullOrWhiteSpace(input.FileName, nameof(input.FileName));

        string blobName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(input.FileName);
        byte[] fileBytes = Convert.FromBase64String(input.Base64);

        var url = await imageContainerManager.SaveAsync(blobName, fileBytes, true);
        return url;
    }

    public async Task DeleteImageAsync(string blobName)
    {
        await imageContainerManager.DeleteAsync(blobName);
    }
}
