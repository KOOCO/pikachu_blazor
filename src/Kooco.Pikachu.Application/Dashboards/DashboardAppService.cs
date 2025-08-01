using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Dashboards;

[Authorize]
[RemoteService(IsEnabled = false)]
public class DashboardAppService(IDashboardRepository dashboardRepository) : PikachuAppService, IDashboardAppService
{
    private readonly IDashboardRepository _dashboardRepository = dashboardRepository;

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(DashboardFiltersDto input)
    {
        var stats = await _dashboardRepository.GetDashboardStatsAsync(
            input.SelectedGroupBuyIds,
            input.StartDate,
            input.EndDate,
            input.PreviousDate
        );

        return ObjectMapper.Map<DashboardStatsModel, DashboardStatsDto>(stats);
    }

    public async Task<DashboardChartsDto> GetDashboardChartsAsync(DashboardFiltersDto input)
    {
        var charts = await _dashboardRepository.GetDashboardChartsAsync(
            input.SelectedPeriodOption,
            input.SelectedGroupBuyIds,
            input.StartDate,
            input.EndDate
        );

        return ObjectMapper.Map<DashboardChartsModel, DashboardChartsDto>(charts);
    }

    public async Task<PagedResultDto<DashboardOrdersDto>> GetRecentOrdersAsync(DashboardFiltersDto input)
    {
        var data = await _dashboardRepository.GetRecentOrdersAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.SelectedGroupBuyIds
            );

