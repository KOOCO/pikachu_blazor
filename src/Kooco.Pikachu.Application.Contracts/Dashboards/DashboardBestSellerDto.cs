using System;

namespace Kooco.Pikachu.Dashboards;

public class DashboardBestSellerDto
{
    public Guid ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal Amount { get; set; }
    public int Percentage { get; set; }
}
