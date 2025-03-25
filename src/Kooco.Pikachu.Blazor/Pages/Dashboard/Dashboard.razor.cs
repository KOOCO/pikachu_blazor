using Kooco.Pikachu.Dashboards;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Reports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Dashboard;

public partial class Dashboard
{
    private readonly List<ReportCalculationUnits> PeriodOptions;
    private DashboardFiltersDto Filters { get; set; } = new();
    private List<KeyValueDto> GroupBuyOptions { get; set; } = [];

    private DashboardStatsDto DashboardStats { get; set; } = new();
    private DashboardChartsDto DashboardCharts { get; set; } = new();

    private DashboardCharts.DashboardCharts _dashboardCharts;
    private bool IsLoading { get; set; } = false;
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

    private async Task ApplyFilters()
    {
        try
        {
            IsLoading = true;
            DashboardStats = await DashboardAppService.GetDashboardStatsAsync(Filters);
            await _dashboardCharts.RenderCharts(await DashboardAppService.GetDashboardChartsAsync(Filters));
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
}