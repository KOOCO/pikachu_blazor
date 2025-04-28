using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Items.Dtos;

public class ItemWithItemTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ItemType ItemType { get; set; }
    public ItemDto? Item { get; set; }
    public SetItemDto? SetItem { get; set; }
    public Guid? SelectedItemAttribute { get; set; }
    public bool IsFirstLoad { get; set; }
    public float? Price { get; set; }
    public List<Guid> SelectedItemDetailIds { get; set; } = new();
    public IEnumerable<Guid> SelectedItemDetails { get; set; } = new List<Guid>();

    // Store Label & Price per selected ItemDetailId
    public Dictionary<Guid, (string Label, float Price)> ItemDetailsWithPrices { get; set; } = new();
    public ItemWithItemTypeDto()
    { 

    }
    public ItemWithItemTypeDto(Guid id, string name, ItemType itemType)
    {
        Id = id;
        Name = name;
        ItemType = itemType;
    }
}

