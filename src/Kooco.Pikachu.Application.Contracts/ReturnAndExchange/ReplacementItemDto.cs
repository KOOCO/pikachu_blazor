using Kooco.Pikachu.Items.Dtos;
using System;

namespace Kooco.Pikachu.ReturnAndExchange;

public class ReplacementItemDto
{
    public Guid OrderItemId { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public string Spec { get; set; }
    public int SelectedQuantity { get; set; }
    public int ReplacementQuantity { get; set; }
    public decimal ItemPrice { get; set; }
    public decimal ReplacementPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public ItemWithItemTypeDto? Item { get; set; }
    public ItemDetailWithItemTypeDto? Detail { get; set; }
    public ItemPricingDto? Pricing { get; set; }
}
