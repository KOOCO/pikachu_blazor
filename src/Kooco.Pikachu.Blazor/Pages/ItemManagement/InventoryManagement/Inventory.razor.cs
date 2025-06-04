using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.InventoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace Kooco.Pikachu.Blazor.Pages.ItemManagement.InventoryManagement;

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
        await GetInventoryListAsync();
        await base.OnInitializedAsync();
    }

    private async Task GetInventoryListAsync()
    {
        try
        {
            var result = await InventoryAppService.GetListAsync(
                new GetInventoryDto
                {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter = Filters.Filter,
                    ItemId = Filters.ItemId,
                    MinCurrentStock = Filters.MinCurrentStock,
                    MaxCurrentStock = Filters.MaxCurrentStock,
                    MinAvailableStock = Filters.MinAvailableStock,
                    MaxAvailableStock = Filters.MaxAvailableStock
                }
            );

            InventoryList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
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

    private void Edit(InventoryDto member)
    {
        //NavigationManager.NavigateTo("/Members/Details/" + member.Id);
    }

    private async Task ExportAsync()
    {
        if (Selected is not { Count: > 0 }) return;
        try
        {
            IsExportingSelected = true;
            await Task.Delay(2000);
            //await ExcelDownloadHelper.DownloadExcelAsync(SelectedMembers, L["Members"] + ".xlsx");
            Selected = [];
            IsExportingSelected = false;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            IsExportingSelected = false;
        }
    }

    private async Task ExportAllAsync()
    {
        try
        {
            IsExportingAll = true;
            await Task.Delay(2000);
            //var allMembers = await MemberAppService.GetAllAsync();
            //await ExcelDownloadHelper.DownloadExcelAsync(allMembers, L["Members"] + ".xlsx");
            IsExportingAll = false;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
            IsExportingAll = false;
        }
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