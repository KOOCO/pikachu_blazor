using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemWithDetailsDto
{
    public Guid Id { get; set; }
    public Guid ShopCartId { get; set; }
    public ItemType ItemType { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? SetItemId { get; set; }
    public string? ItemName { get; set; }
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
    public int Stock { get; set; }
    public int Amount => Quantity * UnitPrice;
}
