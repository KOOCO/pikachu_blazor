using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using System;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyItemGroupDetailsDto
{
    public Guid Id { get; set; }
    public Guid GroupBuyItemGroupId { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; } 
    public int SortOrder { get; set; }
    public ItemDto Item { get; set; }
    public SetItemDto SetItem { get; set; }
    public ItemType ItemType { get; set; }
    public string? DisplayText { get; set; }
    public int? ModuleNumber { get; set; }
}
