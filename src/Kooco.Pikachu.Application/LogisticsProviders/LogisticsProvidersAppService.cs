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
                greenWorld.PlatFormId = input.PlatFormId;
                greenWorld.SenderAddress = input.SenderAddress;
                greenWorld.SenderPostalCode = input.SenderPostalCode;
                greenWorld.City = input.City;

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
                    PlatFormId = input.PlatFormId,
                    SenderAddress = input.SenderAddress,
                    SenderPostalCode = input.SenderPostalCode,
                    City = input.City,
                    LogisticProvider = LogisticProviders.GreenWorldLogistics
                };
                await _logisticsProviderRepository.InsertAsync(greenWorld);
            }
        }
        public async Task UpdateGreenWorldC2CAsync(GreenWorldLogisticsCreateUpdateDto input)
        {
            var greenWorld = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.GreenWorldLogisticsC2C);

            if (greenWorld != null)
            {
                greenWorld.IsEnabled = input.IsEnabled;
                greenWorld.StoreCode = input.StoreCode;
                greenWorld.HashKey = input.HashKey;
                greenWorld.HashIV = input.HashIV;
                greenWorld.SenderName = input.SenderName;
                greenWorld.SenderPhoneNumber = input.SenderPhoneNumber;
                greenWorld.PlatFormId = input.PlatFormId;
                greenWorld.SenderAddress = input.SenderAddress;
                greenWorld.SenderPostalCode = input.SenderPostalCode;
                greenWorld.City = input.City;

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
                    PlatFormId = input.PlatFormId,
                    SenderAddress = input.SenderAddress,
                    SenderPostalCode = input.SenderPostalCode,
                    City = input.City,
                    LogisticProvider = LogisticProviders.GreenWorldLogisticsC2C
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
                   
                    Freight = input.Freight,
                    MainIslands = input.MainIslands,
                    OuterIslands = input.OuterIslands,
                    LogisticProvider = LogisticProviders.HomeDelivery
                };

                await _logisticsProviderRepository.InsertAsync(homeDelivery);
            }
        }
        public async Task UpdatePostOfficeAsync(PostOfficeCreateUpdateDto input)
        {
            var postOffice = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.PostOffice);
            if (postOffice != null)
            {
             postOffice.Freight = input.Freight;
                postOffice.Weight= input.Weight;

                await _logisticsProviderRepository.UpdateAsync(postOffice);
            }
            else
            {
                postOffice = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                Weight = input.Weight,
                LogisticProvider = LogisticProviders.PostOffice
                };

                await _logisticsProviderRepository.InsertAsync(postOffice);
            }
        }
        public async Task UpdateSevenToElevenAsync(SevenToElevenCreateUpdateDto input)
        {
            var sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.SevenToEleven);
            if (sevenToEleven != null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                LogisticProvider = LogisticProviders.SevenToEleven
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        public async Task UpdateSevenToElevenC2CAsync(SevenToElevenCreateUpdateDto input)
        {
            var sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.SevenToElevenC2C);
            if (sevenToEleven != null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.SevenToElevenC2C
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        public async Task UpdateFamilyMartAsync(SevenToElevenCreateUpdateDto input)
        {
            var sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.FamilyMart);
            if (sevenToEleven != null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.FamilyMart
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        public async Task UpdateFamilyMartC2CAsync(SevenToElevenCreateUpdateDto input)
        {
            var sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.FamilyMartC2C);
            if (sevenToEleven != null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.FamilyMartC2C
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        public async Task UpdateSevenToElevenFrozenAsync(SevenToElevenCreateUpdateDto input)
        {
            var sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.SevenToElevenFrozen);
            if (sevenToEleven != null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.SevenToElevenFrozen
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        public async Task UpdateBNormalAsync(BNormalCreateUpdateDto input)
        {
            var bNormal = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.BNormal);
            if (bNormal != null)
            {
                bNormal.Freight = input.Freight;
                bNormal.OuterIslandFreight = input.OuterIslandFreight;
                bNormal.Size= input.Size;

                await _logisticsProviderRepository.UpdateAsync(bNormal);
            }
            else
            {
                bNormal = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    OuterIslandFreight = input.OuterIslandFreight,
                    Size = input.Size,
                    LogisticProvider = LogisticProviders.BNormal
                };

                await _logisticsProviderRepository.InsertAsync(bNormal);
            }
        }
        public async Task UpdateBFreezeAsync(BNormalCreateUpdateDto input)
        {
            var bFreeze = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.BFreeze);
            if (bFreeze != null)
            {
                bFreeze.Freight = input.Freight;
                bFreeze.OuterIslandFreight = input.OuterIslandFreight;
                bFreeze.Size = input.Size;

                await _logisticsProviderRepository.UpdateAsync(bFreeze);
            }
            else
            {
                bFreeze = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    OuterIslandFreight = input.OuterIslandFreight,
                    Size = input.Size,
                    LogisticProvider = LogisticProviders.BFreeze
                };

                await _logisticsProviderRepository.InsertAsync(bFreeze);
            }
        }
        public async Task UpdateBFrozenAsync(BNormalCreateUpdateDto input)
        {
            var bFrozen = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.BFrozen);
            if (bFrozen != null)
            {
                bFrozen.Freight = input.Freight;
                bFrozen.OuterIslandFreight = input.OuterIslandFreight;
                bFrozen.Size = input.Size;

                await _logisticsProviderRepository.UpdateAsync(bFrozen);
            }
            else
            {
                bFrozen = new LogisticsProviderSettings
                {
                    Freight = input.Freight,
                    OuterIslandFreight = input.OuterIslandFreight,
                    Size = input.Size,
                    LogisticProvider = LogisticProviders.BFrozen
                };

                await _logisticsProviderRepository.InsertAsync(bFrozen);
            }
        }
        public async Task<List<LogisticsProviderSettingsDto>> GetAllAsync()
        {
            var providers = await _logisticsProviderRepository.GetListAsync();
            return ObjectMapper.Map<List<LogisticsProviderSettings>, List<LogisticsProviderSettingsDto>>(providers);
        }
    }
}
