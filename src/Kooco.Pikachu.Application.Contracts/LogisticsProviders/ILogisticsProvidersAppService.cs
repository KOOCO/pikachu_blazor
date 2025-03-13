using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.LogisticsProviders
{
    public interface ILogisticsProvidersAppService : IApplicationService
    {
        Task<List<LogisticsProviderSettingsDto>> GetAllAsync();
        Task UpdateTCat711FrozenAsync(TCat711FrozenCreateUpdateDto input);
        Task UpdateTCat711FreezeAsync(TCat711FreezeCreateUpdateDto input);
        Task UpdateTCat711NormalAsync(TCat711NormalCreateUpdate input);
        Task UpdateTCatFrozenAsync(TCatFrozenCreateUpdateDto input);
        Task UpdateTCatFreezeAsync(TCatFreezeCreateUpdateDto input);
        Task UpdateTCatNormalAsync(TCatNormalCreateUpdateDto input);
        Task UpdateTCatAsync(TCatLogisticsCreateUpdateDto entity);
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
        Task<JsonObject> GetAsync(string shippingMethod);
        Task UpdateEcPayHomeDeliveryAsync(EcPayHomeDeliveryCreateUpdateDto input);
    }
}