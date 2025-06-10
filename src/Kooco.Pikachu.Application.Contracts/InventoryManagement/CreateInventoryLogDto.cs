using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.InventoryManagement;

public class CreateInventoryLogDto
{
    public Guid ItemId { get; set; }

    public Guid ItemDetailId { get; set; }

    public string? Sku { get; set; }

    public string? Attributes { get; set; }

    [Required]
    public InventoryStockType? StockType { get; set; }

    [Required]
    public InventoryActionType? ActionType { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int? Amount { get; set; }

    [Required]
    [MaxLength(InventoryLogConsts.MaxDescriptionLength)]
    public string? Description { get; set; }
}
