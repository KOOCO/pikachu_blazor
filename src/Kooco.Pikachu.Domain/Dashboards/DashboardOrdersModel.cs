using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Dashboards;

public class DashboardOrdersWithCountModel
{
    public long TotalCount { get; set; }
    public List<DashboardOrdersModel> Items { get; set; }
}

public class DashboardOrdersModel
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public string? CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public ShippingStatus ShippingStatus { get; set; }
    public string? ItemName { get; set; }
}