using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class CreateUpdateShoppingCreditUsageSettingDto
    {
        [Required]
        public bool AllowUsage { get; set; }

        [Required]
        [StringLength(100)]
        public string DeductionMethod { get; set; }

        [Required]
        public int UnifiedMaxDeductiblePoints { get; set; }

        public string StagedSettings { get; set; }

        [StringLength(255)]
        public string ApplicableItems { get; set; }

        [StringLength(50)]
        public string UsableGroupbuysScope { get; set; }

        [StringLength(50)]
        public string UsableProductsScope { get; set; }
       public List<Guid> GroupbuyIds { get; set; }
       public List<Guid> ProductIds { get; set; }
    }
}
