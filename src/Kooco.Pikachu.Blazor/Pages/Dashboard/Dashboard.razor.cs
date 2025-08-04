using Blazorise.DataGrid;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Dashboards;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Dashboard;

public partial class Dashboard
{
    private readonly List<ReportCalculationUnits> PeriodOptions;
    private DashboardFiltersDto Filters { get; set; } = new();
    private List<KeyValueDto> GroupBuyOptions { get; set; } = [];
    private int PageSize { get; } = 5;
    private int CurrentPage { get; set; } = 1;
    private int RecentOrdersCount { get; set; }

    private DashboardStatsDto DashboardStats { get; set; } = new();
    private IReadOnlyList<DashboardOrdersDto> RecentOrdersList { get; set; } = [];
    private IReadOnlyList<DashboardBestSellerDto> BestSeller { get; set; } = [];

    private DashboardCharts.DashboardCharts _dashboardCharts;

    private bool Loading { get; set; } = true;
    private bool Exporting { get; set; }
    public Dashboard()
    {
        var units = Enum.GetValues<ReportCalculationUnits>();
        PeriodOptions = [.. units];
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ApplyFilters();
            GroupBuyOptions = await GroupBuyAppService.GetAllGroupBuyLookupAsync();
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DashboardOrdersDto> e)
    {
        CurrentPage = e.Page;

        await GetRecentOrders();

        await InvokeAsync(StateHasChanged);
    }

    private async Task GetRecentOrders()
    {
        try
        {
            Filters.MaxResultCount = PageSize;
            Filters.SkipCount = (CurrentPage - 1) * PageSize;

            var orderData = await DashboardAppService.GetRecentOrdersAsync(Filters);
            RecentOrdersList = orderData.Items;
            RecentOrdersCount = (int)orderData.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ApplyFilters()
    {
        try
        {
            Loading = true;
            DashboardStats = await DashboardAppService.GetDashboardStatsAsync(Filters);
            await GetRecentOrders();
            BestSeller = await DashboardAppService.GetBestSellerItemsAsync(Filters);
            await _dashboardCharts.RenderCharts(await DashboardAppService.GetDashboardChartsAsync(Filters));
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    async Task Export()
    {
        try
        {
            Exporting = true;
            var excelData = await DashboardAppService.GetSalesReportExcelAsync(Filters);
            var fileName = string.Format("{0}_{1}_to_{2}.xlsx",
                L["DashboardSalesReportFileName"],
                Filters.StartDate?.ToString("yyyy-MM-dd"),
                Filters.EndDate?.ToString("yyyy-MM-dd")
                );
            await ExcelDownloadHelper.DownloadExcelAsync(excelData, fileName);
            Exporting = false;
        }
        catch (Exception ex)
        {
            Exporting = false;
            await HandleErrorAsync(ex);
        }
    }
}