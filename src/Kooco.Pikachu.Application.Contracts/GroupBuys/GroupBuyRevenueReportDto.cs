using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyRevenueReportDto
    {
        public Guid GroupBuyId { get; set; }
        public string GroupBuyName { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingRevenue { get; set; }
        public decimal RefundAmount { get; set; }
        public List<GroupBuyRevenueItemDto> Items { get; set; } = new List<GroupBuyRevenueItemDto>();
    }

    public class GroupBuyRevenueItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal NetRevenue { get; set; }
    }
}