using System;
using System.Collections.Generic;
using System.Text;

namespace Kooco.Pikachu.AddOnProducts
{
    public class CreateAddOnProductDto
    {
        public Guid ProductId { get; set; }
        public int AddOnAmount { get; set; }
        public int AddOnLimitPerOrder { get; set; }
        public string QuantitySetting { get; set; }
        public int AvailableQuantity { get; set; }
        public string DisplayOriginalPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AddOnConditions { get; set; }
        public int MinimumSpendAmount { get; set; }
        public string GroupbuysScope { get; set; }
        public bool Status { get; set; }
        public int SellingQuantity { get; set; }
       public List<Guid> GroupBuyIds { get; set; } = new List<Guid>();
    }
}
