using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemDto : FullAuditedEntityDto<Guid>
{
    public Guid ShopCartId { get; set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    public int UnitPrice { get; set; }
    public string? ItemName { get; set; }
    public List<string> ItemSkus { get; set; }
}
