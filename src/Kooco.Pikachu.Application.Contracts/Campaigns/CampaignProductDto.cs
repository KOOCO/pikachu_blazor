using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignProductDto : EntityDto<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid ProductId { get; set; }
}
