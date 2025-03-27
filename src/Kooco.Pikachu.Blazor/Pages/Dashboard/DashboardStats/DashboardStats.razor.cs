using Kooco.Pikachu.Dashboards;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.Dashboard.DashboardStats;

public partial class DashboardStats
{
    [Parameter]
    public DashboardStatsDto Value { get; set; }

    [Parameter]
    public bool Loading { get; set; }

    private List<StatsModel> Stats { get; set; } = [];

    public DashboardStats()
    {
        Stats = [
            new(1, "Total Orders", "fa-shopping-cart", "", 0, 0),
            new(2, "New Members", "fa-users", "purple", 0, 0),
            new(3, "Total Sales", "fa-dollar", "orange", 0, 0, Constant.CurrencySymbol),
            new(4, "Total Products", "fa-box", "green", 0, 0)
        ];
    }

    protected override void OnParametersSet()
    {
        for (int i = 0; i < Stats.Count; i++)
        {
            (Stats[i].Value, Stats[i].GrowthOrLoss) = i switch
            {
                0 => (Value.TotalOrders, Value.OrdersGrowthPercentage),
                1 => (Value.NewMembers, Value.NewMembersGrowthPercentage),
                2 => ((int)Value.TotalSales, Value.SalesGrowthPercentage),
                3 => (Value.TotalQuantity, Value.QuantityGrowthPercentage),
                _ => (0, 0)
            };
        }
    }

    public class StatsModel
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public string? ValuePrefix { get; set; }
        public int Value { get; set; }
        public int GrowthOrLoss { get; set; }
        public string DisplayValue { get { return $"{ValuePrefix}{Value:N0}"; } }

        public StatsModel(int index, string name, string icon, string color, int value, int growthOrLoss, string? valuePrefix = null)
        {
            Index = index;
            Name = name;
            Icon = icon;
            Color = color;
            ValuePrefix = valuePrefix;
            Value = value;
            GrowthOrLoss = growthOrLoss;
        }
    }
}