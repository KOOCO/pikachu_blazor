using System;

namespace Kooco.Pikachu.Orders
{
    public class OrderItemsCreateDto
    {
        public Guid ItemId { get; set; }
        public Guid OrderId { get; set; }
        public string? Spec { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
    }
}