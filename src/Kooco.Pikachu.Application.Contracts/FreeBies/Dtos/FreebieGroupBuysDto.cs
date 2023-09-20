using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.Freebies.Dtos
{
    public class FreebieGroupBuysDto
    {
        public Guid FreeBieId { get; set; }
        public Guid GroupBuyId { get; set; }
    }
}
