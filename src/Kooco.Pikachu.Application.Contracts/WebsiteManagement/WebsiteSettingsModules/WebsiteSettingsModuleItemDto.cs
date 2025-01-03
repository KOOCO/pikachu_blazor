using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using System;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteSettingsModules;

public class WebsiteSettingsModuleItemDto
{
    public Guid Id { get; set; }
    public Guid WebsiteSettingsModuleId { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; }
    public ItemType ItemType { get; set; }
    public int SortOrder { get; set; }
    public string? DisplayText { get; set; }
    public int? ModuleNumber { get; set; }
    public ItemDto? Item { get; set; }
    public SetItemDto? SetItem { get; set; }
}
