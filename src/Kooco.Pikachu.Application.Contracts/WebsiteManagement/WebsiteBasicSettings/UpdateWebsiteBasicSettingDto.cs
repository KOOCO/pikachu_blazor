using Kooco.Pikachu.EnumValues;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;

public class UpdateWebsiteBasicSettingDto
{
    public bool IsEnabled { get; set; }

    [Required]
    public WebsiteTitleDisplayOptions? TitleDisplayOption { get; set; }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxStoreTitleLength)]
    public string? StoreTitle { get; set; }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public string? LogoName { get; set; }

    [Required]
    public string? LogoUrl { get; set; }

    [Required]
    public GroupBuyTemplateType? TemplateType { get; set; }

    [Required]
    public ColorScheme? ColorScheme { get; set; }

    [Required]
    public string? ColorSchemeInvisible { get { return ColorScheme?.ToString(); } }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxColorLength)]
    public string? PrimaryColor { get; set; }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxColorLength)]
    public string? SecondaryColor { get; set; }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxColorLength)]
    public string? BackgroundColor { get; set; }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxColorLength)]
    public string? SecondaryBackgroundColor { get; set; }

    [Required]
    [MaxLength(WebsiteBasicSettingConsts.MaxColorLength)]
    public string? AlertColor { get; set; }
}