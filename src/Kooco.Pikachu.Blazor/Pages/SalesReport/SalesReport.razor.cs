using Kooco.Pikachu.SalesReports;
using System.Threading.Tasks;
using Kooco.Pikachu.Extensions;
using System.Collections.Generic;
using System;

namespace Kooco.Pikachu.Blazor.Pages.SalesReport;

public partial class SalesReport
{
    private List<SalesReportDto> SalesReportList { get; set; }
    private GetSalesReportDto Filters { get; set; }

    public SalesReport()
    {
        SalesReportList = [];
        Filters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await ResetAsync();
    }

    private async Task GetSalesReportAsync()
    {
        try
        {
            SalesReportList = await SalesReportAppService.GetSalesReportAsync(Filters);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task ResetAsync()
    {
        Filters = new();
        await FilterDateRange(StringExtensions.Today);
    }

    private async Task FilterDateRange(string dateRange)
    {
        (Filters.StartDate, Filters.EndDate) = dateRange.FindFilterDateRange();

        await GetSalesReportAsync();
    }
}