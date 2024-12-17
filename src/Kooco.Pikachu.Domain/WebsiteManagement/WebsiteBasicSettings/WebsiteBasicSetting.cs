using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;

public class WebsiteBasicSetting : FullAuditedEntity<Guid>, IMultiTenant
{
    public bool IsEnabled { get; set; }
    public WebsiteTitleDisplayOptions TitleDisplayOption { get; set; }
    public string? StoreTitle { get; private set; }
    public string? Description { get; private set; }
    public string? LogoName { get; set; }
    public string? LogoUrl { get; set; }
    public GroupBuyTemplateType TemplateType { get; set; }
    public ColorScheme ColorScheme { get; set; }
    public string? PrimaryColor { get; set; }
    public string? SecondaryColor { get; set; }
    public string? BackgroundColor { get; set; }
    public string? SecondaryBackgroundColor { get; set; }
    public string? AlertColor { get; set; }
    public Guid? TenantId { get; set; }


    public WebsiteBasicSetting(
        Guid id,
        bool isEnabled,
        WebsiteTitleDisplayOptions titleDisplayOption,
        string storeTitle,
        string description,
        string logoName,
        string logoUrl,
        GroupBuyTemplateType templateType,
        ColorScheme colorScheme,
        string primaryColor,
        string secondaryColor,
        string backgroundColor,
        string secondaryBackgroundColor,
        string alertColor
        ) : base(id)
    {
        SetIsEnabled(isEnabled);
        SetStoreTitle(storeTitle);
        SetDescription(description);
        SetLogo(logoName, logoUrl);
        SetColorScheme(colorScheme, primaryColor, secondaryColor, backgroundColor, secondaryBackgroundColor, alertColor);
        TitleDisplayOption = titleDisplayOption;
        TemplateType = templateType;
    }

    public void SetIsEnabled(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }

    public void SetStoreTitle(string storeTitle)
    {
        StoreTitle = Check.NotNullOrWhiteSpace(storeTitle, nameof(StoreTitle), maxLength: WebsiteBasicSettingConsts.MaxStoreTitleLength);
    }

    public void SetDescription(string description)
    {
        Description = Check.NotNullOrWhiteSpace(description, nameof(Description), maxLength: WebsiteBasicSettingConsts.MaxDescriptionLength);
    }

    public void SetLogo(string logoName, string logoUrl)
    {
        LogoName = logoName;
        LogoUrl = logoUrl;
    }

    public void SetColorScheme(ColorScheme colorScheme, string primaryColor, string secondaryColor, string backgroundColor,
        string secondaryBackgroundColor, string alertColor)
    {
        ColorScheme = colorScheme;
        PrimaryColor = Check.NotNullOrWhiteSpace(primaryColor, nameof(PrimaryColor), maxLength: WebsiteBasicSettingConsts.MaxColorLength);
        SecondaryColor = Check.NotNullOrWhiteSpace(secondaryColor, nameof(SecondaryColor), maxLength: WebsiteBasicSettingConsts.MaxColorLength);
        BackgroundColor = Check.NotNullOrWhiteSpace(backgroundColor, nameof(BackgroundColor), maxLength: WebsiteBasicSettingConsts.MaxColorLength);
        SecondaryBackgroundColor = Check.NotNullOrWhiteSpace(secondaryBackgroundColor, nameof(SecondaryBackgroundColor), maxLength: WebsiteBasicSettingConsts.MaxColorLength);
        AlertColor = Check.NotNullOrWhiteSpace(alertColor, nameof(AlertColor), maxLength: WebsiteBasicSettingConsts.MaxColorLength);
    }
}
