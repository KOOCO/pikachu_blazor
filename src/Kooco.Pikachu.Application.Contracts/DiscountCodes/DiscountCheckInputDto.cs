using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.DiscountCodes
{
    public class DiscountCheckInputDto
    {
        public Guid GroupbuyId { get; set; }
        public string Code { get; set; }
    }
}