        return new PagedResultDto<DashboardOrdersDto>
        {
            TotalCount = data.TotalCount,
            Items = ObjectMapper.Map<List<DashboardOrdersModel>, List<DashboardOrdersDto>>(data.Items)
        };
    }

    public async Task<List<DashboardBestSellerDto>> GetBestSellerItemsAsync(DashboardFiltersDto input)
    {
        var data = await _dashboardRepository.GetBestSellerItemsAsync(input.SelectedGroupBuyIds);
        return ObjectMapper.Map<List<DashboardBestSellerModel>, List<DashboardBestSellerDto>>(data);
    }

    public async Task<byte[]> GetSalesReportExcelAsync(DashboardFiltersDto input)
    {
        if (!input.StartDate.HasValue || !input.EndDate.HasValue)
        {
            throw new UserFriendlyException("Please provide a date range");
        }

        ExcelPackage.License.SetNonCommercialOrganization("Pikachu");
        using var package = new ExcelPackage();
        await GenerateSummaryReportSheet(input, package);
        await GenerateProductSummaryReportSheet(input, package);
        await GenerateOrderItemDetailsSheet(input, package);

        return await Task.FromResult(package.GetAsByteArray());
    }

    private async Task GenerateSummaryReportSheet(DashboardFiltersDto input, ExcelPackage package)
    {
        var summary = await _dashboardRepository.GetSummaryReportAsync(
            input.StartDate.Value,
            input.EndDate.Value,
            input.SelectedGroupBuyIds
            );

        var sheet = package.Workbook.Worksheets.Add(L["SummaryReport"]);

        int row = 1;

        sheet.Cells[row, 1].Value = Upper("GroupBuySalesReport");
        sheet.Cells[row, 1, row, 2].Merge = true;
        sheet.Cells[row, 1, row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[row, 1, row, 2].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        row++;

        sheet.Cells[row++, 1].Value = L["GeneratedOn"] + ":";
        sheet.Cells[row - 1, 2].Value = DateTime.Today.ToString("M/dd/yyyy");

        sheet.Cells[row++, 1].Value = L["ReportPeriod"] + ":";
        sheet.Cells[row - 1, 2].Value = $"{input.StartDate.Value:yyyy-MM-dd} to {input.EndDate.Value:yyyy-MM-dd}";

        row++;

        sheet.Cells[row, 1].Value = Upper("KeyMetrics");
        sheet.Cells[row, 2].Value = Upper("Value");
        sheet.Cells[row, 1, row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[row, 1, row, 2].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        row++;

        var metrics = new (string Label, object Value)[]
        {
            (L["TotalOrdersCompleted"], summary.TotalOrdersCompleted),
            (L["TotalItemsSold"], summary.TotalItemsSold),
            (L["TotalSales"], $"${summary.TotalSales:N0}"),
            (L["AverageOrderValue"], $"${summary.AverageOrderValue:N0}"),
            (L["NewMembers"], summary.NewMembers),
            (L["ActiveGroupBuys"], summary.ActiveGroupBuys)
        };

        foreach (var (label, value) in metrics)
        {
            sheet.Cells[row, 1].Value = label;
            sheet.Cells[row, 2].Value = value;
            sheet.Cells[row, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            row++;
        }

        row++;

        sheet.Cells[row, 1].Value = Upper("GroupBuys");
        sheet.Cells[row, 2].Value = $"{Upper("Revenue")} ($)";
        sheet.Cells[row, 3].Value = Upper("CompletedOrders");
        sheet.Cells[row, 4].Value = Upper("PercentageOfTotal");
        sheet.Cells[row, 1, row, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[row, 1, row, 4].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        row++;

        foreach (var gb in summary.GroupBuys)
        {
            sheet.Cells[row, 1].Value = gb.Name;
            sheet.Cells[row, 2].Value = $"${gb.Revenue:N0}";
            sheet.Cells[row, 3].Value = gb.CompletedOrders;
            sheet.Cells[row, 4].Value = $"{gb.PercentageOfTotal:N0}%";
            sheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            row++;
        }

        row++;

        sheet.Cells[row, 1].Value = Upper("PerformanceHighlights");
        sheet.Cells[row, 1, row, 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[row, 1, row, 2].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);

        row++;

        var topPerformingGroupBuy = summary.GroupBuys.OrderByDescending(gb => gb.PercentageOfTotal).FirstOrDefault();
        var highlights = new (string Label, string Value)[]
        {
            ($"{L["BestSellingProduct"]}:", $"{summary.TopPerformingProduct} ({L["DashboardReport:UnitsSold", summary.TopPerformingProductQuantity]})"),
            ($"{L["HighestRevenueGroupBuy"]}:", $"{topPerformingGroupBuy?.Name} ({L["DashboardReport:Revenue", topPerformingGroupBuy?.PercentageOfTotal.ToString("N2")]})"),
            ($"{L["OrderCompletionRate"]}:", $"{summary.OrderCompletionRate:N0}% ({L["DashboardReport:OrdersComplete", summary.TotalOrdersCompleted, summary.TotalOrders]})")
        };

        foreach (var (label, value) in highlights)
        {
            sheet.Cells[row, 1].Value = label;
            sheet.Cells[row++, 2].Value = value;
        }

        sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
    }

    private async Task GenerateProductSummaryReportSheet(DashboardFiltersDto input, ExcelPackage package)
    {
        var summaries = await _dashboardRepository.GetProductSummaryAsync(
            input.StartDate.Value,
            input.EndDate.Value,
            input.SelectedGroupBuyIds
            );

        var sheet = package.Workbook.Worksheets.Add(L["ProductSummary"]);

        int row = 1;

        sheet.Cells[row, 1].Value = L["ProductName"];
        sheet.Cells[row, 2].Value = L["Attributes"];
        sheet.Cells[row, 3].Value = L["TotalQtySold"];
        sheet.Cells[row, 4].Value = L["UnitPrice"];
        sheet.Cells[row, 5].Value = L["TotalRevenue"];
        sheet.Cells[row, 6].Value = L["GroupBuy"];
        sheet.Cells[row, 7].Value = L["Category"];
        sheet.Cells[row, 1, row, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[row, 1, row, 7].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        row++;

        foreach (var summary in summaries)
        {
            sheet.Cells[row, 1].Value = summary.Name;
            sheet.Cells[row, 2].Value = summary.SKU;
            sheet.Cells[row, 3].Value = summary.TotalQuantity;
            sheet.Cells[row, 4].Value = summary.UnitPrice;
            sheet.Cells[row, 5].Value = summary.TotalRevenue;
            sheet.Cells[row, 6].Value = summary.GroupBuy;
            sheet.Cells[row, 7].Value = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "zh" ? summary.CategoryZH : summary.Category;

            row++;
        }
    }

    private async Task GenerateOrderItemDetailsSheet(DashboardFiltersDto input, ExcelPackage package)
    {
        var summaries = await _dashboardRepository.GetOrderItemDetailsAsync(
            input.StartDate.Value,
            input.EndDate.Value,
            input.SelectedGroupBuyIds
            );

        var sheet = package.Workbook.Worksheets.Add(L["OrderItemDetails"]);

        int row = 1;

        sheet.Cells[row, 1].Value = L["OrderNo"];
        sheet.Cells[row, 2].Value = L["OrderDate"];
        sheet.Cells[row, 3].Value = L["GroupBuy"];
        sheet.Cells[row, 4].Value = L["ProductName"];
        sheet.Cells[row, 5].Value = L["Attributes"];
        sheet.Cells[row, 6].Value = L["SKU"];
        sheet.Cells[row, 7].Value = L["Qty"];
        sheet.Cells[row, 8].Value = L["UnitPrice"];
        sheet.Cells[row, 9].Value = L["Total"];
        sheet.Cells[row, 10].Value = L["OrderStatus"];
        sheet.Cells[row, 1, row, 10].Style.Fill.PatternType = ExcelFillStyle.Solid;
        sheet.Cells[row, 1, row, 10].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
        row++;

        foreach (var summary in summaries)
        {
            sheet.Cells[row, 1].Value = summary.OrderNo;
            sheet.Cells[row, 2].Value = summary.CreationTime.ToString("yyyy-MM-dd HH:mm");
            sheet.Cells[row, 3].Value = summary.GroupBuy;
            sheet.Cells[row, 4].Value = summary.ProductName;
            sheet.Cells[row, 5].Value = summary.Attributes;
            sheet.Cells[row, 6].Value = summary.SKU;
            sheet.Cells[row, 7].Value = summary.Quantity;
            sheet.Cells[row, 8].Value = summary.UnitPrice;
            sheet.Cells[row, 9].Value = summary.Total;
            sheet.Cells[row, 10].Value = L[summary.ShippingStatus.ToString()];

            row++;
        }
    }

    string Upper(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return L[value].ToString().ToUpper();
    }
}
