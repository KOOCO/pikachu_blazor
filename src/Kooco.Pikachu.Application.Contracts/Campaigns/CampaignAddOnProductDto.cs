using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignAddOnProductDto : EntityDto<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ItemDetailId { get; set; }
    public int ProductAmount { get; set; }
    public int LimitPerOrder { get; set; }
    public bool IsUnlimitedQuantity { get; set; }
    public int? AvailableQuantity { get; set; }
    public AddOnDisplayPrice DisplayPrice { get; set; }
    public CampaignSpendCondition SpendCondition { get; set; }
    public int? Threshold { get; set; }
}
