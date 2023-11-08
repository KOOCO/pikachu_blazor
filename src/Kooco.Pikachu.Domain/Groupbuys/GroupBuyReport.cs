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
        public string  TenantName { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal AmountReceived { get; set; }
        public int TotalOrder { get; set; }
    }
}
