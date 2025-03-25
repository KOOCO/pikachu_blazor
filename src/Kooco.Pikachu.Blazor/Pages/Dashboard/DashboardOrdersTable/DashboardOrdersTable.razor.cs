using Kooco.Pikachu.Dashboards;
using Microsoft.AspNetCore.Components;

namespace Kooco.Pikachu.Blazor.Pages.Dashboard.DashboardOrdersTable;

public partial class DashboardOrdersTable
{
    [Parameter]
    public DashboardFiltersDto Filters { get; set; }
}