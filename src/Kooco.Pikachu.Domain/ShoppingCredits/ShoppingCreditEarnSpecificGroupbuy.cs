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
    public class ShoppingCreditEarnSpecificGroupbuy : Entity<Guid>
    {
        public Guid ShoppingCreditEarnSettingId { get; set; }
        public Guid GroupbuyId { get; set; }
        [ForeignKey(nameof(ShoppingCreditEarnSettingId))]
        public ShoppingCreditEarnSetting? ShoppingCreditEarnSetting { get; set; }

        [ForeignKey(nameof(GroupbuyId))]
        public GroupBuy? GroupBuy { get; set; }
        public ShoppingCreditEarnSpecificGroupbuy(Guid id, Guid shoppingCreditEarnSettingId, Guid groupbuyId)
            : base(id)
        {
            ShoppingCreditEarnSettingId = shoppingCreditEarnSettingId;
            GroupbuyId = groupbuyId;
        }
    }
}
