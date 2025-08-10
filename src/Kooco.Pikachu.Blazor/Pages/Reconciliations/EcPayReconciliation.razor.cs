using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Reconciliations;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Reconciliations;

public partial class EcPayReconciliation
{
    private IReadOnlyList<EcPayReconciliationRecordDto> Records { get; set; } = [];
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await GetEcPayReconciliationRecordsAsync();
    }

    private async Task GetEcPayReconciliationRecordsAsync()
    {
        try
        {
            var result = await EcPayReconciliationAppService.GetListAsync(
                new EcPayReconciliationRecordListInput
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting
                }
            );

            Records = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<EcPayReconciliationRecordDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetEcPayReconciliationRecordsAsync();

        await InvokeAsync(StateHasChanged);
    }

    void Navigate(Guid id)
    {
        NavigationManager.NavigateTo("OrderDetails/" + id);
    }
}