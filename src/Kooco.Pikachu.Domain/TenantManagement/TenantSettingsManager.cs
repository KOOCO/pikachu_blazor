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
        string? faviconUrl, string? logoUrl, string? bannerUrl)
    {
        ValidateInputs(webpageTitle, privacyPolicy, companyName, businessRegistrationNumber,
            contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo,
            faviconUrl, logoUrl, bannerUrl);

        if (await tenantSettingsRepository.AnyAsync())
        {
            throw new TenantSettingsAlreadyExistException();
        }

        var tenantSettings = new TenantSettings(GuidGenerator.Create(), faviconUrl, webpageTitle, privacyPolicy, companyName,
            businessRegistrationNumber, contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo);

        await tenantSettingsRepository.InsertAsync(tenantSettings);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        if (logoUrl != null)
        {
            tenant.RemoveProperty(Constant.Logo);
            tenant.SetProperty(Constant.Logo, logoUrl);
        }

        if (bannerUrl != null)
        {
            tenant.RemoveProperty(Constant.BannerUrl);
            tenant.SetProperty(Constant.BannerUrl, bannerUrl);
        }

        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateAsync(TenantSettings tenantSettings, string? webpageTitle, string? privacyPolicy, string? companyName,
        string? businessRegistrationNumber, string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo,
        string? faviconUrl, string? logoUrl, string? bannerUrl)
    {
        Check.NotNull(tenantSettings, nameof(tenantSettings));
        ValidateInputs(webpageTitle, privacyPolicy, companyName, businessRegistrationNumber,
            contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo,
            faviconUrl, logoUrl, bannerUrl);

        tenantSettings.SetFaviconUrl(faviconUrl);
        tenantSettings.SetWebpageTitle(webpageTitle);
        tenantSettings.SetPrivacyPolicy(privacyPolicy);
        tenantSettings.SetCompanyName(companyName);
        tenantSettings.SetBusinessRegistrationNumber(businessRegistrationNumber);
        tenantSettings.SetContactPhone(contactPhone);
        tenantSettings.SetCustomerServiceEmail(customerServiceEmail);
        tenantSettings.SetServiceHours(serviceHoursFrom, serviceHoursTo);

        await tenantSettingsRepository.UpdateAsync(tenantSettings);

        var tenant = await tenantRepository.FirstOrDefaultAsync(x => CurrentTenant != null && x.Id == CurrentTenant.Id)
            ?? throw new EntityNotFoundException(typeof(Tenant));

        tenant.RemoveProperty(Constant.Logo);
        tenant.SetProperty(Constant.Logo, logoUrl);

        tenant.RemoveProperty(Constant.BannerUrl);
        tenant.SetProperty(Constant.BannerUrl, bannerUrl);

        return tenantSettings;
    }

    private static void ValidateInputs(string? webpageTitle, string? privacyPolicy, string? companyName, string? businessRegistrationNumber,
        string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo,
        string? faviconUrl, string? logoUrl, string? bannerUrl)
    {
        Check.Length(webpageTitle, nameof(webpageTitle), maxLength: TenantSettingsConsts.MaxWebpageTitleLength);
        Check.Length(privacyPolicy, nameof(privacyPolicy), maxLength: TenantSettingsConsts.MaxPrivacyPolicyLength);
        Check.Length(companyName, nameof(companyName), maxLength: TenantSettingsConsts.MaxCompanyNameLength);
        Check.Length(businessRegistrationNumber, nameof(businessRegistrationNumber), maxLength: TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        Check.Length(contactPhone, nameof(contactPhone), maxLength: TenantSettingsConsts.MaxContactPhoneLength);
        Check.Length(customerServiceEmail, nameof(customerServiceEmail), maxLength: TenantSettingsConsts.MaxCustomerServiceEmailLength);

        //Check.NotNullOrWhiteSpace(faviconUrl, nameof(faviconUrl));
        //Check.NotNullOrWhiteSpace(logoUrl, nameof(logoUrl));
        //Check.NotNullOrWhiteSpace(bannerUrl, nameof(bannerUrl));

        if (serviceHoursFrom > serviceHoursTo)
        {
            throw new InvalidServiceHoursException();
        }
    }
}
