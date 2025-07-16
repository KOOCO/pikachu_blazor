using System;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Orders
{
    public class OrderInvoiceDto : AuditedEntityDto<Guid>
    {
        public Guid OrderId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime IssueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string InvoiceDetails { get; set; } = string.Empty;
    }
}