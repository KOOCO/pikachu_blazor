using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace Kooco.Pikachu.Blazor.Pages.InventoryManagement;

public partial class Inventory
{
    private string PageTitle { get; set; } = "InventoryManagement";
    protected List<BreadcrumbItem> BreadcrumbItems { get; set; } = [];
    private IReadOnlyList<InventoryDto> InventoryList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private GetInventoryDto Filters { get; set; }
    private List<InventoryDto> Selected { get; set; }
    private bool IsLoading { get; set; }
    private bool IsExportingSelected { get; set; }
    private bool IsExportingAll { get; set; }

    public Inventory()
    {
        Filters = new();
    }

    protected override async Task OnInitializedAsync()
    {
        PageTitle = L[PageTitle];
        BreadcrumbItems.Add(new BreadcrumbItem(PageTitle));
        await base.OnInitializedAsync();
    }

    private async Task GetInventoryListAsync()
    {
        try
        {
            IsLoading = true;
            var result = await InventoryAppService.GetListAsync(
                new GetInventoryDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    ItemId = Filters.ItemId,
                    Warehouse = Filters.Warehouse,
                    Sku = Filters.Sku,
                    Attributes = Filters.Attributes,
                    MinCurrentStock = Filters.MinCurrentStock,
                    MaxCurrentStock = Filters.MaxCurrentStock,
                    MinAvailableStock = Filters.MinAvailableStock,
                    MaxAvailableStock = Filters.MaxAvailableStock
                }
            );

            InventoryList = result.Items;
            TotalCount = (int)result.TotalCount;
            IsLoading = false;
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<InventoryDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetInventoryListAsync();

        await InvokeAsync(StateHasChanged);
    }

    void Edit(InventoryDto inventory)
    {
        NavigationManager.NavigateTo($"Inventory-Management/Inventory-Logs/{inventory.ItemId}/{inventory.ItemDetailId}");
    }

    private async Task ExportAsync(bool exportAll)
    {
        try
        {
            SetExportingState(exportAll, true);

            var items = exportAll ? null : Selected;
            if (!exportAll && (items is null || items.Count == 0)) return;

            var inventoryFile = await InventoryAppService.GetListAsExcelAsync(items);
            await ExcelDownloadHelper.DownloadExcelAsync(inventoryFile);

            Selected = [];
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            SetExportingState(exportAll, false);
        }
    }

    private void SetExportingState(bool exportAll, bool isExporting)
    {
        IsExportingAll = exportAll && isExporting;
        IsExportingSelected = !exportAll && isExporting;
        StateHasChanged();
    }

    private async Task ApplyFilters()
    {
        CurrentPage = 1;

        await GetInventoryListAsync();

        await InvokeAsync(StateHasChanged);
    }

    private static bool RowSelectableHandler(RowSelectableEventArgs<InventoryDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
}