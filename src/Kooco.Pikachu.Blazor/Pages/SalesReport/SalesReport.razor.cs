using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Reports;
using Kooco.Pikachu.SalesReports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                await ResetAsync();
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
            StateHasChanged();
            SalesReportList = await SalesReportAppService.GetSalesReportAsync(Filters);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    private async Task FilterAsync()
    {
        await GetSalesReportAsync();
    }

    private async Task ResetAsync()
    {
        Filters = new();
        await GetSalesReportAsync();
    }

    private async Task UnitChangedAsync(ReportCalculationUnits reportCalculationUnit)
    {
        Filters.ReportCalculationUnit = reportCalculationUnit;
        await InvokeAsync(StateHasChanged);
        await GetSalesReportAsync();
    }

    private async Task OnShowDetails(SalesReportDto input)
    {
        input.ShowDetails = !input.ShowDetails;
        if (input.Details.Count == 0)
        {
            input.Details = await SalesReportAppService.GetGroupBuySalesReportAsync(input.StartDate, input.EndDate, Filters.GroupBuyId);
        }
    }
}