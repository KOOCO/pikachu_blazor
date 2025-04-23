using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Campaigns;

public class CampaignDiscount : Entity<Guid>
{
    public Guid CampaignId { get; set; }
    public bool IsDiscountCodeRequired { get; private set; }
    public string? DiscountCode { get; private set; }
    public int AvailableQuantity { get; private set; }
    public int MaximumUsePerPerson { get; private set; }
    public DiscountMethod DiscountMethod { get; private set; }
    public int? MinimumSpendAmount { get; private set; }
    public bool? ApplyToAllShippingMethods { get; private set; }
    public string? DeliveryMethodsJson { get; private set; }
    public DiscountType? DiscountType { get; private set; }
    public int? DiscountAmount { get; private set; }
    public int? DiscountPercentage { get; private set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    [NotMapped]
    public IEnumerable<DeliveryMethod> DeliveryMethods
    {
        get
        {
            return !string.IsNullOrWhiteSpace(DeliveryMethodsJson)
                ? JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsJson)!
                : [];
        }
    }

    private CampaignDiscount() { }

    internal CampaignDiscount(
        Guid id,
        Guid campaignId,
        bool isDiscountCodeRequired,
        string? discountCode,
        int availableQuantity,
        int maximumUsePerPerson,
        DiscountMethod discountMethod,
        int? minimumSpendAmount,
        bool? applyToAllShippingMethods,
        IEnumerable<DeliveryMethod> deliveryMethods,
        DiscountType? discountType,
        int? discountAmount,
        int? discountPercentage
        ) : base(id)
    {
        CampaignId = campaignId;
        SetIsDiscountCodeRequired(isDiscountCodeRequired, discountCode);
        SetAvailableQuantity(availableQuantity);
        SetMaximumUsePerPerson(maximumUsePerPerson);
        SetDiscountMethod(discountMethod, minimumSpendAmount);
        SetShippingOptions(applyToAllShippingMethods, deliveryMethods);
        SetDiscount(discountType, discountAmount, discountPercentage);
    }

    public void SetAvailableQuantity(int quantity) => AvailableQuantity = Check.Range(quantity, nameof(AvailableQuantity), 0);
    public void SetMaximumUsePerPerson(int maximumUsePerPerson) => MaximumUsePerPerson = Check.Range(maximumUsePerPerson, nameof(MaximumUsePerPerson), 0);

    public void SetIsDiscountCodeRequired(bool isRequired, string? code = null)
    {
        IsDiscountCodeRequired = isRequired;

        if (IsDiscountCodeRequired)
        {
            DiscountCode = Check.NotNullOrWhiteSpace(code, nameof(DiscountCode), CampaignConsts.MaxDiscountCodeLength);
        }
        else
        {
            DiscountCode = null;
        }
    }

    public void SetDiscountMethod(DiscountMethod method, int? minSpend = null)
    {
        DiscountMethod = method;

        if (DiscountMethod == DiscountMethod.MinimumSpendAmount)
        {
            Check.NotNull(minSpend, nameof(MinimumSpendAmount));
            MinimumSpendAmount = Check.Range(minSpend.Value, nameof(MinimumSpendAmount), 0);
        }
        else
        {
            MinimumSpendAmount = null;
        }
    }

    public void SetShippingOptions(bool? applyToAllShippingMethods, IEnumerable<DeliveryMethod> methods = null!)
    {
        if (DiscountMethod == DiscountMethod.ShippingDiscount && !applyToAllShippingMethods.HasValue)
        {
            throw new BusinessException("Apply To All Shipping Methods can not be null.", nameof(applyToAllShippingMethods));
        }

        ApplyToAllShippingMethods = applyToAllShippingMethods;

        if (applyToAllShippingMethods == false)
        {
            if (methods == null || !methods.Any())
            {
                throw new BusinessException("Delivery methods cannot be empty when Apply To All Shipping Methods is false.", nameof(methods));
            }

            DeliveryMethodsJson = JsonSerializer.Serialize(methods);
        }
        else
        {
            DeliveryMethodsJson = JsonSerializer.Serialize(string.Empty);
        }
    }

    public void SetDiscount(DiscountType? type, int? amount = null, int? percentage = null)
    {
        DiscountType = type;

        if (type == Campaigns.DiscountType.FixedAmount)
        {
            Check.NotNull(amount, nameof(DiscountAmount));
            DiscountAmount = Check.Range(amount.Value, nameof(DiscountAmount), 0);
            DiscountPercentage = null;
        }
        else if (type == Campaigns.DiscountType.Percentage)
        {
            Check.NotNull(percentage, nameof(DiscountPercentage));
            DiscountPercentage = Check.Range(percentage.Value, nameof(DiscountPercentage), 0);
            DiscountAmount = null;
        }
    }
}
