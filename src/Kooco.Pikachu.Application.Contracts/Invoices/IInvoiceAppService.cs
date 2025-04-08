using System;
using System.Threading.Tasks;
using Kooco.Pikachu.Application.Contracts.Invoices.Dtos;
using Kooco.Pikachu.Invoices.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Application.Contracts.Invoices
{
    public interface IInvoiceAppService : 
        ICrudAppService<
            InvoiceDto,
            Guid,
            InvoicePagedAndSortedResultRequestDto,
            CreateInvoiceDto,
            UpdateInvoiceDto>
    {
        Task<InvoiceDto> GetByOrderIdAsync(Guid orderId);
        Task<InvoiceDto> VoidInvoiceAsync(Guid id, string reason);
    }
} 