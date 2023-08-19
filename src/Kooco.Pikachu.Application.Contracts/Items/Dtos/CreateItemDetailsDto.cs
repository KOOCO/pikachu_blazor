using System;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateItemDetailsDto
{
    public string Tag { get; set; }
    public string ItemStyleAttribute { get; set; }
    public string ItemStyleOptions { get; set; }
    public string Sku { get; set; }
    public float SalePrice { get; set; }
    public float GroupBuyPrice { get; set; }
    public float InventoryAccount { get; set; }
    public float SalableQuantity { get; set; }
    public float? PreOrderableQuantity { get; set; }
    public float? SaleablePreOrderQuantity { get; set; }
}