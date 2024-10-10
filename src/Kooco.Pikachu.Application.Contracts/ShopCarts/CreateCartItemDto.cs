using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ShopCarts;

public class CreateCartItemDto
{
    [Required]
    public Guid? ItemId { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int UnitPrice { get; set; }

    [Required]
    public List<string>? ItemSkus { get; set; }
}