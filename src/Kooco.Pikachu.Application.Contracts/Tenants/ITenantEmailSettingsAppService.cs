using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Tenants
{
    public interface ITenantEmailSettingsAppService : IApplicationService
    {
        Task UpdateEmailSettingsAsync(CreateUpdateTenantEmailSettingsDto input);
        Task<TenantEmailSettingsDto> GetEmailSettingsAsync();
    }
}
