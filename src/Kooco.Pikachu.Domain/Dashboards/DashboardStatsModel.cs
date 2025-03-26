using System;

namespace Kooco.Pikachu.Dashboards;

public class DashboardStatsModel
{
    public decimal TotalSales { get; set; }
    public decimal TotalPreviousSales { get; set; }
    public int SalesGrowthPercentage => CalculateGrowthPercentage(TotalSales, TotalPreviousSales);

    public int TotalOrders { get; set; }
    public int TotalPreviousOrders { get; set; }
    public int OrdersGrowthPercentage => CalculateGrowthPercentage(TotalOrders, TotalPreviousOrders);

    public int NewMembers { get; set; }
    public int PreviousNewMembers { get; set; }
    public int NewMembersGrowthPercentage => CalculateGrowthPercentage(NewMembers, PreviousNewMembers);

    public int TotalQuantity { get; set; }
    public int TotalPreviousQuantity { get; set; }
    public int QuantityGrowthPercentage => CalculateGrowthPercentage(TotalQuantity, TotalPreviousQuantity);

    private static int CalculateGrowthPercentage(decimal current, decimal previous)
    {
        if (previous == 0)
            return current > 0 ? 100 : 0;

        return (int)(((current - previous) / Math.Abs(current)) * 100);
    }
}

