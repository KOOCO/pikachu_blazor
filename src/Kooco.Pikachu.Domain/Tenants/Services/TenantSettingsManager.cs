using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.TenantManagement;
using Kooco.Pikachu.Extensions;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Exceptions;

namespace Kooco.Pikachu.Tenants.Services;

public class TenantSettingsManager(IRepository<TenantSettings, Guid> tenantSettingsRepository, IRepository<Tenant, Guid> tenantRepository) : DomainService
{
    public async Task<TenantSettings> GetAsync()
    {
        var tenantSettings = await tenantSettingsRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.TenantId == CurrentTenant.Id);

        if (tenantSettings is null)
        {
            tenantSettings = new TenantSettings(GuidGenerator.Create());
            await tenantSettingsRepository.InsertAsync(tenantSettings);
        }
        await tenantSettingsRepository.EnsurePropertyLoadedAsync(tenantSettings, t => t.Tenant);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateTenantInformationAsync(string? shortCode, string? tenantUrl, string? domain, string? tenantContactTitle,
        string? tenantContactPerson, string? contactPhone, string? tenantContactEmail)
    {
        Check.NotNullOrWhiteSpace(shortCode, nameof(shortCode), TenantSettingsConsts.MaxShortCodeLength);
        Check.NotNullOrWhiteSpace(tenantUrl, nameof(tenantUrl), TenantSettingsConsts.MaxTenantUrlLength);
        Check.NotNullOrWhiteSpace(domain, nameof(domain), TenantSettingsConsts.MaxDomainLength);
        Check.NotNullOrWhiteSpace(tenantContactTitle, nameof(tenantContactTitle), TenantSettingsConsts.MaxTenantContactTitleLength);
        Check.NotNullOrWhiteSpace(tenantContactPerson, nameof(tenantContactPerson), TenantSettingsConsts.MaxTenantContactPersonLength);
        Check.NotNullOrWhiteSpace(contactPhone, nameof(contactPhone), TenantSettingsConsts.MaxContactPhoneLength);
        Check.NotNullOrWhiteSpace(tenantContactEmail, nameof(tenantContactEmail), TenantSettingsConsts.MaxTenantContactEmailLength);

        if (tenantUrl?.IsEmptyOrValidUrl() == false)
        {
            throw new InvalidUrlException(nameof(tenantUrl));
        }

        if (domain?.IsEmptyOrValidUrl() == false)
        {
            throw new InvalidUrlException(nameof(domain));
        }

        var tenantSettings = await GetAsync();
        tenantSettings.SetContactPhone(contactPhone);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        tenant.RemoveProperty(Constant.ShortCode);
        tenant.SetProperty(Constant.ShortCode, shortCode);

        tenant.RemoveProperty(Constant.TenantContactTitle);
        tenant.SetProperty(Constant.TenantContactTitle, tenantContactTitle);

        tenant.RemoveProperty(Constant.TenantContactPerson);
        tenant.SetProperty(Constant.TenantContactPerson, tenantContactPerson);

        tenant.RemoveProperty(Constant.TenantContactEmail);
        tenant.SetProperty(Constant.TenantContactEmail, tenantContactEmail);

        tenant.RemoveProperty(Constant.TenantUrl);
        tenant.SetProperty(Constant.TenantUrl, tenantUrl);

        tenant.RemoveProperty(Constant.Domain);
        tenant.SetProperty(Constant.Domain, domain);

        await tenantRepository.UpdateAsync(tenant);
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateCustomerServiceAsync(string? companyName, string? businessRegistrationNumber,
        string? customerServiceEmail, string? customerServiceContactPhone, DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        Check.NotNullOrWhiteSpace(companyName, nameof(companyName), maxLength: TenantSettingsConsts.MaxCompanyNameLength);
        Check.NotNullOrWhiteSpace(businessRegistrationNumber, nameof(businessRegistrationNumber), maxLength: TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        Check.NotNullOrWhiteSpace(customerServiceEmail, nameof(customerServiceEmail), maxLength: TenantSettingsConsts.MaxCustomerServiceEmailLength);
        Check.NotNullOrWhiteSpace(customerServiceContactPhone, nameof(customerServiceContactPhone), maxLength: TenantSettingsConsts.MaxContactPhoneLength);
        if (serviceHoursFrom > serviceHoursTo)
        {
            throw new InvalidServiceHoursException();
        }

        var tenantSettings = await GetAsync();

        tenantSettings.SetCompanyName(companyName);
        tenantSettings.SetBusinessRegistrationNumber(businessRegistrationNumber);
        tenantSettings.SetCustomerServiceEmail(customerServiceEmail);
        tenantSettings.SetCustomerServiceContactPhone(customerServiceContactPhone);
        tenantSettings.SetServiceHours(serviceHoursFrom, serviceHoursTo);

        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdatePrivacyPolicyAsync(string privacyPolicy)
    {
        Check.NotNullOrWhiteSpace(privacyPolicy, nameof(privacyPolicy));

        var tenantSettings = await GetAsync();
        tenantSettings.SetPrivacyPolicy(privacyPolicy);
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateTenantFrontendInformationAsync(string? webpageTitle, string? faviconUrl, string? logoUrl, string? bannerUrl, string? description)
    {
        Check.NotNullOrWhiteSpace(webpageTitle, nameof(webpageTitle), TenantSettingsConsts.MaxWebpageTitleLength);
        Check.Length(description, nameof(description), TenantSettingsConsts.MaxDescriptionLength);

        var tenantSettings = await GetAsync();
        tenantSettings.SetWebpageTitle(webpageTitle);
        tenantSettings.SetFaviconUrl(faviconUrl);
        tenantSettings.SetDescription(description);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        tenant.RemoveProperty(Constant.Logo);
        tenant.SetProperty(Constant.Logo, logoUrl);

        tenant.RemoveProperty(Constant.BannerUrl);
        tenant.SetProperty(Constant.BannerUrl, bannerUrl);

        await tenantRepository.UpdateAsync(tenant);
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateTenantSocialMediaAsync(string? facebookTitle, string? facebookLink, string? instagramTitle, string? instagramLink, string? lineTitle, string? lineLink)
    {
        var tenantSettings = await GetAsync();
        tenantSettings.SetSocials(facebookTitle, facebookLink, instagramTitle, instagramLink, lineTitle, lineLink);
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateTenantGoogleTagManagerAsync(bool gtmEnabled, string? gtmContainerId)
    {
        var tenantSettings = await GetAsync();
        tenantSettings.SetGtm(gtmEnabled, gtmContainerId);
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }
}
