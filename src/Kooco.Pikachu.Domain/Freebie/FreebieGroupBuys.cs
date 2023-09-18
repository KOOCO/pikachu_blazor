using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Freebie
{
    public class FreebieGroupBuys : Entity 
    {
        public Guid FreeBieId { get; set; }
        public Guid GroupBuyId { get; set; }
        public FreebieGroupBuys() 
        { 
        
        }
        public FreebieGroupBuys(Guid freeBieId, Guid groupBuyId)
        {
            FreeBieId = freeBieId;
            GroupBuyId = groupBuyId;
        }
        public override object[] GetKeys()
        {
            return new object[] { FreeBieId, GroupBuyId };
        }
    }
}
