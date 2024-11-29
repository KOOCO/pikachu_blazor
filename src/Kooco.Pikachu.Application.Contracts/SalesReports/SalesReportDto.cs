using System;

namespace Kooco.Pikachu.SalesReports;

public class SalesReportDto
{
    public DateTime Date { get; set; }
    public decimal GrossSales { get; set; }
    public decimal NetSales { get; set; }
    public decimal Discount { get; set; }
    public decimal NumberOfOrders { get; set; }
    public decimal AverageOrderValue { get { return NumberOfOrders > 0 ? GrossSales / NumberOfOrders : GrossSales; } }
    public decimal CostOfGoodsSold { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal GrossProfitMargin { get; set; }
}