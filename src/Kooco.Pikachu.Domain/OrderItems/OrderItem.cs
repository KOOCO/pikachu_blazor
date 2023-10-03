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
        public string? Attribute1Value { get; set; }
        public string? Attribute2Value { get; set; }
        public string? Attribute3Value { get; set; }
        public int Quantity { get; set; }

        public OrderItem()
        {
            
        }

        public OrderItem(
            Guid id,
            Guid itemId,
            Guid orderId,
            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value,
            int quantity
            ) : base(id)
        {
            ItemId = itemId;
            OrderId = orderId;
            Attribute1Value = attribute1Value;
            Attribute2Value = attribute2Value;
            Attribute3Value = attribute3Value;
            Quantity = quantity;
        }
    }
}
