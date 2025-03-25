using System.Collections.Generic;

namespace Kooco.Pikachu.Dashboards;

public class DashboardChartsModel
{
    public DashboardDonutChartModel Donut { get; set; }
    public DashboardBarChartModel Bar { get; set; }
}

public class DashboardDonutChartModel
{
    public List<int> Series { get; set; }
    public List<string?> Labels { get; set; }
}

public class DashboardBarChartModel
{
    public List<string> Categories { get; set; }
    public List<int> TotalSales { get; set; }
    public List<int> TotalOrders { get; set; }
}