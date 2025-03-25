using Kooco.Pikachu.Dashboards;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Dashboard.DashboardCharts;

public partial class DashboardCharts
{
    private static string BarChartId => "dashboard-bar-chart";
    private static string DonutChartId => "dashboard-donut-chart";

    public async Task RenderCharts(DashboardChartsDto value)
    {
        var columnChartOptions = new
        {
            series = new[]
            {
                new { name = L["TotalOrders"].Value, data = value.Bar.TotalOrders.ToArray() },
                new { name = L["TotalSales"].Value, data = value.Bar.TotalSales.ToArray() },
            },
            xaxis = new { categories = value.Bar.Categories.ToArray() },
        };

        var donutChartOptions = new
        {
            series = value.Donut.Series.ToArray(),
            labels = value.Donut.Labels.ToArray()
        };

        if (value != null)
        {
            if (value.Bar != null)
                await JSRuntime.InvokeVoidAsync("renderBarChart", BarChartId, columnChartOptions);

            if (value.Donut != null)
                await JSRuntime.InvokeVoidAsync("renderDonutChart", DonutChartId, donutChartOptions);
        }

        StateHasChanged();
    }
}