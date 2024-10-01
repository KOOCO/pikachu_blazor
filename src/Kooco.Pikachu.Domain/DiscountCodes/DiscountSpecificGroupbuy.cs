using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountSpecificGroupbuy:Entity<Guid>
    {
        public Guid DiscountCodeId { get; set; }
        public Guid GroupbuyId { get; set; }
        [ForeignKey(nameof(DiscountCodeId))]
        public DiscountCode? DiscountCode { get; set; }

        [ForeignKey(nameof(GroupbuyId))]
        public GroupBuy? Groupbuy { get; set; }
        public DiscountSpecificGroupbuy(Guid id, Guid discountCodeId, Guid groupbuyId) : base(id)
        {
            DiscountCodeId = discountCodeId;
            GroupbuyId = groupbuyId;
        }
    }
}
