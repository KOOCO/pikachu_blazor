using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.LogisticsProviders
{
    public interface ILogisticsProvidersAppService : IApplicationService
    {
        Task<List<LogisticsProviderSettingsDto>> GetAllAsync();
        Task UpdateGreenWorldAsync(GreenWorldLogisticsCreateUpdateDto input);
        Task UpdateHomeDeliveryAsync(HomeDeliveryCreateUpdateDto input);
    }
}