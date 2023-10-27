using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Orders
{
    public class OrderReport
    {
        public Guid? GroupBuyId { get; set; }
        public string? GroupBuyName { get; set; }
        public int? TotalQuantity { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
