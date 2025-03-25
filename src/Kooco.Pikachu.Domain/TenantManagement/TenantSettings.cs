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
    public string? Description { get; private set; }
    public string? BusinessRegistrationNumber { get; private set; }
    public string? ContactPhone { get; private set; }
    public string? CustomerServiceEmail { get; private set; }
    public string? CustomerServiceContactPhone { get; set; }
    public DateTime? ServiceHoursFrom { get; private set; }
    public DateTime? ServiceHoursTo { get; private set; }

    public string? FacebookDisplayName { get; set; }
    public string? FacebookLink { get; set; }
    public string? InstagramDisplayName { get; set; }
    public string? InstagramLink { get; set; }
    public string? LineDisplayName { get; set; }
    public string? LineLink { get; set; }

    public bool GtmEnabled { get; set; }
    public string? GtmContainerId { get; set; }

    public Guid? TenantId { get; set; }

    [ForeignKey(nameof(TenantId))]
    public Tenant? Tenant { get; set; }

    public TenantSettings() { }

    public TenantSettings(Guid id) : base(id) { }

    public TenantSettings(
        Guid id,
        string? faviconUrl,
        string? webpageTitle,
        string? privacyPolicy,
        string? companyName,
        string? businessRegistrationNumber,
        string? contactPhone,
        string? customerServiceEmail,
        string? customerServiceContactPhone,
        DateTime? serviceHoursFrom,
        DateTime? serviceHoursTo,
        string? facebookLink,
        string? instagramLink,
        string? lineLink,
        string? facebookTitle,
        string? instagramTitle,
        string? lineTitle,
        bool gtmEnabled,
        string? gtmContainerId,
        string? description
        ) : base(id)
    {
        SetFaviconUrl(faviconUrl);
        SetWebpageTitle(webpageTitle);
        SetPrivacyPolicy(privacyPolicy);
        SetCompanyName(companyName);
        SetBusinessRegistrationNumber(businessRegistrationNumber);
        SetContactPhone(contactPhone);
        SetCustomerServiceEmail(customerServiceEmail);
        SetCustomerServiceContactPhone(customerServiceContactPhone);
        SetServiceHours(serviceHoursFrom, serviceHoursTo);
        SetSocials(facebookTitle, facebookLink, instagramTitle,instagramLink, lineTitle,lineLink);
        SetGtm(gtmEnabled, gtmContainerId);
        SetDescription(description);
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

    public TenantSettings SetDescription(string? description)
    {
        Description = Check.Length(description, nameof(Description), TenantSettingsConsts.MaxDescriptionLength);
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

    public TenantSettings SetCustomerServiceContactPhone(string? customerServiceContactPhone)
    {
        CustomerServiceContactPhone = Check.NotNullOrWhiteSpace(customerServiceContactPhone, nameof(CustomerServiceContactPhone), TenantSettingsConsts.MaxContactPhoneLength);
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

    public TenantSettings SetSocials(string? facebookTitle,string? facebookUrl, string? instagramTitle, string? instagramUrl,string? lineTitle,string? lineUrl)
    {
        FacebookLink = facebookUrl.IsEmptyOrValidUrl() ? facebookUrl : throw new InvalidUrlException(nameof(facebookUrl));
        InstagramLink = instagramUrl.IsEmptyOrValidUrl() ? instagramUrl : throw new InvalidUrlException(nameof(instagramUrl));
        LineLink = lineUrl.IsEmptyOrValidUrl() ? lineUrl : throw new InvalidUrlException(nameof(lineUrl));
        FacebookDisplayName = facebookTitle;
        InstagramDisplayName = instagramTitle;
        LineDisplayName = lineTitle;
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
