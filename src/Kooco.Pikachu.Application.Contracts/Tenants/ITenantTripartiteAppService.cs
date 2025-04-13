using Kooco.Pikachu.Tenants.ElectronicInvoiceSettings;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Tenants
{
    public interface ITenantTripartiteAppService : IApplicationService
    {
        Task<ElectronicInvoiceSettingDto> GetSettingAsync();
        Task<ElectronicInvoiceSettingDto> CreateAsyc(CreateUpdateElectronicInvoiceDto input);
        Task<ElectronicInvoiceSettingDto> UpdateAsyc(Guid Id, CreateUpdateElectronicInvoiceDto input);
    }
}