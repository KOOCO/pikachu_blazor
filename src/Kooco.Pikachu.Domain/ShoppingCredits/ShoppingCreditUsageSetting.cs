using Kooco.Pikachu.DiscountCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSetting : AuditedAggregateRoot<Guid>
    {
        public bool AllowUsage { get; set; }
        public string DeductionMethod { get; set; }
        public decimal UnifiedMaxDeductiblePoints { get; set; }
        public int MaximumDeduction { get; set; }
        public string StagedSettings { get; set; } // You may need to handle this as a JSON object if required
        public string? ApplicableItems { get; set; }
        public string UsableGroupbuysScope { get; set; }
        public string UsableProductsScope { get; set; }

        public ICollection<ShoppingCreditUsageSpecificProduct> SpecificProducts { get; set; }
        public ICollection<ShoppingCreditUsageSpecificGroupbuy> SpecificGroupbuys { get; set; }

        public ShoppingCreditUsageSetting(
            Guid id,
            bool allowUsage,
            string deductionMethod,
            decimal unifiedMaxDeductiblePoints,
            string stagedSettings,
            string applicableItems,
            string usableGroupbuysScope,
            string usableProductsScope,
            int maximumDeduction
        ) : base(id)
        {
            AllowUsage = allowUsage;
            DeductionMethod = Check.NotNullOrWhiteSpace(deductionMethod, nameof(deductionMethod));
            UnifiedMaxDeductiblePoints = unifiedMaxDeductiblePoints;
            StagedSettings = Check.NotNullOrWhiteSpace(stagedSettings, nameof(stagedSettings));
            ApplicableItems = Check.NotNullOrWhiteSpace(applicableItems, nameof(applicableItems));
            UsableGroupbuysScope = Check.NotNullOrWhiteSpace(usableGroupbuysScope, nameof(usableGroupbuysScope));
            UsableProductsScope = Check.NotNullOrWhiteSpace(usableProductsScope, nameof(usableProductsScope));
            MaximumDeduction = maximumDeduction;

            SpecificProducts = new List<ShoppingCreditUsageSpecificProduct>();
            SpecificGroupbuys = new List<ShoppingCreditUsageSpecificGroupbuy>();
        }

        public void AddSpecificGroupbuys(List<Guid> groupbuyIds)
        {
            if (groupbuyIds.Count > 0)
            {
                foreach (var id in groupbuyIds)
                {
                    SpecificGroupbuys.Add(new ShoppingCreditUsageSpecificGroupbuy(Guid.NewGuid(), Id, id));
                }
            }
        }

        public void UpdateSpecificGroupbuys(List<Guid> newGroupbuys)
        {
            SpecificGroupbuys.RemoveAll(SpecificGroupbuys);
            AddSpecificGroupbuys(newGroupbuys);
        }

        public void AddSpecificProducts(List<Guid> productIds)
        {
            if (productIds.Count > 0)
            {

               
                foreach (var id in productIds)
                {
                    SpecificProducts.Add(new ShoppingCreditUsageSpecificProduct(Guid.NewGuid(), Id, id));
                }
            }
        }

        public void UpdateSpecificProducts(List<Guid> newProducts)
        {
            SpecificProducts.RemoveAll(SpecificProducts);
            AddSpecificProducts(newProducts);
        }
    }
}