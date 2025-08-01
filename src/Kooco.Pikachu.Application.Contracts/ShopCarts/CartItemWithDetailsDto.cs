﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemWithDetailsDto
{
    public Guid? Id { get; set; }
    public Guid ShopCartId { get; set; }
    public ItemType ItemType { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? ItemDetailId { get; set; }
    public Guid? SetItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ItemDetail { get; set; }
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public int GroupBuyPrice { get; set; }
    public int SellingPrice { get; set; }
    public int Stock { get; set; }
    public int Amount => Quantity * GroupBuyPrice;
    public IEnumerable<CartItemDetailsDto> Details { get; set; } = [];
    public bool IsInvalid { get; set; }
}
