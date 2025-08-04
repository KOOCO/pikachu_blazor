using Kooco.Pikachu.TierManagement;

namespace Kooco.Pikachu.Members;

public class VipTierUpgradeEmailModel
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public VipTier? PreviousTier { get; set; }
    public VipTier? NewTier { get; set; }
    public VipTier? NextTier { get; set; }
    public long RequiredOrders { get; set; }
    public decimal RequiredAmount { get; set; }
}
