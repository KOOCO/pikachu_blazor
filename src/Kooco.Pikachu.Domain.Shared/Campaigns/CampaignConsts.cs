using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.Campaigns;

public class CampaignConsts
{
    public class TargetAudience
    {
        public const string All = "All";
        public const string AllMembers = "AllMembers";

        public static readonly List<string> Values = [All, AllMembers];
    }
}
