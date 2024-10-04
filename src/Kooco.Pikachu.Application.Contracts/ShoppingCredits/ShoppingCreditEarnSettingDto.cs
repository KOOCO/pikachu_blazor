using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditEarnSettingDto:AuditedEntityDto<Guid>
    {
        public Guid Id { get; set; }
        public bool RegistrationBonusEnabled { get; set; }
        public int RegistrationEarnedPoints { get; set; }
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
        public string CashbackApplicableItems { get; set; }
        public string CashbackApplicableGroupbuys { get; set; }
        public string CashbackApplicableProducts { get; set; }

        public List<ShoppingCreditEarnSpecificProductDto> SpecificProducts { get; set; }
        public List<ShoppingCreditEarnSpecificGroupbuyDto> SpecificGroupBuys { get; set; }
    }
}
