using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.InventoryManagement;

public class InventoryModel
{
    public Guid ItemId { get; set; }
    public Guid ItemDetailId { get; set; }
    public string ItemName { get; set; }
    public string? Attribute1 { get; set; }
    public string? Attribute2 { get; set; }
    public string? Attribute3 { get; set; }
    public string Attributes => string.Join(" / ", new[] { Attribute1, Attribute2, Attribute3 }.Where(attr => !string.IsNullOrEmpty(attr))) ?? "";
    public string? Sku { get; set; }
    public string? Warehouse { get; set; }
    public float CurrentStock { get; set; }
    public float AvailableStock { get; set; }
    public float PreOrderQuantity { get; set; }
    public float AvailablePreOrderQuantity { get; set; }
}
