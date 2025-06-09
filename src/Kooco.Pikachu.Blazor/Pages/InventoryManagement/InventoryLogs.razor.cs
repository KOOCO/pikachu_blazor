using Blazorise;
using Kooco.Pikachu.InventoryManagement;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InventoryManagement;

public partial class InventoryLogs
{
    [Parameter] public EventCallback OnClose { get; set; }
    private InventoryDto Inventory { get; set; }
    private IReadOnlyList<InventoryDto> InventoryLogList { get; set; } = [];
    private bool IsLoading { get; set; }
    private bool IsExporting { get; set; }
    private Modal ModalRef { get; set; }
    private AdjustStockModal AdjustStockModalRef { get; set; }

    public async Task Show(InventoryDto inventory)
    {
        IsLoading = true;
        Inventory = inventory;
        await ModalRef.Show();
        StateHasChanged();
        await Task.Delay(2000);
        IsLoading = false;
        StateHasChanged();
    }

    async Task Hide()
    {
        await OnClose.InvokeAsync();
        await ModalRef.Hide();
    }

    async Task Export()
    {
        IsExporting = true;
        await Task.Delay(2000);
        IsExporting = false;
    }

    async Task AdjustStock()
    {
        await AdjustStockModalRef.Show();
    }
}