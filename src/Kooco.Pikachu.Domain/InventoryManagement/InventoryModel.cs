using System;
using System.Linq;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryModel
{
    public Guid ItemId { get; set; }
    public Guid ItemDetailId { get; set; }
    public string ItemName { get; set; }
    public string Attributes { get; set; }
    public string? Sku { get; set; }
    public string? Warehouse { get; set; }
    public float CurrentStock { get; set; }
    public float AvailableStock { get; set; }
    public float PreOrderQuantity { get; set; }
    public float AvailablePreOrderQuantity { get; set; }
}
