using System;

namespace Kooco.Pikachu.ShopCarts;

public class VerifyCartItemDto
{
    public Guid? ItemId { get; set; }
    public Guid? ItemDetailId { get; set; }
    public Guid? SetItemId { get; set; }
    public int SaleableQuantity { get; set; }
    public int SaleablePreOrderQuantity { get; set; }
}
