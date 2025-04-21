using System.Collections.Generic;

namespace Kooco.Pikachu.Campaigns;

public class CampaignConsts
{
    public const int MaxNameLength = 64;
    public const int MaxDescriptionLength = 2000;

    public class TargetAudience
    {
        public const string All = "All";
        public const string AllMembers = "AllMembers";

        public static readonly List<string> Values = [All, AllMembers];
    }
}
