using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Kooco.Pikachu.DiscountCodes
{
    public class CreateUpdateDiscountCodeDto
    {
        [Required]
        public string EventName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Code { get; set; } // Renamed to avoid conflict with class name
        public string SpecifiedCode { get; set; } = "";
        [Required]
        public int AvailableQuantity { get; set; } = 0;
       
        public int TotalQuantity { get; set; }
        [Required]
        public int MaxUsePerPerson { get; set; }
        [Required]
        public string GroupbuysScope { get; set; }
        [Required]
        public string ProductsScope { get; set; }
        [Required]
        public string DiscountMethod { get; set; }
        public int MinimumSpendAmount { get; set; }
        [Required]
        public string ShippingDiscountScope { get; set; }
        public int DiscountPercentage { get; set; }
        public int DiscountAmount { get; set; }
        public bool Status { get; set; }
        public List<Guid> GroupbuyIds { get; set; }
        public List<Guid> ProductIds { get; set; } 
        public List<int> SpecificShippingMethods { get; set; }
    }
}
