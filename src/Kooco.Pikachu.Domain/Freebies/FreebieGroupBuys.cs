using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Freebies
{
    public class FreebieGroupBuys : Entity 
    {
        public Guid FreebieId { get; set; }
        public Guid GroupBuyId { get; set; }
        public FreebieGroupBuys() 
        { 
        
        }
        public FreebieGroupBuys(Guid freeBieId, Guid groupBuyId)
        {
            FreebieId = freeBieId;
            GroupBuyId = groupBuyId;
        }
        public override object[] GetKeys()
        {
            return new object[] { FreebieId, GroupBuyId };
        }
    }
}
