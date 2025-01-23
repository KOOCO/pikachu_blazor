using System;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateItemDetailsDto
{
    public Guid? Id { get; set; }
    public string ItemName { get; set; }
    public string Sku { get; set; }
    public float SellingPrice { get; set; }
    public float GroupBuyPrice { get; set; }
    public float Cost { get; set; }
    public string InventoryAccount { get; set; }
    public float SaleableQuantity { get; set; }
    public float? PreOrderableQuantity { get; set; }
    public float? SaleablePreOrderQuantity { get; set; }

    public int? LimitQuantity { get; set; }
    public int? StockOnHand { get; set; }
    public string? Attribute1Value { get; set; }
    public string? Attribute2Value { get; set; }
    public string? Attribute3Value { get; set; }
    public string? ItemDescription { get; set; }
    public string? Image { get; set; }
    public bool Status { get; set; }
    public bool IsExpanded { get; set; }
    public int SortNo { get; set; }
}