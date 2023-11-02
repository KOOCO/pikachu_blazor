using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.ElectronicInvoiceSettings
{
    public interface IElectronicInvoiceSettingAppService : IApplicationService
    {
        Task<ElectronicInvoiceSettingDto> GetSettingAsync();
        Task<ElectronicInvoiceSettingDto> CreateAsyc(CreateUpdateElectronicInvoiceDto input);
        Task<ElectronicInvoiceSettingDto> UpdateAsyc(Guid Id, CreateUpdateElectronicInvoiceDto input);
    }
}