﻿using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;

namespace Kooco.Pikachu.Orders
{
    public class CreateOrderDto
    {
        public bool IsIndividual { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public PaymentMethods? PaymentMethod { get; set; }
        public InvoiceType? InvoiceType { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? UniformNumber { get; set; }
        public string? TaxTitle { get; set; }
        public bool IsAsSameBuyer { get; set; }
        public string? RecipientName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? RecipientEmail { get; set; }
        public DeliveryMethod? DeliveryMethod { get; set; }
        public string? ShippingNumber { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? District { get; set; }
        public string? Road { get; set; }
        public string? AddressDetails { get; set; }
        public string? Remarks { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ReceivingTime? ReceivingTime { get; set; }
        public Guid GroupBuyId { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemsCreateDto> OrderItems { get; set; }
        public OrderReturnStatus? ReturnStatus { get; set; }
        public OrderType? OrderType { get; set; }
    }
}
