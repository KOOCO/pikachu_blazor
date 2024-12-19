using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class UpdateWebsiteSettingsModuleDto
{
    public Guid? Id { get; set; }
    public Guid? TenantId { get; set; }
    public Guid WebsiteSettingsId { get; set; }
    public int SortOrder { get; set; }
    public GroupBuyModuleType GroupBuyModuleType { get; set; }
    public string? AdditionalInfo { get; set; }
    public string? ProductGroupModuleTitle { get; set; }
    public string? ProductGroupModuleImageSize { get; set; }
    public int? ModuleNumber { get; set; }
    public List<UpdateWebsiteSettingsModuleItemDto> ModuleItems { get; set; } = [];
}
