using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class OrderDto : FullAuditedEntityDto<Guid>
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

    }
}
