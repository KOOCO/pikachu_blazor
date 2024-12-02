using Kooco.Pikachu.SalesReports;
using System.Threading.Tasks;
using Kooco.Pikachu.Extensions;
using System.Collections.Generic;
using System;
using Kooco.Pikachu.Items.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.SalesReport;

public partial class SalesReport
{
    private List<SalesReportDto> SalesReportList { get; set; }
    private GetSalesReportDto Filters { get; set; }
    private List<KeyValueDto> GroupBuyLookup { get; set; }
    private bool IsLoading { get; set; } = false;

    public SalesReport()
    {
        SalesReportList = [];
        Filters = new();
        GroupBuyLookup = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await ResetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                GroupBuyLookup = await GroupBuyAppService.GetGroupBuyLookupAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
    }

    private async Task GetSalesReportAsync()
    {
        try
        {
            IsLoading = true;
            await InvokeAsync(StateHasChanged);
            SalesReportList = await SalesReportAppService.GetSalesReportAsync(Filters);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task FilterAsync()
    {
        await GetSalesReportAsync();
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

    private async Task OnShowDetails(SalesReportDto input)
    {
        input.ShowDetails = !input.ShowDetails;
        if (input.Details.Count == 0)
        {
            input.Details = await SalesReportAppService.GetGroupBuySalesReportAsync(input.Date);
        }
    }
}