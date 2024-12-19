using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement;

public class WebsiteSettingsDto : FullAuditedEntityDto<Guid>
{
    public string? PageTitle { get; private set; }
    public string? PageDescription { get; private set; }
    public string? PageLink { get; private set; }
    public bool SetAsHomePage { get; set; }
    public WebsitePageType PageType { get; set; }
    public GroupBuyTemplateType? TemplateType { get; set; }
    public GroupBuyModuleType? GroupBuyModuleType { get; set; }
    public Guid? ProductCategoryId { get; set; }
    public string? ArticleHtml { get; set; }
    public Guid? TenantId { get; set; }
}