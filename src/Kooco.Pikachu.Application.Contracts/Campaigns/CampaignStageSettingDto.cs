using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignStageSettingDto : EntityDto<Guid>
{
    public Guid ShoppingCreditId { get; set; }
    public int Spend { get; set; }
    public int PointsToReceive { get; set; }
}
