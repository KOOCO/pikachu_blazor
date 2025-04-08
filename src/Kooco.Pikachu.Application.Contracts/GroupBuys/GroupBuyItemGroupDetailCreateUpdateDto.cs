using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Images;
using System;

namespace Kooco.Pikachu.GroupBuys;

public class GroupBuyItemGroupDetailCreateUpdateDto
{
    public Guid? Id { get; set; }
    public Guid GroupBuyItemGroupId { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; }
    public ItemType ItemType { get; set; }
    public int SortOrder { get; set; }
    public string? DisplayText { get; set; }
    public int? ModuleNumber { get; set; }
    public Guid? ItemDetailId { get; set; }
    public float Price { get; set; }
}
