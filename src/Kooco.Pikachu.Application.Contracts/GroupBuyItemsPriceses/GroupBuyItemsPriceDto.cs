using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public class GroupBuyItemsPriceDto: FullAuditedEntityDto<Guid>
    {

       
        public Guid? SetItemId { get; set; }
        public Guid? GroupBuyId { get; set; }
        public float GroupBuyPrice { get; set; }
        public Guid? ItemDetailId { get; set; }
       
    }
}
