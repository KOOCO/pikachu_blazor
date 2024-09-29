using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.AddOnProducts
{
    public class CreateUpdateAddOnProductDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        
        public int AddOnAmount { get; set; }
        [Required]
        public int AddOnLimitPerOrder { get; set; }
        [Required]
        public string QuantitySetting { get; set; }
        public int AvailableQuantity { get; set; }
        [Required]
        public string DisplayOriginalPrice { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string AddOnConditions { get; set; }
        public int MinimumSpendAmount { get; set; }
        [Required]
        public string GroupbuysScope { get; set; }
        public bool Status { get; set; }
        public int SellingQuantity { get; set; }
        public List<Guid> GroupBuyIds { get; set; } = new List<Guid>();
    }
}
