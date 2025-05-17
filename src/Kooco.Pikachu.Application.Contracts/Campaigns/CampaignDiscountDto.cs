using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Campaigns;

public class CampaignDiscountDto : EntityDto<Guid>
{
    public Guid CampaignId { get; set; }
    public bool IsDiscountCodeRequired { get; set; }
    public string? DiscountCode { get; set; }
    public int AvailableQuantity { get; set; }
    public int MaximumUsePerPerson { get; set; }
    public DiscountMethod DiscountMethod { get; set; }
    public int? MinimumSpendAmount { get; set; }
    public bool? ApplyToAllShippingMethods { get; set; }
    public DiscountType? DiscountType { get; set; }
    public int? DiscountAmount { get; set; }
    public int? DiscountPercentage { get; set; }
    public double? CapAmount { get; set; }
    public IEnumerable<DeliveryMethod> DeliveryMethods { get; set; }
}
