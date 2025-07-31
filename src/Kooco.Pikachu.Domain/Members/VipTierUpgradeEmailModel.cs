using Kooco.Pikachu.TierManagement;
using System;

namespace Kooco.Pikachu.Members;

public class VipTierUpgradeEmailModel
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public VipTier? CurrentTier { get; set; }
    public VipTier? PreviousTier { get; set; }
    public VipTier? NewTier { get; set; }
    public VipTier? NextTier { get; set; }
}
