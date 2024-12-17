using Kooco.Pikachu.EnumValues;
using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;

public class WebsiteBasicSettingDto : EntityDto<Guid>
{
    public bool IsEnabled { get; set; }
    public WebsiteTitleDisplayOptions TitleDisplayOption { get; set; }
    public string? StoreTitle { get; set; }
    public string? Description { get; set; }
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
}