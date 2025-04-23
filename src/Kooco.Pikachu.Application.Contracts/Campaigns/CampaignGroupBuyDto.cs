using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignGroupBuyDto : EntityDto<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid GroupBuyId { get; set; }
}
