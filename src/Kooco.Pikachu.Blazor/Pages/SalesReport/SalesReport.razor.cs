using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Reports;
using Kooco.Pikachu.SalesReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.SalesReport;

public partial class SalesReport
{
    private List<SalesReportDto> SalesReportList { get; set; }
    private GetSalesReportDto Filters { get; set; }
    private List<KeyValueDto> GroupBuyLookup { get; set; }
    private bool IsLoading { get; set; } = false;

    public SalesReport()
    {
        SalesReportList = [];
        Filters = new();
        GroupBuyLookup = [];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await ResetAsync();
                GroupBuyLookup = await GroupBuyAppService.GetGroupBuyLookupAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private async Task GetSalesReportAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();
            SalesReportList = await SalesReportAppService.GetSalesReportAsync(Filters);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task FilterAsync()
    {
        await GetSalesReportAsync();
    }

    private async Task ResetAsync()
    {
        Filters = new();
        await GetSalesReportAsync();
    }

    private async Task UnitChangedAsync(ReportCalculationUnits reportCalculationUnit)
    {
        Filters.ReportCalculationUnit = reportCalculationUnit;
        await InvokeAsync(StateHasChanged);
        await GetSalesReportAsync();
    }

    private async Task OnShowDetails(SalesReportDto input)
    {
        input.ShowDetails = !input.ShowDetails;
        if (input.Details.Count == 0)
        {
            input.Details = await SalesReportAppService.GetGroupBuySalesReportAsync(input.StartDate, input.EndDate, Filters.GroupBuyId);
        }
    }

    private async Task ExportAsync()
    {
        try
        {
            var headers = new Dictionary<string, string>
            {
                { "Date", L["Date"] },
                { "GrossSales", L["GrossSales"] },
                { "NetSales", L["NetSales"] },
                { "Discount", L["Discount"] },
                { "NumberOfOrders", L["NumberOfOrders"] },
                { "AverageOrderValue", L["AverageOrderValue"] },
                { "CostOfGoodsSold", L["CostOfGoodsSold"] },
                { "ShippingCost", L["ShippingCost"] },
                { "GrossProfit", L["GrossProfit"] },
                { "GrossProfitMargin", L["GrossProfitMargin"] }
            };

            var excelModel = SalesReportList
                .Select(x => new Dictionary<string, object>
                {
                    { headers["Date"], x.Date },
                    { headers["GrossSales"], x.GrossSales == 0 ? "-" : x.GrossSales.ToString("N2") },
                    { headers["NetSales"], x.NetSales == 0 ? "-" : x.NetSales.ToString("N2") },
                    { headers["Discount"], x.Discount == 0 ? "-" : x.Discount.ToString("N2") },
                    { headers["NumberOfOrders"], x.NumberOfOrders == 0 ? "-" : x.NumberOfOrders.ToString("N0") },
                    { headers["AverageOrderValue"], x.AverageOrderValue == 0 ? "-" : x.AverageOrderValue.ToString("N2") },
                    { headers["CostOfGoodsSold"], x.CostOfGoodsSold == 0 ? "-" : x.CostOfGoodsSold.ToString("N2") },
                    { headers["ShippingCost"], x.ShippingCost == 0 ? "-" : x.ShippingCost.ToString("N2") },
                    { headers["GrossProfit"], x.GrossProfit == 0 ? "-" : x.GrossProfit.ToString("N2") },
                    { headers["GrossProfitMargin"], x.GrossProfitMargin == 0 ? "-" : x.GrossProfitMargin.ToString("N3") }
                }).ToList();

            var fileName = $"{L["SalesReport"]} - {L[Filters.ReportCalculationUnit.ToString()]}.xlsx";
            await ExcelDownloadHelper.DownloadExcelAsync(excelModel, fileName);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}