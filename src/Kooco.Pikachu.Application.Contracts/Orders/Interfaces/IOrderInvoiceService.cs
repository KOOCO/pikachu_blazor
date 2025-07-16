using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for order invoice generation and management operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IOrderInvoiceService : IApplicationService
    {
        /// <summary>
        /// Generate invoice for an order
        /// </summary>
        Task<OrderInvoiceDto> GenerateInvoiceAsync(Guid orderId);

        /// <summary>
        /// Void an invoice for an order
        /// </summary>
        Task VoidInvoice(Guid orderId, string reason);

        /// <summary>
        /// Update invoice details for an order
        /// </summary>
        Task UpdateInvoiceDetailsAsync(Guid orderId, string invoiceNumber, string invoiceDetails);

        /// <summary>
        /// Send invoice to customer
        /// </summary>
        Task SendInvoiceAsync(Guid orderId);

        /// <summary>
        /// Get invoice status for an order
        /// </summary>
        Task<InvoiceStatusDto> GetInvoiceStatusAsync(Guid orderId);

        /// <summary>
        /// Process electronic invoice creation
        /// </summary>
        Task ProcessElectronicInvoiceAsync(Guid orderId);

        /// <summary>
        /// Create credit note for invoice
        /// </summary>
        Task CreditNoteInvoice(Guid orderId, string reason);
    }
}