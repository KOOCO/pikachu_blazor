using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kooco.Pikachu.DiscountCodes
{
    
    public class DiscountCheckOutputDto
    {
		public Guid DiscountCodeId { get; set; }
		public string ProductsScope { get; set; }
		public Guid[] ProductIds { get; set; }
        public string DiscountType { get; set; }
        public int? DiscountAmount { get; set; }
        public int? DiscountPercentage { get; set; }
        public int? MinimumSpendAmount { get; set; }
        public string[] ShippingMethods { get; set; }
        public string ErrorMessage { get; set; }
    }
}
