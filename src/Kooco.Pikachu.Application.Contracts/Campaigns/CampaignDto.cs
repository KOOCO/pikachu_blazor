﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Description { get; set; }
    public PromotionModule PromotionModule { get; set; }
    public bool ApplyToAllGroupBuys { get; set; }
    public bool? ApplyToAllProducts { get; set; }
    public CampaignUsagePolicy DiscountUsagePolicy { get; set; }
    public CampaignUsagePolicy ShoppingCreditUsagePolicy { get; set; }
    public CampaignUsagePolicy AddOnProductUsagePolicy { get; set; }
    public bool IsEnabled { get; set; }
    public Guid? TenantId { get; set; }
    public virtual ICollection<CampaignGroupBuyDto> GroupBuys { get; set; } = [];
    public virtual ICollection<CampaignProductDto> Products { get; set; } = [];
    public virtual CampaignDiscountDto Discount { get; set; }
    public virtual CampaignShoppingCreditDto ShoppingCredit { get; set; }
    public virtual CampaignAddOnProductDto AddOnProduct { get; set; }
    public IEnumerable<string> TargetAudience { get; set; }
    public virtual IEnumerable<UseableCampaignDto> AllowedDiscounts { get; set; } = [];
    public virtual IEnumerable<UseableCampaignDto> AllowedShoppingCredits { get; set; } = [];
    public virtual IEnumerable<UseableCampaignDto> AllowedAddOnProducts { get; set; } = [];
}