using System;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCartListWithDetailsModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IdentityUser? User { get; set; }
    public string Email => User?.Email ?? string.Empty;
    public string UserName => User?.UserName ?? string.Empty;
    public string Gender => User?.GetProperty<string?>(Constant.Gender) ?? string.Empty;
    public Guid GroupBuyId { get; set; }
    public string? GroupBuyName { get; set; }
    public string? VipTier { get; set; }
    public string? MemberStatus { get; set; }
    public int TotalItems { get; set; }
    public int TotalAmount { get; set; }
}
