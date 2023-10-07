using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class OrderDto : FullAuditedEntityDto<Guid>
    {
        public string OrderNo { get; set; }
        public bool IsIndividual { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public PaymentMethods? PaymentMethod { get; set; }
        public InvoiceType? InvoiceType { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? UniformNumber { get; set; }
        public bool IsAsSameBuyer { get; set; }
        public string? Name2 { get; set; }
        public string? Phone2 { get; set; }
        public string? Email2 { get; set; }
        public DeliveryMethod? DeliveryMethod { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Road { get; set; }
        public string? AddressDetails { get; set; }
        public string? Remarks { get; set; }
        public ReceivingTime? ReceivingTime { get; set; }
        public Guid GroupBuyId { get; set; }
        public GroupBuyDto GroupBuy { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
