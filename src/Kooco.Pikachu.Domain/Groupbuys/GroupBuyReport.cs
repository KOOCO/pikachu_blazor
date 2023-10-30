using System;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyReport
    {
        public Guid GroupBuyId { get; set; }
        public string? GroupBuyName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
    }
}
