using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyPricingDto
    {
        public Guid GroupBuyId { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxRate { get; set; }
        public List<BulkPricingRuleDto> BulkPricingRules { get; set; } = new List<BulkPricingRuleDto>();
    }

    public class BulkPricingRuleDto
    {
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal FixedPrice { get; set; }
    }
}