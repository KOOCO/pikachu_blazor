using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShopCarts;

public class CartItemDto : FullAuditedEntityDto<Guid>
{
    public Guid ShopCartId { get; set; }
    public Guid? ItemId { get; set; }
    public int Quantity { get; set; }
    public int GroupBuyPrice { get; set; }
    public string? ItemName { get; set; }
    public Guid? ItemDetailId { get; set; }
    public Guid? SetItemId { get; set; }
}
