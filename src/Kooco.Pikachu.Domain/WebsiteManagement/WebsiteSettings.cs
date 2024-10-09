using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement;

public class WebsiteSettings : FullAuditedEntity<Guid>, IMultiTenant
{
    public string NotificationBar { get; private set; }
    public string LogoName { get; private set; }
    public string LogoUrl { get; private set; }
    public string StoreTitle { get; private set; }
    public WebsiteTitleDisplayOptions TitleDisplayOption { get; private set; }
    public string Facebook { get; private set; }
    public string Instagram { get; private set; }
    public string Line { get; private set; }
    public string ReturnExchangePolicy { get; private set; }
    public Guid? TenantId { get; set; }

    public WebsiteSettings(
        Guid id,
        string notificationBar,
        string logoName,
        string logoUrl,
        string storeTitle,
        WebsiteTitleDisplayOptions titleDisplayOption,
        string facebook,
        string instagram,
        string line,
        string returnExchangePolicy
        ) : base(id)
    {
        SetNotificationBar(notificationBar);
        SetLogoName(logoName);
        SetLogoUrl(logoUrl);
        SetStoreTitle(storeTitle);
        SetFacebook(facebook);
        SetInstagram(instagram);
        SetLine(line);
        SetReturnExchangePolicy(returnExchangePolicy);
        SetTitleDisplayOption(titleDisplayOption);
    }

    public WebsiteSettings SetNotificationBar(string notificationBar)
    {
        NotificationBar = Check.NotNullOrWhiteSpace(notificationBar, nameof(NotificationBar), maxLength: WebsiteSettingsConsts.MaxNotificationBarLength);
        return this;
    }

    public WebsiteSettings SetLogoName(string logoName)
    {
        LogoName = Check.NotNullOrWhiteSpace(logoName, nameof(LogoName), maxLength: WebsiteSettingsConsts.MaxLogoNameLength);
        return this;
    }

    public WebsiteSettings SetLogoUrl(string logoUrl)
    {
        LogoUrl = Check.NotNullOrWhiteSpace(logoUrl, nameof(LogoUrl), maxLength: WebsiteSettingsConsts.MaxLogoUrlLength);
        return this;
    }

    public WebsiteSettings SetStoreTitle(string storeTitle)
    {
        StoreTitle = Check.NotNullOrWhiteSpace(storeTitle, nameof(StoreTitle), maxLength: WebsiteSettingsConsts.MaxStoreTitleLength);
        return this;
    }

    public WebsiteSettings SetFacebook(string facebook)
    {
        Facebook = Check.NotNullOrWhiteSpace(facebook, nameof(Facebook), maxLength: WebsiteSettingsConsts.MaxFacebookLength);
        return this;
    }

    public WebsiteSettings SetInstagram(string instagram)
    {
        Instagram = Check.NotNullOrWhiteSpace(instagram, nameof(Instagram), maxLength: WebsiteSettingsConsts.MaxInstagramLength);
        return this;
    }

    public WebsiteSettings SetLine(string line)
    {
        Line = Check.NotNullOrWhiteSpace(line, nameof(Line), maxLength: WebsiteSettingsConsts.MaxLineLength);
        return this;
    }

    public WebsiteSettings SetReturnExchangePolicy(string returnExchangePolicy)
    {
        ReturnExchangePolicy = Check.NotNullOrWhiteSpace(returnExchangePolicy, nameof(ReturnExchangePolicy), maxLength: WebsiteSettingsConsts.MaxReturnExchangePolicyLength);
        return this;
    }

    public WebsiteSettings SetTitleDisplayOption(WebsiteTitleDisplayOptions titleDisplayOption)
    {
        if (!Enum.IsDefined(typeof(WebsiteTitleDisplayOptions), titleDisplayOption))
        {
            throw new InvalidEnumValueException(nameof(TitleDisplayOption));
        }

        TitleDisplayOption = titleDisplayOption;
        return this;
    }
}
