using System;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryDto
{
    public Guid ItemId { get; set; }
    public Guid ItemDetailId { get; set; }
    public string ItemName { get; set; }
    public string? Attributes { get; set; }
    public string? Sku { get; set; }
    public string? Warehouse { get; set; }
    public int CurrentStock { get; set; }
    public int AvailableStock { get; set; }
    public int PreOrderQuantity { get; set; }
    public int AvailablePreOrderQuantity { get; set; }
}
