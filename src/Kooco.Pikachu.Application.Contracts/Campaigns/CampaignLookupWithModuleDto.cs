using System;

namespace Kooco.Pikachu.Campaigns;

public class CampaignLookupWithModuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public PromotionModule Module { get; set; }
}
