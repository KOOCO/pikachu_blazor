using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCartDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public Guid GroupBuyId { get; set; }
    public string? UserName { get; set; }
    public List<CartItemDto> CartItems { get; set; } = [];
}