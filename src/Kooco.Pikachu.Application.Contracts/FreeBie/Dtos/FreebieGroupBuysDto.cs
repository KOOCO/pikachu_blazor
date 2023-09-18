using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Freebie.Dtos
{
    public class FreebieGroupBuysDto: FullAuditedAggregateRoot<Guid>, IHasConcurrencyStamp
    {
        public Guid FreeBieId { get; set; }
        public Guid GroupBuyId { get; set; }

    }
}
