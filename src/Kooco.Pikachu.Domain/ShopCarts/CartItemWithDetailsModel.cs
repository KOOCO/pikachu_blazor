using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemWithDetailsModel
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
}
