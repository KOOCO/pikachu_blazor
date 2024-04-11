using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public interface IElectronicInvoiceAppService:IApplicationService
    {

         Task CreateInvoiceAsync(Guid orderId);
        Task CreateVoidInvoiceAsync(Guid orderId,string reason);
        Task CreateCreditNoteAsync(Guid orderId);
    }
}
