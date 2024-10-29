using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Kooco.Pikachu.Extensions;

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

    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? Line { get; set; }

    public bool GtmEnabled { get; set; }
    public string? GtmContainerId { get; set; }

    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant? Tenant { get; set; }

    public TenantSettings() { }

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
        DateTime? serviceHoursTo,
        string? facebook,
        string? instagram,
        string? line,
        bool gtmEnabled,
        string? gtmContainerId
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
        SetSocials(facebook, instagram, line);
        SetGtm(gtmEnabled, gtmContainerId);
    }

    public TenantSettings SetFaviconUrl(string? faviconUrl)
    {
        FaviconUrl = faviconUrl;
        return this;
    }

    public TenantSettings SetWebpageTitle(string? webpageTitle)
    {
        WebpageTitle = Check.NotNullOrWhiteSpace(webpageTitle, nameof(WebpageTitle), TenantSettingsConsts.MaxWebpageTitleLength);
        return this;
    }

    public TenantSettings SetPrivacyPolicy(string? privacyPolicy)
    {
        PrivacyPolicy = Check.NotNullOrWhiteSpace(privacyPolicy, nameof(PrivacyPolicy));
        return this;
    }

    public TenantSettings SetCompanyName(string? companyName)
    {
        CompanyName = Check.NotNullOrWhiteSpace(companyName, nameof(CompanyName), TenantSettingsConsts.MaxCompanyNameLength);
        return this;
    }

    public TenantSettings SetBusinessRegistrationNumber(string? businessRegistrationNumber)
    {
        BusinessRegistrationNumber = Check.NotNullOrWhiteSpace(businessRegistrationNumber, nameof(BusinessRegistrationNumber), TenantSettingsConsts.MaxBusinessRegistrationNumberLength);
        return this;
    }

    public TenantSettings SetContactPhone(string? contactPhone)
    {
        ContactPhone = Check.NotNullOrWhiteSpace(contactPhone, nameof(ContactPhone), TenantSettingsConsts.MaxContactPhoneLength);
        return this;
    }

    public TenantSettings SetCustomerServiceEmail(string? customerServiceEmail)
    {
        CustomerServiceEmail = Check.NotNullOrWhiteSpace(customerServiceEmail, nameof(CustomerServiceEmail), TenantSettingsConsts.MaxCustomerServiceEmailLength);
        return this;
    }

    public TenantSettings SetServiceHours(DateTime? serviceHoursFrom, DateTime? serviceHoursTo)
    {
        if (serviceHoursFrom?.TimeOfDay >= serviceHoursTo?.TimeOfDay)
        {
            throw new InvalidServiceHoursException();
        }

        ServiceHoursFrom = serviceHoursFrom;
        ServiceHoursTo = serviceHoursTo;
        return this;
    }

    public TenantSettings SetSocials(string? facebook, string? instagram, string? line)
    {
        Facebook = facebook.IsEmptyOrValidUrl() ? facebook : throw new InvalidUrlException(nameof(facebook));
        Instagram = instagram.IsEmptyOrValidUrl() ? instagram : throw new InvalidUrlException(nameof(instagram));
        Line = line.IsEmptyOrValidUrl() ? line : throw new InvalidUrlException(nameof(line));
        return this;
    }

    public TenantSettings SetGtm(bool gtmEnabled, string? gtmContainerId)
    {
        GtmEnabled = gtmEnabled;
        if (GtmEnabled)
        {
            GtmContainerId = Check.NotNullOrWhiteSpace(gtmContainerId, nameof(GtmContainerId), maxLength: TenantSettingsConsts.MaxGtmContainerIdLength);
        }
        else
        {
            GtmContainerId = gtmContainerId;
        }
        return this;
    }
}
