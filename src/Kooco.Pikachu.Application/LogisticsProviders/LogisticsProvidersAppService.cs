using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticsProviders
{
    [RemoteService(IsEnabled = false)]
    public class LogisticsProvidersAppService : ApplicationService, ILogisticsProvidersAppService
    {
        private readonly IRepository<LogisticsProviderSettings, Guid> _logisticsProviderRepository;

        public LogisticsProvidersAppService(
            IRepository<LogisticsProviderSettings, Guid> logisticsProviderRepository
            )
        {
            _logisticsProviderRepository = logisticsProviderRepository;
        }

        public async Task UpdateGreenWorldAsync(GreenWorldLogisticsCreateUpdateDto input)
        {
            var greenWorld = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.GreenWorldLogistics);

            if (greenWorld != null)
            {
                greenWorld.IsEnabled = input.IsEnabled;
                greenWorld.StoreCode = input.StoreCode;
                greenWorld.HashKey = input.HashKey;
                greenWorld.HashIV = input.HashIV;
                greenWorld.SenderName = input.SenderName;
                greenWorld.SenderPhoneNumber = input.SenderPhoneNumber;
                greenWorld.LogisticsType = input.LogisticsType;
                greenWorld.LogisticsSubTypes = input.LogisticsSubTypes;
                greenWorld.FreeShippingThreshold = input.FreeShippingThreshold;
                greenWorld.Freight = input.Freight;

                await _logisticsProviderRepository.UpdateAsync(greenWorld);
            }
            else
            {
                greenWorld = new LogisticsProviderSettings
                {
                    IsEnabled = input.IsEnabled,
                    StoreCode = input.StoreCode,
                    HashKey = input.HashKey,
                    HashIV = input.HashIV,
                    SenderName = input.SenderName,
                    SenderPhoneNumber = input.SenderPhoneNumber,
                    LogisticsType = input.LogisticsType,
                    LogisticsSubTypes = input.LogisticsSubTypes,
                    FreeShippingThreshold = input.FreeShippingThreshold,
                    Freight = input.Freight,
                    LogisticProvider = LogisticProviders.GreenWorldLogistics
                };
                await _logisticsProviderRepository.InsertAsync(greenWorld);
            }
        }

        public async Task UpdateHomeDeliveryAsync(HomeDeliveryCreateUpdateDto input)
        {
            var homeDelivery = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.HomeDelivery);
            if (homeDelivery != null)
            {
                homeDelivery.IsEnabled = input.IsEnabled;
                homeDelivery.CustomTitle = input.CustomTitle;
                homeDelivery.FreeShippingThreshold = input.FreeShippingThreshold;
                homeDelivery.Freight = input.Freight;
                homeDelivery.MainIslands = input.MainIslands;
                homeDelivery.OuterIslands = input.OuterIslands;

                await _logisticsProviderRepository.UpdateAsync(homeDelivery);
            }
            else
            {
                homeDelivery = new LogisticsProviderSettings
                {
                    IsEnabled = input.IsEnabled,
                    CustomTitle = input.CustomTitle,
                    FreeShippingThreshold = input.FreeShippingThreshold,
                    Freight = input.Freight,
                    MainIslands = input.MainIslands,
                    OuterIslands = input.OuterIslands,
                    LogisticProvider = LogisticProviders.HomeDelivery
                };

                await _logisticsProviderRepository.InsertAsync(homeDelivery);
            }
        }

        public async Task<List<LogisticsProviderSettingsDto>> GetAllAsync()
        {
            var providers = await _logisticsProviderRepository.GetListAsync();
            return ObjectMapper.Map<List<LogisticsProviderSettings>, List<LogisticsProviderSettingsDto>>(providers);
        }
    }
}
