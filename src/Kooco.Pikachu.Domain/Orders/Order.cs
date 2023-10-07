using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders
{
    public class Order : FullAuditedAggregateRoot<Guid>
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

        [ForeignKey(nameof(GroupBuyId))]
        public GroupBuy GroupBuy { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }

        public Order() { }

        public Order(
            [NotNull] Guid id,
            [NotNull] Guid groupBuyId,
            string orderNo,
            bool isIndividual,
            string? name,
            string? phone,
            string? email,
            PaymentMethods? paymentMethods,
            InvoiceType? invoiceType,
            string invoiceNumber,
            string? uniformNumber,
            bool isAsSameBuyer,
            string? name2,
            string? email2,
            string? phone2,
            DeliveryMethod? deliveryMethod,
            string? city,
            string? district,
            string? road,
            string? addressDetails,
            string? remarks,
            ReceivingTime? receivingTime,
            int totalQuantity,
            decimal totalAmount
         )
        {
            Id = id;
            GroupBuyId = groupBuyId;
            OrderNo = orderNo;
            IsIndividual = isIndividual;
            Name = name;
            Phone = phone;
            Email = email;
            PaymentMethod = paymentMethods;
            InvoiceType = invoiceType;
            InvoiceNumber = invoiceNumber;
            UniformNumber = uniformNumber;
            IsAsSameBuyer = isAsSameBuyer;
            Name2 = name2;
            Email2 = email2;
            Phone2 = phone2;
            DeliveryMethod = deliveryMethod;
            City = city;
            District = district;
            Road = road;
            AddressDetails = addressDetails;
            Remarks = remarks;
            ReceivingTime = receivingTime;
            TotalQuantity = totalQuantity; 
            TotalAmount = totalAmount;
            OrderStatus = OrderStatus.Open;
            OrderItems = new List<OrderItem>();
        }

        public void AddOrderItem(
            Guid id,
            Guid itemId,
            string? spec,
            decimal itemPrice,
            decimal totalAmount,
            int quantity
            )
        {
            OrderItems.Add(new OrderItem(
                    id,
                    itemId,
                    this.Id,
                    spec,
                    itemPrice,
                    totalAmount,
                    quantity
                    ));
        }
    }
}
