using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement;

public class UpdateWebsiteSettingsDto
{
    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxNotificationBarLength)]
    public string NotificationBar { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxLogoNameLength)]
    public string LogoName { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxLogoUrlLength)]
    public string LogoUrl { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxStoreTitleLength)]
    public string StoreTitle { get; set; }

    [Required]
    public WebsiteTitleDisplayOptions? TitleDisplayOption { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxFacebookLength)]
    public string Facebook { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxInstagramLength)]
    public string Instagram { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxLineLength)]
    public string Line { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxReturnExchangePolicyLength)]
    public string ReturnExchangePolicy { get; set; }
}