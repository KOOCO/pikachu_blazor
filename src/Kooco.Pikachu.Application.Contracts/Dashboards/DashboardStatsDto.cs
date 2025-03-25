namespace Kooco.Pikachu.Dashboards;

public class DashboardStatsDto
{
    public decimal TotalSales { get; set; }
    public decimal TotalPreviousSales { get; set; }
    public int SalesGrowthPercentage { get; set; }

    public int TotalOrders { get; set; }
    public int TotalPreviousOrders { get; set; }
    public int OrdersGrowthPercentage { get; set; }

    public int NewMembers { get; set; }
    public int PreviousNewMembers { get; set; }
    public int NewMembersGrowthPercentage { get; set; }

    public int TotalQuantity { get; set; }
    public int TotalPreviousQuantity { get; set; }
    public int QuantityGrowthPercentage { get; set; }
}
