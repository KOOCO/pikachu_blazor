using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemWithDetailsModel
{
    public Guid Id { get; set; }
    public Guid ShopCartId { get; set; }
    public ItemType ItemType { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? ItemDetailId { get; set; }
    public Guid? SetItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ItemDetail { get; set; }
    public string? Image { get; set; }
    public int Quantity { get; set; }
    public float GroupBuyPrice { get; set; }
    public float SellingPrice { get; set; }
    public int Stock { get; set; }
}
