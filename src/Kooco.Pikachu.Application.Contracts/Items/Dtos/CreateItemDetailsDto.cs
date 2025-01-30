using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Items.Dtos;

[Serializable]
public class CreateItemDetailsDto
{
    public Guid? Id { get; set; }
    public string ItemName { get; set; }
    public string Sku { get; set; }
    [Range(0, 99999, ErrorMessage = "Price must be a positive number.")]
    [RegularExpression(@"^\d{1,5}(\.\d{1,2})?$", ErrorMessage = "Price can have up to 2 decimal places.")]
    public float SellingPrice { get; set; }
    [Range(0, 99999, ErrorMessage = "Price must be a positive number.")]
    [RegularExpression(@"^\d{1,5}(\.\d{1,2})?$", ErrorMessage = "Price can have up to 2 decimal places.")]
    public float GroupBuyPrice { get; set; }
    [Range(0, 99999, ErrorMessage = "Price must be a positive number.")]
    [RegularExpression(@"^\d{1,5}(\.\d{1,2})?$", ErrorMessage = "Price can have up to 2 decimal places.")]
    public float Cost { get; set; }
    public string InventoryAccount { get; set; }
    [Range(0, 99999, ErrorMessage = "Quantity must be a positive number and less than 99,999.")]
    public float SaleableQuantity { get; set; }
    [Range(0, 99999, ErrorMessage = "Quantity must be a positive number and less than 99,999.")]
    public float? PreOrderableQuantity { get; set; }
    [Range(0, 99999, ErrorMessage = "Quantity must be a positive number and less than 99,999.")]
    public float? SaleablePreOrderQuantity { get; set; }
    [Range(0, 99999, ErrorMessage = "Quantity must be a positive number and less than 99,999.")]
    public int? LimitQuantity { get; set; }
    [Range(0, 99999, ErrorMessage = "Quantity must be a positive number and less than 99,999.")]
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