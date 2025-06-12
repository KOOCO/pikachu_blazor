using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ShopCarts;

public class CreateCartItemDto
{
    [Range(int.MinValue, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int GroupBuyPrice { get; set; }
    [Range(0, int.MaxValue)]
    public int SellingPrice { get; set; }
    public Guid? ItemId { get; set; }
    public Guid? ItemDetailId { get; set; }
    public Guid? SetItemId { get; set; }
}