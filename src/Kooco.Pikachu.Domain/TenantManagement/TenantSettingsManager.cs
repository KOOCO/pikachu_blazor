using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.TenantManagement;

public class TenantSettingsManager(IRepository<TenantSettings, Guid> tenantSettingsRepository) : DomainService
{
    public async Task<TenantSettings> CreateAsync(string? faviconUrl, string? webpageTitle, string? privacyPolicy, string? companyName,
        string? businessRegistrationNumber, string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        ValidateInputs(faviconUrl, webpageTitle, privacyPolicy, companyName, businessRegistrationNumber,
            contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo);

        if (await tenantSettingsRepository.AnyAsync())
        {
            throw new TenantSettingsAlreadyExistException();
        }

        var tenantSettings = new TenantSettings(GuidGenerator.Create(), faviconUrl, webpageTitle, privacyPolicy, companyName,
            businessRegistrationNumber, contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo);

        await tenantSettingsRepository.InsertAsync(tenantSettings);
        return tenantSettings;
    }

    public async Task<TenantSettings> UpdateAsync(TenantSettings tenantSettings, string? faviconUrl, string? webpageTitle, string? privacyPolicy, string? companyName,
        string? businessRegistrationNumber, string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        Check.NotNull(tenantSettings, nameof(tenantSettings));
        ValidateInputs(faviconUrl, webpageTitle, privacyPolicy, companyName, businessRegistrationNumber,
            contactPhone, customerServiceEmail, serviceHoursFrom, serviceHoursTo);

        if (faviconUrl != tenantSettings.FaviconUrl)
        {
            tenantSettings.SetFaviconUrl(faviconUrl);
        }

        if (webpageTitle != tenantSettings.WebpageTitle)
        {
            tenantSettings.SetWebpageTitle(webpageTitle);
        }

        if (privacyPolicy != tenantSettings.PrivacyPolicy)
        {
            tenantSettings.SetPrivacyPolicy(privacyPolicy);
        }

        if (companyName != tenantSettings.CompanyName)
        {
            tenantSettings.SetCompanyName(companyName);
        }

        if (businessRegistrationNumber != tenantSettings.BusinessRegistrationNumber)
        {
            tenantSettings.SetBusinessRegistrationNumber(businessRegistrationNumber);
        }

        if (contactPhone != tenantSettings.ContactPhone)
        {
            tenantSettings.SetContactPhone(contactPhone);
        }

        if (customerServiceEmail != tenantSettings.CustomerServiceEmail)
        {
            tenantSettings.SetCustomerServiceEmail(customerServiceEmail);
        }

        if (serviceHoursFrom != tenantSettings.ServiceHoursFrom || serviceHoursTo != tenantSettings.ServiceHoursTo)
        {
            tenantSettings.SetServiceHours(serviceHoursFrom, serviceHoursTo);
        }
        await tenantSettingsRepository.UpdateAsync(tenantSettings);
        return tenantSettings;
    }

    private static void ValidateInputs(string? faviconUrl, string? webpageTitle, string? privacyPolicy, string? companyName,
        string? businessRegistrationNumber, string? contactPhone, string? customerServiceEmail, DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        Check.Length(faviconUrl, nameof(faviconUrl), maxLength: TenantSettingsConsts.MaxFaviconUrlLength);
        Check.Length(webpageTitle, nameof(webpageTitle), maxLength: TenantSettingsConsts.MaxWebpageTitleLength);
        Check.Length(privacyPolicy, nameof(privacyPolicy), maxLength: TenantSettingsConsts.MaxPrivacyPolicyLength);
        Check.Length(companyName, nameof(companyName), maxLength: TenantSettingsConsts.MaxCompanyNameLength);
        Check.Length(businessRegistrationNumber, nameof(businessRegistrationNumber), maxLength: TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        Check.Length(contactPhone, nameof(contactPhone), maxLength: TenantSettingsConsts.MaxContactPhoneLength);
        Check.Length(customerServiceEmail, nameof(customerServiceEmail), maxLength: TenantSettingsConsts.MaxCustomerServiceEmailLength);

        if (serviceHoursFrom > serviceHoursTo)
        {
            throw new InvalidServiceHoursException();
        }
    }
}
