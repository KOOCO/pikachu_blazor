using Kooco.Pikachu.Items;
using System;
using Volo.Abp.Domain.Entities;

namespace Kooco.Pikachu.OrderItems
{
    public class OrderItem : Entity<Guid>
    {
        public Guid ItemId { get; set; }
        public Item Item { get; set; }
        public Guid OrderId { get; set; }
        public string? Spec { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderItem()
        {

        }
        public OrderItem(
            Guid id,
            Guid itemId,
            Guid orderId,
            string? spec,
            decimal itemPrice,
            decimal totalAmount,
            int quantity
            ) : base(id)
        {
            ItemId = itemId;
            OrderId = orderId;
            Spec = spec;
            ItemPrice = itemPrice;
            TotalAmount = totalAmount;
            Quantity = quantity;
        }
    }
}
