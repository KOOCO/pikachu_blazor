using System.Collections.Generic;

namespace Kooco.Pikachu.Dashboards;

public class DashboardChartsDto
{
    public DashboardDonutChartDto Donut { get; set; } = new();
    public DashboardBarChartDto Bar { get; set; } = new();
}

public class DashboardDonutChartDto
{
    public List<int> Series { get; set; } = [];
    public List<string?> Labels { get; set; } = [];
}

public class DashboardBarChartDto
{
    public List<string> Categories { get; set; } = [];
    public List<int> TotalSales { get; set; } = [];
    public List<int> TotalOrders { get; set; } = [];
}