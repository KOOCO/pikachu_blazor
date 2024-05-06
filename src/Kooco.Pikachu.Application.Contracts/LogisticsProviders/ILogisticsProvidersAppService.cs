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
        Task UpdatePostOfficeAsync(PostOfficeCreateUpdateDto input);
        Task UpdateSevenToElevenAsync(SevenToElevenCreateUpdateDto input);
        Task UpdateFamilyMartAsync(SevenToElevenCreateUpdateDto input);
        Task UpdateSevenToElevenFrozenAsync(SevenToElevenCreateUpdateDto input);
        Task UpdateBNormalAsync(BNormalCreateUpdateDto input);
        Task UpdateBFrozenAsync(BNormalCreateUpdateDto input);
        Task UpdateBFreezeAsync(BNormalCreateUpdateDto input);
        Task UpdateGreenWorldC2CAsync(GreenWorldLogisticsCreateUpdateDto input);
        Task UpdateFamilyMartC2CAsync(SevenToElevenCreateUpdateDto input);
        Task UpdateSevenToElevenC2CAsync(SevenToElevenCreateUpdateDto input);
    }
}