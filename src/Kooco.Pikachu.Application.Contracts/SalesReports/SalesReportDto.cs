using Kooco.Pikachu.Reports;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.SalesReports;

public class SalesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Date { get { return GetDate(); } }
    public string? GroupBuyName { get; set; }
    public decimal GrossSales { get; set; }
    public decimal NetSales { get; set; }
    public decimal Discount { get; set; }
    public decimal NumberOfOrders { get; set; }
    public decimal AverageOrderValue { get { return NumberOfOrders > 0 ? GrossSales / NumberOfOrders : GrossSales; } }
    public decimal CostOfGoodsSold { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal GrossProfit { get; set; }
    public decimal GrossProfitMargin { get; set; }

    public ReportCalculationUnits ReportCalculationUnit { get; set; }
    public bool ShowDetails { get; set; } = false;
    public List<SalesReportDto> Details { get; set; } = [];

    private string GetDate()
    {
        return ReportCalculationUnit switch
        {
            ReportCalculationUnits.Daily => StartDate.ToString("yyyy-MM-dd"),
            ReportCalculationUnits.Weekly => $"{StartDate:yyyy-MM-dd} ~ {EndDate:yyyy-MM-dd}",
            ReportCalculationUnits.Monthly => StartDate.ToString("yyyy-MMM"),
            _ => ""
        };
    }
}