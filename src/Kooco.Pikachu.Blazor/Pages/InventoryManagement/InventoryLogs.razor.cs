using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.InventoryManagement;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace Kooco.Pikachu.Blazor.Pages.InventoryManagement;

public partial class InventoryLogs
{
    [Parameter] public Guid ItemId { get; set; }
    [Parameter] public Guid ItemDetailId { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    private string PageTitle { get; set; } = "";
    protected BreadcrumbItem InventoryBreadcrumb { get; set; }
    protected List<BreadcrumbItem> BreadcrumbItems { get; set; } = [];
    private InventoryDto Inventory { get; set; }
    private InventoryLogDto SelectedLog { get; set; }
    private IReadOnlyList<InventoryLogDto> InventoryLogList { get; set; } = [];
    private int PageSize { get; } = 30;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; }
    private int TotalCount { get; set; }
    private bool IsLoading { get; set; }
    private bool IsExporting { get; set; }
    private AdjustStockModal AdjustStockModalRef { get; set; }
    private Modal ViewModalRef { get; set; }

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        InventoryBreadcrumb = new BreadcrumbItem(L["InventoryManagement"], "Inventory-Management/Inventory");

        if (ItemId == Guid.Empty || ItemDetailId == Guid.Empty)
        {
            Close();
            return;
        }

        Inventory = await InventoryAppService.GetAsync(ItemId, ItemDetailId);
        PageTitle = string.Join(" - ", new[] { Inventory.ItemName, Inventory.Attributes }.Where(x => !string.IsNullOrWhiteSpace(x)));
        BreadcrumbItems.AddRange([InventoryBreadcrumb, new(PageTitle)]);
        
        await base.OnInitializedAsync();
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<InventoryLogDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;

        await GetLogsAsync();

        await InvokeAsync(StateHasChanged);
    }

    async Task GetLogsAsync()
    {
        try
        {
            IsLoading = true;
            StateHasChanged();

            var result = await InventoryLogAppService
                .GetListAsync(new GetInventoryLogListDto
                {
                    ItemId = ItemId,
                    ItemDetailId = ItemDetailId,
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting
                });
            
            InventoryLogList = result.Items;
            TotalCount = (int)result.TotalCount;
            StateHasChanged();
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

    void Close()
    {
        NavigationManager.NavigateTo(InventoryBreadcrumb.Url);
    }

    async Task Export()
    {
        try
        {
            IsExporting = true;

            var inventoryLogFile = await InventoryLogAppService.GetListAsExcelAsync(ItemId, ItemDetailId);

            await ExcelDownloadHelper.DownloadExcelAsync(inventoryLogFile);

        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsExporting = false;
        }
    }

    async Task AdjustStock()
    {
        await AdjustStockModalRef.Show(Inventory);
    }

    async Task OnStockAdjustment(bool saved)
    {
        if (saved)
        {
            await GetLogsAsync();
        }
    }

    async Task View(InventoryLogDto log)
    {
        if (log.ActionType == InventoryActionType.ItemSold)
        {
            NavigationManager.NavigateTo("Orders/OrderDetails/" + log.OrderId);
        }
        else
        {
            SelectedLog = log;
            await ViewModalRef.Show();
        }
    }

    async Task Refresh()
    {
        CurrentPage = 1;

        await GetLogsAsync();

        await InvokeAsync(StateHasChanged);
    }
}