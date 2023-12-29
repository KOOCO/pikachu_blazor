using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.Items.Dtos;

public class ItemWithItemTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ItemType ItemType { get; set; }
    public ItemDto? Item { get; set; }
    public SetItemDto? SetItem { get; set; }
    public bool IsFirstLoad { get; set; }
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

