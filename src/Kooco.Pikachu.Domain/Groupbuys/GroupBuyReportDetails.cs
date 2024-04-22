
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Groupbuys;

public class GroupBuyReportDetails
{
    public string GroupBuyName { get; set; }    
    public int OrderQuantityPaid { get; set; }
    public int TotalOrderQuantity { get; set; }
    public decimal SalesAmount { get; set; }
    public decimal SalesAmountExclShipping { get; set; }
    public decimal AmountReceived { get; set; }
    public decimal AmountReceivedExclShipping { get; set; }
    public decimal SalesAmountMinusShipping { get; set; }
    public decimal BloggersProfit { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public GroupBuy GroupBuy { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}
