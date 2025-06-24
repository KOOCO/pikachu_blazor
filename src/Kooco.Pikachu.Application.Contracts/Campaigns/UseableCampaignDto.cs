using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class UseableCampaignDto : EntityDto<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid AllowedCampaignId { get; set; }
    public PromotionModule PromotionModule { get; set; }
}
