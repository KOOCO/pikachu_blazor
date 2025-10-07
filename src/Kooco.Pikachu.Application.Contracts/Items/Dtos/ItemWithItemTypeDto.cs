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
    public ItemStorageTemperature? Temperature { get; set; }

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
public class ItemDetailWithItemTypeDto
{
    public Guid Id { get; set; }
    public Guid? ItemDetailId { get; set; }
    public ItemType ItemType { get; set; }
    public string Sku { get; set; }
    public List<string> Attributes { get; set; }
    public ItemPricingDto Pricing { get; set; } = new();
}

public class ItemPricingDto
{
    public float Price { get; set; }
    public float? Available { get; set; }
    public float AvailableQuantity => Available ?? 0;
}
