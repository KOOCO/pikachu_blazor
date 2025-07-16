using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.GroupBuys
{
    public class GroupBuyInventoryReportDto
    {
        public Guid GroupBuyId { get; set; }
        public string GroupBuyName { get; set; }
        public int TotalStock { get; set; }
        public int ReservedStock { get; set; }
        public int AvailableStock { get; set; }
        public int SoldQuantity { get; set; }
        public List<GroupBuyInventoryItemDto> Items { get; set; } = new List<GroupBuyInventoryItemDto>();
    }

    public class GroupBuyInventoryItemDto
    {
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public int Stock { get; set; }
        public int Reserved { get; set; }
        public int Available { get; set; }
        public int Sold { get; set; }
    }
}