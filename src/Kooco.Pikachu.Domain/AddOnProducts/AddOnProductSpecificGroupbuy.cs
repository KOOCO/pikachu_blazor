using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.AddOnProducts
{
    public class AddOnProductSpecificGroupbuy : Entity<Guid>
    {
        public Guid AddOnProductId { get; set; }
        public Guid GroupbuyId { get; set; }
        [ForeignKey(nameof(AddOnProductId))]
        public AddOnProduct? AddOnProduct { get; set; }

        [ForeignKey(nameof(GroupbuyId))]
        public GroupBuy? Groupbuy { get; set; }
        public AddOnProductSpecificGroupbuy(Guid id, Guid addOnProductId, Guid groupbuyId) : base(id)
        {
            AddOnProductId = addOnProductId;
            GroupbuyId = groupbuyId;
            }
    }
}
