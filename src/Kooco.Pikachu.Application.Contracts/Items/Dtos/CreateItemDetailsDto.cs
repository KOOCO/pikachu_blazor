using System;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateItemDetailsDto
{
    public string ItemName { get; set; }
    public string ItemStyleAttribute { get; set; }
    public string ItemStyleOptions { get; set; }
    public string Sku { get; set; }
    public float SellingPrice { get; set; }
    public float GroupBuyPrice { get; set; }
    public float InventoryAccount { get; set; }
    public float SaleableQuantity { get; set; }
    public float? PreOrderableQuantity { get; set; }
    public float? SaleablePreOrderQuantity { get; set; }

    public int? LimitQuantity { get; set; }

    public string? Attribute1Value { get; set; }
    public string? Attribute2Value { get; set; }
    public string? Attribute3Value { get; set; }
}