using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class UpdateWebsiteSettingsModuleItemDto
{
    public Guid? Id { get; set; }
    public Guid GroupBuyItemGroupId { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; }
    public ItemType ItemType { get; set; }
    public int SortOrder { get; set; }
    public string? DisplayText { get; set; }
    public int? ModuleNumber { get; set; }
}
