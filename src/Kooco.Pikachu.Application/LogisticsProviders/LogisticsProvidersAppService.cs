using Kooco.Pikachu.EnumValues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
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
        public async Task UpdateTCat711NormalAsync(TCat711NormalCreateUpdate input)
        {
            LogisticsProviderSettings? sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.TCat711Normal);

            if (sevenToEleven is not null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new ()
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.TCat711Normal
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        
        public async Task UpdateTCat711FreezeAsync(TCat711FreezeCreateUpdateDto input)
        {
            LogisticsProviderSettings? sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.TCat711Freeze);

            if (sevenToEleven is not null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new ()
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.TCat711Freeze
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        
        public async Task UpdateTCat711FrozenAsync(TCat711FrozenCreateUpdateDto input)
        {
            LogisticsProviderSettings? sevenToEleven = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.TCat711Frozen);

            if (sevenToEleven is not null)
            {
                sevenToEleven.Freight = input.Freight;
                sevenToEleven.Payment = input.Payment;

                await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
            }
            else
            {
                sevenToEleven = new ()
                {
                    Freight = input.Freight,
                    Payment = input.Payment,
                    LogisticProvider = LogisticProviders.TCat711Frozen
                };

                await _logisticsProviderRepository.InsertAsync(sevenToEleven);
            }
        }
        
        public async Task UpdateTCatAsync(TCatLogisticsCreateUpdateDto entity)
        {
            LogisticsProviderSettings? tCat = await _logisticsProviderRepository.FirstOrDefaultAsync(f => f.LogisticProvider == LogisticProviders.TCat);

            string zipCode = await GenerateSenderZIPCodeAsync(entity.CustomerId, entity.CustomerToken, entity.SenderAddress);

            if (tCat is not null)
            {
                tCat.LogisticProvider = LogisticProviders.TCat;
                tCat.IsEnabled = entity.IsEnabled;
                tCat.CustomerId = entity.CustomerId;
                tCat.CustomerToken = entity.CustomerToken;
                tCat.SenderName = entity.SenderName;
                tCat.SenderPhoneNumber = entity.SenderPhoneNumber;
                tCat.SenderAddress = entity.SenderAddress;
                tCat.TCatShippingLabelForm = entity.TCatShippingLabelForm;
                tCat.TCatPickingListForm = entity.TCatPickingListForm;
                tCat.TCatShippingLabelForm711 = entity.TCatShippingLabelForm711;
                tCat.ReverseLogisticShippingFee = entity.ReverseLogisticShippingFee;
                tCat.DeclaredValue = entity.DeclaredValue;

                await _logisticsProviderRepository.UpdateAsync(tCat);
            }

            else
            {
                tCat = new()
                {
                    LogisticProvider = LogisticProviders.TCat,
                    IsEnabled = entity.IsEnabled,
                    CustomerId = entity.CustomerId,
                    CustomerToken = entity.CustomerToken,
                    SenderName = entity.SenderName,
                    SenderPhoneNumber = entity.SenderPhoneNumber,
                    SenderAddress = entity.SenderAddress,
                    TCatShippingLabelForm = entity.TCatShippingLabelForm,
                    TCatPickingListForm = entity.TCatPickingListForm,
                    TCatShippingLabelForm711 = entity.TCatShippingLabelForm711,
                    ReverseLogisticShippingFee = entity.ReverseLogisticShippingFee,
                    DeclaredValue = entity.DeclaredValue
                };

                await _logisticsProviderRepository.InsertAsync(tCat);
            }
        }

        public async Task<string> GenerateSenderZIPCodeAsync(string customerId, string customerToken, string search)
        {
            using HttpClient client = new();

            var request = new
            {
                CustomerId = customerId,
                CustomerToken = customerToken,
                Addresses = new[]
                {
                    new { Search = search }
                }
            };

            string jsonContent = JsonConvert.SerializeObject(request);

            StringContent content = new (jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://egs.suda.com.tw:8443/api/Egs/ParsingAddress", content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                return string.Empty;
            }
            else
            {
                throw new Exception($"Request failed with status code: {response.StatusCode}");
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

        public async Task UpdateTCatNormalAsync(TCatNormalCreateUpdateDto input)
        {
            LogisticsProviderSettings? tCatNormal = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.TCatNormal);
            
            if (tCatNormal is not null)
            {
                tCatNormal.Freight = input.Freight;
                tCatNormal.OuterIslandFreight = input.OuterIslandFreight;
                tCatNormal.Size= input.Size;
                tCatNormal.Payment = input.Payment;
                tCatNormal.TCatPaymentMethod = input.TCatPaymentMethod;

                await _logisticsProviderRepository.UpdateAsync(tCatNormal);
            }
            
            else
            {
                tCatNormal = new ()
                {
                    Freight = input.Freight,
                    OuterIslandFreight = input.OuterIslandFreight,
                    Size = input.Size,
                    Payment = input.Payment,
                    TCatPaymentMethod = input.TCatPaymentMethod,
                    LogisticProvider = LogisticProviders.TCatNormal
                };

                await _logisticsProviderRepository.InsertAsync(tCatNormal);
            }
        }
        public async Task UpdateTCatFreezeAsync(TCatFreezeCreateUpdateDto input)
        {
            LogisticsProviderSettings? tCatFreeze = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.TCatFreeze);
            
            if (tCatFreeze is not null)
            {
                tCatFreeze.Freight = input.Freight;
                tCatFreeze.OuterIslandFreight = input.OuterIslandFreight;
                tCatFreeze.Size= input.Size;
                tCatFreeze.Payment = input.Payment;
                tCatFreeze.TCatPaymentMethod = input.TCatPaymentMethod;

                await _logisticsProviderRepository.UpdateAsync(tCatFreeze);
            }
            
            else
            {
                tCatFreeze = new ()
                {
                    Freight = input.Freight,
                    OuterIslandFreight = input.OuterIslandFreight,
                    Size = input.Size,
                    Payment = input.Payment,
                    TCatPaymentMethod = input.TCatPaymentMethod,
                    LogisticProvider = LogisticProviders.TCatFreeze
                };

                await _logisticsProviderRepository.InsertAsync(tCatFreeze);
            }
        }
        public async Task UpdateTCatFrozenAsync(TCatFrozenCreateUpdateDto input)
        {
            LogisticsProviderSettings? tCatFrozen = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.TCatFrozen);
            
            if (tCatFrozen is not null)
            {
                tCatFrozen.Freight = input.Freight;
                tCatFrozen.OuterIslandFreight = input.OuterIslandFreight;
                tCatFrozen.Size= input.Size;
                tCatFrozen.Payment = input.Payment;
                tCatFrozen.TCatPaymentMethod = input.TCatPaymentMethod;

                await _logisticsProviderRepository.UpdateAsync(tCatFrozen);
            }
            
            else
            {
                tCatFrozen = new ()
                {
                    Freight = input.Freight,
                    OuterIslandFreight = input.OuterIslandFreight,
                    Size = input.Size,
                    Payment = input.Payment,
                    TCatPaymentMethod = input.TCatPaymentMethod,
                    LogisticProvider = LogisticProviders.TCatFrozen
                };

                await _logisticsProviderRepository.InsertAsync(tCatFrozen);
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
