using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.Items.Dtos;
using System;

namespace Kooco.Pikachu.OrderItems
{
    public class OrderItemsCreateDto
    {
        public Guid? Id { get; set; }
        public Guid? ItemId { get; set; }
        public ItemDto? Item { get; set; }
        public Guid? SetItemId { get; set; }
        public SetItemDto? SetItem { get; set; }
        public Guid? FreebieId { get; set; }
        public FreebieDto? Freebie { get; set; }
        public ItemType ItemType { get; set; }
        public Guid OrderId { get; set; }
        public string? Spec { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public string? SKU { get; set; }
    }
}