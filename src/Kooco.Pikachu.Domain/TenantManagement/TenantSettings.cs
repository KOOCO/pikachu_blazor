using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.TenantManagement;

public class TenantSettings : FullAuditedEntity<Guid>, IMultiTenant
{
    public string? FaviconUrl { get; private set; }
    public string? WebpageTitle { get; private set; }
    public string? PrivacyPolicy { get; private set; }
    public string? CompanyName { get; private set; }
    public string? BusinessRegistrationNumber { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? CustomerServiceEmail { get; private set; }
    public DateTime? ServiceHoursFrom { get; private set; }
    public DateTime? ServiceHoursTo { get; private set; }

    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant? Tenant { get; set; }

    public TenantSettings(
        Guid id,
        string? faviconUrl,
        string? webpageTitle,
        string? privacyPolicy,
        string? companyName,
        string? businessRegistrationNumber,
        string? contactPhone,
        string? customerServiceEmail,
        DateTime? serviceHoursFrom,
        DateTime? serviceHoursTo
        ) : base(id)
    {
        SetFaviconUrl(faviconUrl);
        SetWebpageTitle(webpageTitle);
        SetPrivacyPolicy(privacyPolicy);
        SetCompanyName(companyName);
        SetBusinessRegistrationNumber(businessRegistrationNumber);
        SetContactPhone(contactPhone);
        SetCustomerServiceEmail(customerServiceEmail);
        SetServiceHours(serviceHoursFrom, serviceHoursTo);
    }

    public TenantSettings SetFaviconUrl(string? faviconUrl)
    {
        FaviconUrl = Check.Length(faviconUrl, nameof(FaviconUrl), TenantSettingsConsts.MaxFaviconUrlLength);
        return this;
    }

    public TenantSettings SetWebpageTitle(string? webpageTitle)
    {
        WebpageTitle = Check.Length(webpageTitle, nameof(WebpageTitle), TenantSettingsConsts.MaxWebpageTitleLength);
        return this;
    }

    public TenantSettings SetPrivacyPolicy(string? privacyPolicy)
    {
        PrivacyPolicy = Check.Length(privacyPolicy, nameof(PrivacyPolicy), TenantSettingsConsts.MaxPrivacyPolicyLength);
        return this;
    }

    public TenantSettings SetCompanyName(string? companyName)
    {
        CompanyName = Check.Length(companyName, nameof(CompanyName), TenantSettingsConsts.MaxCompanyNameLength);
        return this;
    }

    public TenantSettings SetBusinessRegistrationNumber(string? businessRegistrationNumber)
    {
        BusinessRegistrationNumber = Check.Length(businessRegistrationNumber, nameof(BusinessRegistrationNumber), TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        return this;
    }

    public TenantSettings SetContactPhone(string? contactPhone)
    {
        ContactPhone = Check.Length(contactPhone, nameof(ContactPhone), TenantSettingsConsts.MaxContactPhoneLength);
        return this;
    }

    public TenantSettings SetCustomerServiceEmail(string? customerServiceEmail)
    {
        CustomerServiceEmail = Check.Length(customerServiceEmail, nameof(CustomerServiceEmail), TenantSettingsConsts.MaxCustomerServiceEmailLength);
        return this;
    }

    public TenantSettings SetServiceHours(DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        if (serviceHoursFrom >= serviceHoursTo)
        {
            throw new InvalidServiceHoursException();
        }

        ServiceHoursFrom = serviceHoursFrom;
        ServiceHoursTo = serviceHoursTo;
        return this;
    }
}
