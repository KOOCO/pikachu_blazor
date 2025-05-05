using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditEarnSetting:AuditedAggregateRoot<Guid>
    { 
     public bool RegistrationBonusEnabled { get; set; }
    public int RegistrationEarnedPoints { get;  set; }
    public string RegistrationUsagePeriodType { get; set; }
    public int RegistrationValidDays { get; set; }

    public bool BirthdayBonusEnabled { get; set; }
    public int BirthdayEarnedPoints { get; set; }
    public string BirthdayUsagePeriodType { get; set; }
    public int BirthdayValidDays { get; set; }

    public bool CashbackEnabled { get; set; }
    public string CashbackUsagePeriodType { get; set; }
    public int CashbackValidDays { get; set; }
    public string CashbackCalculationMethod { get; set; }
    public decimal CashbackUnifiedMaxDeductiblePoints { get; set; }

    public string CashbackStagedSettings { get; set; }
    public string? CashbackApplicableItems { get; set; }
    public string CashbackApplicableGroupbuys { get; set; }
    public string CashbackApplicableProducts { get; set; }

    public ICollection<ShoppingCreditEarnSpecificProduct> SpecificProducts { get; set; }
    public ICollection<ShoppingCreditEarnSpecificGroupbuy> SpecificGroupbuys { get; set; }
   

  

    public ShoppingCreditEarnSetting(
        Guid id,
        bool registrationBonusEnabled,
        int registrationEarnedPoints,
        string registrationUsagePeriodType,
        int registrationValidDays,
        bool birthdayBonusEnabled,
        int birthdayEarnedPoints,
        string birthdayUsagePeriodType,
        int birthdayValidDays,
        bool cashbackEnabled,
        string cashbackUsagePeriodType,
        int cashbackValidDays,
        string cashbackCalculationMethod,
        decimal cashbackUnifiedMaxDeductiblePoints,
        string cashbackStagedSettings,
        string? cashbackApplicableItems,
        string cashbackApplicableGroupbuys,
        string cashbackApplicableProducts) : base(id)
    {
        RegistrationBonusEnabled = registrationBonusEnabled;
        RegistrationEarnedPoints = registrationEarnedPoints;
        RegistrationUsagePeriodType = registrationUsagePeriodType;
        RegistrationValidDays = registrationValidDays;

        BirthdayBonusEnabled = birthdayBonusEnabled;
        BirthdayEarnedPoints = birthdayEarnedPoints;
        BirthdayUsagePeriodType = birthdayUsagePeriodType;
        BirthdayValidDays = birthdayValidDays;

        CashbackEnabled = cashbackEnabled;
        CashbackUsagePeriodType = cashbackUsagePeriodType;
        CashbackValidDays = cashbackValidDays;
        CashbackCalculationMethod = cashbackCalculationMethod;
        CashbackUnifiedMaxDeductiblePoints = cashbackUnifiedMaxDeductiblePoints;

        CashbackStagedSettings = cashbackStagedSettings;
        CashbackApplicableItems = cashbackApplicableItems;
        CashbackApplicableGroupbuys = cashbackApplicableGroupbuys;
        CashbackApplicableProducts = cashbackApplicableProducts;

        SpecificProducts = new List<ShoppingCreditEarnSpecificProduct>();
        SpecificGroupbuys = new List<ShoppingCreditEarnSpecificGroupbuy>();
       
    }

        public void AddSpecificProducts(List<Guid> productIds)
        {
            if (productIds.Count > 0)
            {
                foreach (var id in productIds)
                {
                    SpecificProducts.Add(new ShoppingCreditEarnSpecificProduct(Guid.NewGuid(), Id, id));
                }
            }
        }

        public void UpdateSpecificProducts(List<Guid> newProducts)
        {
            SpecificProducts.Clear();
            AddSpecificProducts(newProducts);
        }
        public void AddSpecificGroupbuys(List<Guid> groupbuyIds)
        {
            if (groupbuyIds.Count > 0)
            {
                foreach (var id in groupbuyIds)
                {
                    SpecificGroupbuys.Add(new ShoppingCreditEarnSpecificGroupbuy(Guid.NewGuid(), Id, id));
                }
            }
        }

        public void UpdateSpecificGroupbuys(List<Guid> newGroupbuys)
        {
            SpecificGroupbuys.Clear();
            AddSpecificGroupbuys(newGroupbuys);
        }

    }
}