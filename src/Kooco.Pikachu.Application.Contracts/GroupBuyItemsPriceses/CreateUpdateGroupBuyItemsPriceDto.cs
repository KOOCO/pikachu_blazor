using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.GroupBuyItemsPriceses
{
    public class CreateUpdateGroupBuyItemsPriceDto
    {
        public Guid? SetItemId { get; set; }
        public Guid GroupBuyId { get; set; }
        public float GroupBuyPrice { get; set; }
        public Guid? ItemDetailId { get; set; }
    }
}
