using Kooco.Pikachu.EnumValues;
using System;

namespace Kooco.Pikachu.Dashboards;

public class DashboardOrdersDto
{
    public Guid Id { get; set; }
    public string OrderNo { get; set; }
    public string? CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public ShippingStatus ShippingStatus { get; set; }
    public string? GroupBuyName { get; set; }
}
