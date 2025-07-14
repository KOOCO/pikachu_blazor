using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSettingDto:AuditedEntityDto<Guid>
    {
        public bool AllowUsage { get; set; }
        public string DeductionMethod { get; set; }
        public decimal UnifiedMaxDeductiblePoints { get; set; }
        public string StagedSettings { get; set; }
        public string? ApplicableItems { get; set; }
        public string UsableGroupbuysScope { get; set; }
        public string UsableProductsScope { get; set; }
        public int MaximumDeduction { get; set; }
        public int MinimumSpendAmount { get; set; }
        public List<ShoppingCreditUsageSpecificProductDto> SpecificProducts { get; set; }
        public List<ShoppingCreditUsageSpecificGroupbuyDto> SpecificGroupbuys { get; set; }
    }
}
