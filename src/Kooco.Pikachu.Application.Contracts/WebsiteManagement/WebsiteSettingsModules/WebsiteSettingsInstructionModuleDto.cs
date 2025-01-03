using System;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsInstructionModuleDto
{
    public Guid Id { get; set; }
    public Guid WebsiteSettingsId { get; set; }
    public string Title { get; set; }
    public string Image { get; set; }
    public string? BodyText { get; set; }
    public Guid? TenantId { get; set; }
}
