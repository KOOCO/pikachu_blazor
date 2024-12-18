using Kooco.Pikachu.EnumValues;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.WebsiteManagement;

public class CreateWebsiteSettingsDto
{
    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxPageTitleLength)]
    public string PageTitle { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxPageDescriptionLength)]
    public string PageDescription { get; set; }

    [Required]
    [MaxLength(WebsiteSettingsConsts.MaxPageLinkLength)]
    public string PageLink { get; set; }

    public bool SetAsHomePage { get; set; }

    [Required]
    public WebsitePageType? WebsitePageType { get; set; }

    [Required]
    public GroupBuyTemplateType? GroupBuyTemplateType { get; set; }

    [Required]
    public GroupBuyModuleType? SelectedGroupBuyModuleType { get; set; }

    public Guid? ProductCategoryId { get; set; }

    public string? ArticleHtml { get; set; }
}