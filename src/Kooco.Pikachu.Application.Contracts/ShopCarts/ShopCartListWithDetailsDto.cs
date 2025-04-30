using System;

namespace Kooco.Pikachu.ShopCarts;

public class ShopCartListWithDetailsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Gender { get; set; }
    public Guid GroupBuyId { get; set; }
    public string? GroupBuyName { get; set; }
    public string? VipTier { get; set; }
    public string? MemberStatus { get; set; }
    public int TotalItems { get; set; }
    public int TotalAmount { get; set; }
    public int Stock { get; set; }
}
