using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Dashboards;

public class SummaryReportModel
{
    public int TotalOrdersCompleted { get; set; }
    public int TotalOrders { get; set; }
    public int TotalItemsSold { get; set; }
    public decimal TotalSales { get; set; }
    public int ActiveGroupBuys { get; set; }
    public int NewMembers { get; set; }
    public decimal AverageOrderValue => TotalOrdersCompleted > 0 ? TotalSales / TotalOrdersCompleted : 0;
    public decimal OrderCompletionRate => TotalOrders > 0 ? TotalOrdersCompleted * 100 / TotalOrders : 0;
    public string? TopPerformingProduct { get; set; }
    public int? TopPerformingProductQuantity { get; set; }
    public List<SummaryReportGroupBuyModel> GroupBuys { get; set; } = [];
}

public class SummaryReportGroupBuyModel
{
    public string? Name { get; set; }
    public decimal Revenue { get; set; }
    public int CompletedOrders { get; set; }
    public decimal PercentageOfTotal { get; set; }
}

public class ProductSummaryModel
{
    public string? Name { get; set; }
    public string? SKU { get; set; }
    public int TotalQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalRevenue { get; set; }
    public string? GroupBuy { get; set; }
    public string? Category { get; set; }
    public string? CategoryZH { get; set; }
}

public class OrderItemDetailsModel
{
    public string? OrderNo { get; set; }
    public DateTime CreationTime { get; set; }
    public string? GroupBuy { get; set; }
    public string? ProductName { get; set; }
    public string? Attributes { get; set; }
    public string? SKU { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public ShippingStatus? ShippingStatus { get; set; }
}