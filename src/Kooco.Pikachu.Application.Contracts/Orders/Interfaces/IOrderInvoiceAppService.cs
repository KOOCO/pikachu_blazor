using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces;
public interface IOrderInvoiceAppService : IApplicationService
{
    Task<string> CreateInvoiceAsync(Guid orderId);
    Task CreateVoidInvoiceAsync(Guid orderId, string reason);
    Task CreateCreditNoteAsync(Guid orderId);
}