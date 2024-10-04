using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSpecificProduct : Entity<Guid>
    {
        public Guid ShoppingCreditsUsageSettingId { get; set; }
        public Guid ProductId { get; set; }
        [ForeignKey(nameof(ShoppingCreditsUsageSettingId))]
        public ShoppingCreditUsageSetting? ShoppingCreditsUsageSetting { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Item? Product { get; set; }
        public ShoppingCreditUsageSpecificProduct(Guid id, Guid shoppingCreditsUsageSettingId, Guid productId)
            : base(id)
        {
            ShoppingCreditsUsageSettingId = shoppingCreditsUsageSettingId;
            ProductId = productId;
        }
    }
}