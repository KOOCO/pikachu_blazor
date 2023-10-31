using Blazorise.DataGrid;
using Blazorise;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Blazorise.LoadingIndicator;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class GroupBuyReportDetails
{
    [Parameter]
    public string Id { get; set; }
    private GroupBuyReportDetailsDto ReportDetails { get; set; }
    private List<OrderDto> Orders { get; set; } = new();
    private int TotalCount { get; set; }
    private OrderDto SelectedOrder { get; set; }
    private int PageIndex { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    private string? Sorting { get; set; }
    private string? Filter { get; set; }

    private readonly HashSet<Guid> ExpandedRows = new();
    private LoadingIndicator loading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            ReportDetails = await _groupBuyAppService.GetGroupBuyReportDetailsAsync(Guid.Parse(Id));
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
    }
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
    {
        PageIndex = e.Page - 1;
        await UpdateItemList();
        await InvokeAsync(StateHasChanged);
    }
    private async Task UpdateItemList()
    {
        try
        {
            await loading.Show();
            int skipCount = PageIndex * PageSize;
            var result = await _orderAppService.GetListAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                GroupBuyId = Guid.Parse(Id)
            });
            Orders = result?.Items.ToList() ?? new List<OrderDto>();
            TotalCount = (int?)result?.TotalCount ?? 0;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }
    
    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
        await UpdateItemList();
    }

    void ToggleRow(DataGridRowMouseEventArgs<OrderDto> e)
    {
        if (ExpandedRows.Contains(e.Item.Id))
        {
            ExpandedRows.Remove(e.Item.Id);
        }
        else
        {
            ExpandedRows.Add(e.Item.Id);
        }
    }
}
