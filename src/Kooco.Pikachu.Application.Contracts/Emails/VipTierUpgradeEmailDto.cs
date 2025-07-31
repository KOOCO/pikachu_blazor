using Kooco.Pikachu.TierManagement;

namespace Kooco.Pikachu.Emails;

public class VipTierUpgradeEmailDto
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public VipTierDto? CurrentTier { get; set; }
    public VipTierDto? PreviousTier { get; set; }
    public VipTierDto? NewTier { get; set; }
    public VipTierDto? NextTier { get; set; }
}