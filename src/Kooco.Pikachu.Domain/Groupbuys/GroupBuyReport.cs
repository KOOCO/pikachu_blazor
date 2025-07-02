using System;

namespace Kooco.Pikachu.Groupbuys
{
    public class GroupBuyReport
    {
        public Guid GroupBuyId { get; set; }
        public string? GroupBuyName { get; set; }
        public int TotalQuantity { get; set; }
        public string  TenantName { get; set; }
        public decimal SalesAmount { get; set; }
        public decimal AmountReceived { get; set; }
        public int TotalOrder { get; set; }
        public int WatingForShipment { get; set; }
        public int CompleteOrders { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public int ProfitShare { get; set; }
        public decimal ProfitShareAmount => AmountReceived * (ProfitShare / 100m);
        public decimal? SalesAmountDiscount { get; set; }
        public decimal? AmountReceivedDiscount { get; set; }
    }
}
