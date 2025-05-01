using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShopCarts;

public class GetShopCartListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? UserId { get; set; }
    public Guid? GroupBuyId { get; set; }
    public int? MinItems { get; set; }
    public int? MaxItems { get; set; }
    public int? MinAmount { get; set; }
    public int? MaxAmount { get; set; }
    public string? VipTier { get; set; }
    public string? MemberStatus { get; set; }
    public bool IncludeDetails { get; set; } = true;
}