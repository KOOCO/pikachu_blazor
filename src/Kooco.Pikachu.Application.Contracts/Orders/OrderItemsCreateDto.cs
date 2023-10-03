using System;

namespace Kooco.Pikachu.Orders
{
    public class OrderItemsCreateDto
    {
        public Guid ItemId { get; set; }
        public Guid OrderId { get; set; }
        public string? Attribute1Value { get; set; }
        public string? Attribute2Value { get; set; }
        public string? Attribute3Value { get; set; }
        public int Quantity { get; set; }
    }
}