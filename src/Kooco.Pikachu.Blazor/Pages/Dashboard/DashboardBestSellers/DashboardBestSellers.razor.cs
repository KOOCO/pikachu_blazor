using Kooco.Pikachu.Dashboards;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Kooco.Pikachu.Blazor.Pages.Dashboard.DashboardBestSellers;

public partial class DashboardBestSellers
{
    [Parameter]
    public IReadOnlyList<DashboardBestSellerDto> Values { get; set; } = [];
}