using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.TierManagement;

public static class VipTierConsts
{
    public const int MaxTierNameLength = 10;
    public static readonly List<Tier> TierOptions = [.. (Tier[])Enum.GetValues(typeof(Tier))];
    public static readonly List<VipTierCondition> TierConditionOptions = [.. (VipTierCondition[])Enum.GetValues(typeof(VipTierCondition))];
}
