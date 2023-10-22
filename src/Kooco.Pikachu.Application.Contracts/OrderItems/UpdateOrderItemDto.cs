using System;

namespace Kooco.Pikachu.OrderItems
{
    public class UpdateOrderItemDto
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsQuantiyError { get; set; }
        public bool IsItemPriceError { get; set; }
    }
}
