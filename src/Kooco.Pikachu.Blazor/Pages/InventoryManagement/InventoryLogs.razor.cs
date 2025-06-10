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
    private IReadOnlyList<InventoryLogDto> InventoryLogList { get; set; } = [];
    private bool IsLoading { get; set; }
    private bool IsExporting { get; set; }
    private AdjustStockModal AdjustStockModalRef { get; set; }

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
        await GetLogsAsync();
        await base.OnInitializedAsync();
    }

    async Task GetLogsAsync()
    {
        try
        {
            IsLoading = true;
            InventoryLogList = await InventoryLogAppService.GetListAsync(ItemId, ItemDetailId);
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
        IsExporting = true;
        await Task.Delay(2000);
        IsExporting = false;
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
}