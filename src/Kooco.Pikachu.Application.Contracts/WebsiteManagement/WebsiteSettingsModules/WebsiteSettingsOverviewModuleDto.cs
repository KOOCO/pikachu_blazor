using System;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsOverviewModuleDto
{
    public Guid Id { get; set; }
    public Guid WebsiteSettingsId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? SubTitle { get; set; }
    public string? BodyText { get; set; }
    public bool IsButtonEnable { get; set; }
    public string? ButtonText { get; set; }
    public string? ButtonLink { get; set; }
    public Guid? TenantId { get; set; }
}
