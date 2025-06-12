using Blazorise;
using Kooco.Pikachu.InventoryManagement;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InventoryManagement;

public partial class AdjustStockModal
{
    [Parameter] public EventCallback<bool> OnClosed { get; set; }
    private CreateInventoryLogDto Entity { get; set; } = new();
    private IReadOnlyList<InventoryStockType> StockTypeOptions { get; set; } = [.. Enum.GetValues<InventoryStockType>()];
    private IReadOnlyList<InventoryActionType> ActionTypeOptions { get; set; } = [.. Enum.GetValues<InventoryActionType>().Where(x => x != InventoryActionType.ItemSold)];
    private bool IsLoading { get; set; }
    private Modal ModalRef { get; set; }
    private Validations ValidationsRef { get; set; }

    public async Task Show(InventoryDto inventory)
    {
        Entity = new()
        {
            ItemId = inventory.ItemId,
            ItemDetailId = inventory.ItemDetailId,
            Sku = inventory.Sku,
            Attributes = inventory.Attributes,
        };
        StateHasChanged();
        await Task.Delay(100);
        await ValidationsRef.ClearAll();
        await ModalRef.Show();
    }

    public async Task Hide()
    {
        await OnClosed.InvokeAsync();
        await ModalRef.Hide();
    }

    public async Task Hide(bool saved = false)
    {
        await OnClosed.InvokeAsync(saved);
        await ModalRef.Hide();
    }

    private async Task Save()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;
                await InventoryLogAppService.CreateAsync(Entity);
                await Hide(true);
            }
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
}