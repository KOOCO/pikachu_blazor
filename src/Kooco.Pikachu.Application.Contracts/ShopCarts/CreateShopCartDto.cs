using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.ShopCarts;

public class CreateShopCartDto
{
    [Required]
    public Guid? UserId { get; set; }

    [Required]
    public Guid GroupBuyId { get; set; }

    [Required]
    public List<CreateCartItemDto> CartItems { get; set; }
}