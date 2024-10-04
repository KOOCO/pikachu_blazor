using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class ShoppingCreditUsageSpecificGroupbuy : Entity<Guid>
    {
        public Guid ShoppingCreditsUsageSettingId { get; set; }
        public Guid GroupbuyId { get; set; }
        [ForeignKey(nameof(ShoppingCreditsUsageSettingId))]
        public ShoppingCreditUsageSetting? ShoppingCreditsUsageSetting { get; set; }

        [ForeignKey(nameof(GroupbuyId))]
        public GroupBuy? GroupBuy { get; set; }
        public ShoppingCreditUsageSpecificGroupbuy(Guid id, Guid shoppingCreditsUsageSettingId, Guid groupbuyId)
            : base(id)
        {
            ShoppingCreditsUsageSettingId = shoppingCreditsUsageSettingId;
            GroupbuyId = groupbuyId;
        }
    }
}