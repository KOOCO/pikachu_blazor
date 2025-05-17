using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Campaigns;

public class CampaignShoppingCredit : Entity<Guid>
{
    public Guid CampaignId { get; set; }
    public bool CanExpire { get; private set; }
    public int? ValidForDays { get; private set; }
    public CalculationMethod CalculationMethod { get; private set; }
    public double? CalculationPercentage { get; private set; }
    public double? CapAmount { get; private set; }
    public bool ApplicableToAddOnProducts { get; set; }
    public bool ApplicableToShippingFees { get; set; }
    public int Budget { get; private set; }
    public ICollection<CampaignStageSetting> StageSettings { get; set; }

    [ForeignKey(nameof(CampaignId))]
    public virtual Campaign Campaign { get; set; }

    private CampaignShoppingCredit() { }

    internal CampaignShoppingCredit(
        Guid id,
        Guid campaignId,
        bool canExpire,
        int? validForDays,
        CalculationMethod calculationMethod,
        double? calculationPercentage,
        double? capAmount,
        bool applicableToAddOnProducts,
        bool applicableToShippingFees,
        int budget,
        int maxPointsToReceive
        ) : base(id)
    {
        CampaignId = campaignId;
        SetExpiration(canExpire, validForDays);
        SetCalculationMethod(calculationMethod, calculationPercentage, capAmount, maxPointsToReceive);
        ApplicableToAddOnProducts = applicableToAddOnProducts;
        ApplicableToShippingFees = applicableToShippingFees;
        SetBudget(budget);
        StageSettings = new List<CampaignStageSetting>();
    }

    public void SetExpiration(bool canExpire, int? validForDays)
    {
        CanExpire = canExpire;
        if (CanExpire)
        {
            Check.NotNull(validForDays, nameof(ValidForDays));
            ValidForDays = Check.Range(validForDays.Value, nameof(ValidForDays), 0);
        }
        else
        {
            ValidForDays = null;
        }
    }

    public void SetCalculationMethod(CalculationMethod method, double? percentage, double? capAmount, int maxPointsToReceive)
    {
        CalculationMethod = method;
        if (CalculationMethod == CalculationMethod.UnifiedCalculation)
        {
            Check.NotNull(percentage, nameof(CalculationPercentage));
            Check.NotNull(capAmount, nameof(CapAmount));
            CalculationPercentage = Check.Range(percentage.Value, nameof(CalculationPercentage), 0);
            CapAmount = Check.Range(capAmount.Value, nameof(CapAmount), 0);
        }
        else
        {
            CalculationPercentage = null;
            CapAmount = maxPointsToReceive;
        }
    }

    public void SetBudget(int budget)
    {
        Budget = Check.Range(budget, nameof(Budget), 0);
    }

    public void DeductBudget(int deduction)
    {
        Budget -= deduction;
    }

    public CampaignStageSetting AddStageSetting(Guid id, int spend, int pointsToReceive)
    {
        var stageSetting = new CampaignStageSetting(id, Id, spend, pointsToReceive);
        StageSettings.Add(stageSetting);
        return stageSetting;
    }
}
