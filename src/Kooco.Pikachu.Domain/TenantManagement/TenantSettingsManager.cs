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
    public async Task<TenantSettings> GetAsync()
    {
        var tenantSettings = await tenantSettingsRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.TenantId == CurrentTenant.Id);

        if (tenantSettings is null)
        {
            tenantSettings = new TenantSettings(GuidGenerator.Create());
            await tenantSettingsRepository.InsertAsync(tenantSettings);

            var tenant = await tenantRepository.FirstOrDefaultAsync(x => x.Id == CurrentTenant.Id);
            tenantSettings.Tenant = tenant;
        }
        else
        {
            await tenantSettingsRepository.EnsurePropertyLoadedAsync(tenantSettings, t => t.Tenant);
        }

        return tenantSettings;
    }

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
        if (gtmEnabled)
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

    public async Task<TenantSettings> UpdateTenantInformationAsync(string? domain, string? tenantContactTitle,
        string? tenantContactPerson, string? contactPhone, string? tenantContactEmail)
    {
        Check.NotNullOrWhiteSpace(domain, nameof(domain), TenantSettingsConsts.MaxDomainLength);
        Check.NotNullOrWhiteSpace(tenantContactTitle, nameof(tenantContactTitle), TenantSettingsConsts.MaxTenantContactTitleLength);
        Check.NotNullOrWhiteSpace(tenantContactPerson, nameof(tenantContactPerson), TenantSettingsConsts.MaxTenantContactPersonLength);
        Check.NotNullOrWhiteSpace(contactPhone, nameof(contactPhone), TenantSettingsConsts.MaxContactPhoneLength);
        Check.NotNullOrWhiteSpace(tenantContactEmail, nameof(tenantContactEmail), TenantSettingsConsts.MaxTenantContactEmailLength);

        var tenantSettings = await GetAsync();
        tenantSettings.SetContactPhone(contactPhone);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        tenant.RemoveProperty(Constant.TenantContactTitle);
        tenant.SetProperty(Constant.TenantContactTitle, tenantContactTitle);

        tenant.RemoveProperty(Constant.TenantContactPerson);
        tenant.SetProperty(Constant.TenantContactPerson, tenantContactPerson);

        tenant.RemoveProperty(Constant.TenantContactEmail);
        tenant.SetProperty(Constant.TenantContactEmail, tenantContactEmail);

        tenant.RemoveProperty(Constant.Domain);
        tenant.SetProperty(Constant.Domain, domain);

        await tenantRepository.UpdateAsync(tenant);
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateCustomerServiceAsync(string? shortCode, string? companyName, string? businessRegistrationNumber,
        string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        Check.NotNullOrWhiteSpace(shortCode, nameof(shortCode), maxLength: TenantSettingsConsts.MaxShortCodeLength, minLength: TenantSettingsConsts.MinShortCodeLength);
        Check.NotNullOrWhiteSpace(companyName, nameof(companyName), maxLength: TenantSettingsConsts.MaxCompanyNameLength);
        Check.NotNullOrWhiteSpace(businessRegistrationNumber, nameof(businessRegistrationNumber), maxLength: TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        Check.NotNullOrWhiteSpace(customerServiceEmail, nameof(customerServiceEmail), maxLength: TenantSettingsConsts.MaxCustomerServiceEmailLength);
        if (serviceHoursFrom > serviceHoursTo)
        {
            throw new InvalidServiceHoursException();
        }

        var tenantSettings = await GetAsync();

        tenantSettings.SetCompanyName(companyName);
        tenantSettings.SetBusinessRegistrationNumber(businessRegistrationNumber);
        tenantSettings.SetCustomerServiceEmail(customerServiceEmail);
        tenantSettings.SetServiceHours(serviceHoursFrom, serviceHoursTo);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        tenant.RemoveProperty(Constant.ShortCode);
        tenant.SetProperty(Constant.ShortCode, shortCode);
        await tenantRepository.UpdateAsync(tenant);
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

    public async Task<TenantSettings> UpdateTenantFrontendInformationAsync(string? webpageTitle, string? faviconUrl, string? logoUrl, string? bannerUrl)
    {
        Check.NotNullOrWhiteSpace(webpageTitle, nameof(webpageTitle), TenantSettingsConsts.MaxWebpageTitleLength);

        var tenantSettings = await GetAsync();
        tenantSettings.SetWebpageTitle(webpageTitle);
        tenantSettings.SetFaviconUrl(faviconUrl);

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
}
