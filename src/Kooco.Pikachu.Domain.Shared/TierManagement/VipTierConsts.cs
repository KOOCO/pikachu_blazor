using System;
using System.Collections.Generic;
using System.IO;

namespace Kooco.Pikachu.TierManagement;

public static class VipTierConsts
{
    public const int MaxTierNameLength = 10;
    public static readonly List<Tier> TierOptions = [.. (Tier[])Enum.GetValues(typeof(Tier))];
    public static readonly List<VipTierCondition> TierConditionOptions = [.. (VipTierCondition[])Enum.GetValues(typeof(VipTierCondition))];
}

public class VipTierTemplateNames
{
    public const string Path = "wwwroot/EmailTemplates/tier-management/{0}_{1}.html";
    public const string FirstTier = "vip_first_tier";
    public const string TierUpgrade = "vip_tier_upgrade";
    public const string NextTier = "vip_next_tier";
    public const string RequiredOrders = "next_tier_required_orders";
    public const string RequiredAmount = "next_tier_required_amount";

    public static string Get(string templateName, string language = "zh")
    {
        return File.ReadAllText(string.Format(Path, templateName, language));
    }
}