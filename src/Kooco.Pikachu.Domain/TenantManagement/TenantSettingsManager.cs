using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantManagement;

public class TenantSettingsManager(IRepository<TenantSettings, Guid> tenantSettingsRepository, IRepository<Tenant, Guid> tenantRepository) : DomainService
{
    public async Task<TenantSettings> CreateAsync(string? webpageTitle, string? privacyPolicy, string? companyName,
        string? businessRegistrationNumber, string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo,
        string? faviconUrl, string? logoUrl, string? bannerUrl, string? tenantContactTitle, string? tenantContactPerson, string? tenantContactEmail,
        string? domain, string? shortCode, string? facebook, string? instagram, string? line, bool gtmEnabled, string? gtmContainerId)
    {
        ValidateInputs(webpageTitle, privacyPolicy, companyName, businessRegistrationNumber,
            contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo, faviconUrl, logoUrl, bannerUrl,
            tenantContactTitle, tenantContactPerson, tenantContactEmail, domain, shortCode, gtmEnabled, gtmContainerId);

        if (await tenantSettingsRepository.AnyAsync())
        {
            throw new TenantSettingsAlreadyExistException();
        }

        var tenantSettings = new TenantSettings(GuidGenerator.Create(), faviconUrl, webpageTitle, privacyPolicy, companyName,
            businessRegistrationNumber, contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo,
            facebook, instagram, line, gtmEnabled, gtmContainerId);

        await tenantSettingsRepository.InsertAsync(tenantSettings);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        UpdateTenantProperties(tenant, logoUrl, bannerUrl, tenantContactTitle, tenantContactPerson, tenantContactEmail, domain, shortCode);

        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateAsync(TenantSettings tenantSettings, string? webpageTitle, string? privacyPolicy, string? companyName,
        string? businessRegistrationNumber, string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo,
        string? faviconUrl, string? logoUrl, string? bannerUrl, string? tenantContactTitle, string? tenantContactPerson, string? tenantContactEmail,
        string? domain, string? shortCode, string? facebook, string? instagram, string? line, bool gtmEnabled, string? gtmContainerId)
    {
        Check.NotNull(tenantSettings, nameof(tenantSettings));
        ValidateInputs(webpageTitle, privacyPolicy, companyName, businessRegistrationNumber,
            contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo, faviconUrl, logoUrl, bannerUrl,
            tenantContactTitle, tenantContactPerson, tenantContactEmail, domain, shortCode, gtmEnabled, gtmContainerId);

        tenantSettings.SetFaviconUrl(faviconUrl);
        tenantSettings.SetWebpageTitle(webpageTitle);
        tenantSettings.SetPrivacyPolicy(privacyPolicy);
        tenantSettings.SetCompanyName(companyName);
        tenantSettings.SetBusinessRegistrationNumber(businessRegistrationNumber);
        tenantSettings.SetContactPhone(contactPhone);
        tenantSettings.SetCustomerServiceEmail(customerServiceEmail);
        tenantSettings.SetServiceHours(serviceHoursFrom, serviceHoursTo);
        tenantSettings.SetSocials(facebook, instagram, line);
        tenantSettings.SetGtm(gtmEnabled, gtmContainerId);

        await tenantSettingsRepository.UpdateAsync(tenantSettings);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        UpdateTenantProperties(tenant, logoUrl, bannerUrl, tenantContactTitle, tenantContactPerson, tenantContactEmail, domain, shortCode);

        return tenantSettings;
    }

    private static void UpdateTenantProperties(Tenant tenant, string? logoUrl, string? bannerUrl, string? tenantContactTitle,
        string? tenantContactPerson, string? tenantContactEmail, string? domain, string? shortCode)
    {
        tenant.RemoveProperty(Constant.Logo);
        tenant.SetProperty(Constant.Logo, logoUrl);

        tenant.RemoveProperty(Constant.BannerUrl);
        tenant.SetProperty(Constant.BannerUrl, bannerUrl);

        tenant.RemoveProperty(Constant.TenantContactTitle);
        tenant.SetProperty(Constant.TenantContactTitle, tenantContactTitle);

        tenant.RemoveProperty(Constant.TenantContactPerson);
        tenant.SetProperty(Constant.TenantContactPerson, tenantContactPerson);

        tenant.RemoveProperty(Constant.TenantContactEmail);
        tenant.SetProperty(Constant.TenantContactEmail, tenantContactEmail);

        tenant.RemoveProperty(Constant.Domain);
        tenant.SetProperty(Constant.Domain, domain);

        tenant.RemoveProperty(Constant.ShortCode);
        tenant.SetProperty(Constant.ShortCode, shortCode);
    }

    private static void ValidateInputs(string? webpageTitle, string? privacyPolicy, string? companyName, string? businessRegistrationNumber,
        string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo,
        string? faviconUrl, string? logoUrl, string? bannerUrl, string? tenantContactTitle, string? tenantContactPerson, string? tenantContactEmail,
        string? domain, string? shortCode, bool gtmEnabled, string? gtmContainerId)
    {
        Check.NotNullOrWhiteSpace(webpageTitle, nameof(webpageTitle), maxLength: TenantSettingsConsts.MaxWebpageTitleLength);
        Check.NotNullOrWhiteSpace(privacyPolicy, nameof(privacyPolicy));
        Check.NotNullOrWhiteSpace(companyName, nameof(companyName), maxLength: TenantSettingsConsts.MaxCompanyNameLength);
        Check.NotNullOrWhiteSpace(businessRegistrationNumber, nameof(businessRegistrationNumber), maxLength: TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        Check.NotNullOrWhiteSpace(contactPhone, nameof(contactPhone), maxLength: TenantSettingsConsts.MaxContactPhoneLength);
        Check.NotNullOrWhiteSpace(customerServiceEmail, nameof(customerServiceEmail), maxLength: TenantSettingsConsts.MaxCustomerServiceEmailLength);
        Check.NotNullOrWhiteSpace(tenantContactTitle, nameof(tenantContactTitle), maxLength: TenantSettingsConsts.MaxTenantContactTitleLength);
        Check.NotNullOrWhiteSpace(tenantContactPerson, nameof(tenantContactPerson), maxLength: TenantSettingsConsts.MaxTenantContactPersonLength);
        Check.NotNullOrWhiteSpace(tenantContactEmail, nameof(tenantContactEmail), maxLength: TenantSettingsConsts.MaxTenantContactEmailLength);
        Check.NotNullOrWhiteSpace(domain, nameof(domain), maxLength: TenantSettingsConsts.MaxDomainLength);
        Check.NotNullOrWhiteSpace(shortCode, nameof(shortCode), maxLength: TenantSettingsConsts.MaxShortCodeLength, minLength: TenantSettingsConsts.MinShortCodeLength);
        if(gtmEnabled)
        {
            Check.NotNullOrWhiteSpace(gtmContainerId, nameof(gtmContainerId), maxLength: TenantSettingsConsts.MaxGtmContainerIdLength);
        }
        //Check.NotNullOrWhiteSpace(faviconUrl, nameof(faviconUrl));
        //Check.NotNullOrWhiteSpace(logoUrl, nameof(logoUrl));
        //Check.NotNullOrWhiteSpace(bannerUrl, nameof(bannerUrl));

        if (serviceHoursFrom > serviceHoursTo)
        {
            throw new InvalidServiceHoursException();
        }
    }
}
