using System;
using Volo.Abp.Application.Dtos;
using Kooco.Pikachu.Orders.Invoices;

namespace Kooco.Pikachu.Application.Contracts.Invoices.Dtos
{
    public class InvoiceDto : AuditedEntityDto<Guid>
    {
        public Guid OrderId { get; set; }
        public string InvoiceNo { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public string UnifiedBusinessNo { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TaxAmount { get; set; }
        public InvoiceTaxType TaxType { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceCreation CreationType { get; set; }
        public string VoidReason { get; set; }
        public DateTime IssueTime { get; set; }
        
        // 可能需要擴展的屬性
        public string OrderNo { get; set; }
        public string InvoiceTypeString => InvoiceType.ToString();
        public string InvoiceStatusString => InvoiceStatus.ToString();
        public string TaxTypeString => TaxType.ToString();
        public string CreationTypeString => CreationType.ToString();
    }
} 