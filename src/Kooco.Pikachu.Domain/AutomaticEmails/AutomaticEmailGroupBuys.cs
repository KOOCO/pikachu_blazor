using Kooco.Pikachu.GroupBuys;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class AutomaticEmailGroupBuys : Entity<Guid>
    {
        public Guid AutomaticEmailId { get; set; }
        public Guid GroupBuyId { get; set; }

        [ForeignKey(nameof(GroupBuyId))]
        public GroupBuy GroupBuy { get; set; }

        public AutomaticEmailGroupBuys()
        {
            
        }
        public AutomaticEmailGroupBuys(
            Guid id,
            Guid automaticEmailId,
            Guid groupBuyId
            ) : base(id)
        {
            AutomaticEmailId = automaticEmailId;
            GroupBuyId = groupBuyId;
        }
    }
}
