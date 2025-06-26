using Kooco.Pikachu.Items;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Campaigns;

public class CampaignAddOnProduct : Entity<Guid>
{
    public Guid CampaignId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ItemDetailId { get; set; }
    public int ProductAmount { get; private set; }
    public int LimitPerOrder { get; private set; }
    public bool IsUnlimitedQuantity { get; private set; }
    public int? AvailableQuantity { get; private set; }
    public AddOnDisplayPrice DisplayPrice { get; set; }
    public CampaignSpendCondition SpendCondition { get; private set; }
    public int? Threshold { get; private set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [ForeignKey(nameof(ProductId))]
    public virtual Item Product { get; set; }

    private CampaignAddOnProduct() { }

    internal CampaignAddOnProduct(
        Guid id,
        Guid campaignId,
        Guid productId,
        Guid itemDetailId,
        int productAmount,
        int limitPerOrder,
        bool isUnlimitedQuantity,
        int? availableQuantity,
        AddOnDisplayPrice displayPrice,
        CampaignSpendCondition spendCondition,
        int? threshold
        ) : base(id)
    {
        CampaignId = campaignId;
        ProductId = productId;
        ItemDetailId = itemDetailId;
        SetProduct(productAmount);
        SetOrderLimit(limitPerOrder);
        SetQuantity(isUnlimitedQuantity, availableQuantity);
        DisplayPrice = displayPrice;
        SetSpendCondition(spendCondition, threshold);
    }

    public void SetProduct(int productAmount)
    {
        ProductAmount = Check.Range(productAmount, nameof(ProductAmount), 0);
    }

    public void SetOrderLimit(int limitPerOrder)
    {
        LimitPerOrder = Check.Range(limitPerOrder, nameof(LimitPerOrder), 0);
    }

    public void SetQuantity(bool isUnlimited, int? availableQuantity)
    {
        IsUnlimitedQuantity = isUnlimited;

        if (!IsUnlimitedQuantity)
        {
            Check.NotNull(availableQuantity, nameof(AvailableQuantity));
            AvailableQuantity = Check.Range(availableQuantity.Value, nameof(AvailableQuantity), 0);
        }
        else
        {
            AvailableQuantity = null;
        }
    }

    public void DeductAvailableQuantity(int amount)
    {
        if (!IsUnlimitedQuantity)
        {
            var deductedAvailableQuantity = AvailableQuantity - amount;
            SetQuantity(IsUnlimitedQuantity, deductedAvailableQuantity);
        }
    }

    public void SetSpendCondition(CampaignSpendCondition condition, int? threshold)
    {
        SpendCondition = condition;

        if (condition == CampaignSpendCondition.MustMeetSpecifiedThreshold)
        {
            Check.NotNull(threshold, nameof(Threshold));
            Threshold = Check.Range(threshold.Value, nameof(Threshold), 0);
        }
        else
        {
            Threshold = null;
        }
    }
}
