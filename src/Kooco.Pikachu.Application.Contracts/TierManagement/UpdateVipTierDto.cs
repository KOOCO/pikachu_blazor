using System;

namespace Kooco.Pikachu.TierManagement;

public class UpdateVipTierDto
{
    public Guid Id { get; set; }
    public Tier Tier { get; set; }
    public string? TierName { get; set; }
    public int OrdersAmount { get; set; }
    public int OrdersCount { get; set; }
}
