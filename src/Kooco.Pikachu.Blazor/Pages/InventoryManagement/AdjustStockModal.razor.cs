using Blazorise;
using Kooco.Pikachu.InventoryManagement;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.InventoryManagement;

public partial class AdjustStockModal
{
    private CreateStockAdjustmentDto Entity { get; set; } = new();
    private IReadOnlyList<InventoryStockType> StockTypeOptions { get; set; } = [.. Enum.GetValues<InventoryStockType>()];
    private IReadOnlyList<InventoryActionType> ActionTypeOptions { get; set; } = [.. Enum.GetValues<InventoryActionType>().Where(x => x != InventoryActionType.ItemSold)];
    private bool IsLoading { get; set; }
    private Modal ModalRef { get; set; }
    private Validations ValidationsRef { get; set; }

    public async Task Show()
    {
        Entity = new();
        await ValidationsRef.ClearAll();
        await ModalRef.Show();
    }

    public async Task Hide()
    {
        await ModalRef.Hide();
    }

    private async Task Save()
    {
        await ValidationsRef.ValidateAll();
        await Task.CompletedTask;
    }
}

public class CreateStockAdjustmentDto
{
    [Required]
    public InventoryStockType? StockType { get; set; }

    [Required]
    public InventoryActionType? ActionType { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int? Amount { get; set; }

    [Required]
    [MaxLength(512)]
    public string? Description { get; set; }
}