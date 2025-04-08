using Volo.Abp.Application.Dtos;
using Kooco.Pikachu.Orders.Invoices;

namespace Kooco.Pikachu.Application.Contracts.Invoices.Dtos
{
    public class InvoicePagedAndSortedResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public InvoiceStatus? InvoiceStatus { get; set; }
        public InvoiceType? InvoiceType { get; set; }
        public InvoiceTaxType? TaxType { get; set; }
    }
} 