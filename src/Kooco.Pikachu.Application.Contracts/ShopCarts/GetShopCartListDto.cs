using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShopCarts;

public class GetShopCartListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? UserId { get; set; }
    public bool IncludeDetails { get; set; } = true;
}