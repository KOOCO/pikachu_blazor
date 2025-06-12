using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Items.Dtos;

public class ItemWithDetailsDto
{
    public Guid Id { get; set; }
    public ItemType ItemType { get; set; }
    public string? ItemName { get; set; }
    public string? Image { get; set; }
    public int? Stock { get; set; }
    public int GroupBuyPrice { get; set; }
    public int SellingPrice { get; set; }

    public IEnumerable<CartItemDetailsDto> Details { get; set; }
}

public class CartItemDetailsDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int? Stock { get; set; }
    public float GroupBuyPrice { get; set; }
    public float SellingPrice { get; set; }
}
