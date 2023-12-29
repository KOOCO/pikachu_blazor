using System.ComponentModel.DataAnnotations.Schema;
using System;
using Kooco.Pikachu.GroupBuys;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class AutomaticEmailGroupBuysDto
    {
        public Guid Id { get; set; }
        public Guid AutomaticEmailId { get; set; }
        public Guid GroupBuyId { get; set; }
        public GroupBuyDto GroupBuy { get; set; }
    }
}