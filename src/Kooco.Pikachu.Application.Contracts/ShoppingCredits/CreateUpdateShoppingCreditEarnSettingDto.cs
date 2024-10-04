using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class CreateUpdateShoppingCreditEarnSettingDto
    {
        [Required]
        public bool RegistrationBonusEnabled { get; set; }
        [Required]
        public int RegistrationEarnedPoints { get; set; }
        [Required]
        public string RegistrationUsagePeriodType { get; set; }
        public int RegistrationValidDays { get; set; }
        [Required]
        public bool BirthdayBonusEnabled { get; set; }
        [Required]
        public int BirthdayEarnedPoints { get; set; }
        [Required]
        public string BirthdayUsagePeriodType { get; set; }
        
        public int BirthdayValidDays { get; set; }
        [Required]
        public bool CashbackEnabled { get; set; }
        [Required]
        public string CashbackUsagePeriodType { get; set; }
        public int CashbackValidDays { get; set; }
        public string CashbackCalculationMethod { get; set; }
       
        public int CashbackUnifiedMaxDeductiblePoints { get; set; }

        public string CashbackStagedSettings { get; set; }
        [Required]
        public string CashbackApplicableItems { get; set; }
        [Required]
        public string CashbackApplicableGroupbuys { get; set; }
        [Required]
        public string CashbackApplicableProducts { get; set; }

        public List<Guid> SpecificProductIds { get; set; }
        public List<Guid> SpecificGroupbuyIds { get; set; }
    }
}
