using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;

namespace Kooco.Pikachu.Orders
{
    public class Order : FullAuditedAggregateRoot<Guid>
    {
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

        public ICollection<OrderItem> OrderItems { get; set; }

        public Order() { }
        
        public Order(
            [NotNull] Guid id,
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
            ReceivingTime? receivingTime
         )
        {
            Id = id;
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
            OrderItems = new List<OrderItem>();
        }

        public void AddOrderItem(
            Guid id,
            Guid itemId,
            string? attribute1Value,
            string? attribute2Value,
            string? attribute3Value,
            int quantity
            )
        {
            OrderItems.Add(new OrderItem(
                    id,
                    itemId,
                    this.Id,
                    attribute1Value,
                    attribute2Value,
                    attribute3Value,
                    quantity
                    ));
        }
    }
}
