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
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Nodes;
using Kooco.Pikachu.DeliveryTemperatureCosts;

namespace Kooco.Pikachu.LogisticsProviders;

[RemoteService(IsEnabled = false)]
public class LogisticsProvidersAppService : ApplicationService, ILogisticsProvidersAppService
{
    #region Inject
    private readonly IRepository<LogisticsProviderSettings, Guid> _logisticsProviderRepository;

    private readonly IDeliveryTemperatureCostAppService _DeliveryTemperatureAppService;

    private readonly IConfiguration _Configuration;
    #endregion

    #region Constructor
    public LogisticsProvidersAppService(
        IRepository<LogisticsProviderSettings, Guid> logisticsProviderRepository,
        IDeliveryTemperatureCostAppService DeliveryTemperatureAppService,
        IConfiguration Configuration
    )
    {
        _logisticsProviderRepository = logisticsProviderRepository;

        _DeliveryTemperatureAppService = DeliveryTemperatureAppService;

        _Configuration = Configuration;
    }
    #endregion

    #region Methods
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

    public async Task UpdateEcPayHomeDeliveryAsync(EcPayHomeDeliveryCreateUpdateDto input)
    {
        var ecPayHomeDelivery = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.EcPayHomeDelivery);

        if (ecPayHomeDelivery != null)
        {
            ecPayHomeDelivery.IsEnabled = input.IsEnabled;
            ecPayHomeDelivery.StoreCode = input.StoreCode;
            ecPayHomeDelivery.HashKey = input.HashKey;
            ecPayHomeDelivery.HashIV = input.HashIV;
            ecPayHomeDelivery.SenderName = input.SenderName;
            ecPayHomeDelivery.SenderPhoneNumber = input.SenderPhoneNumber;
            ecPayHomeDelivery.PlatFormId = input.PlatFormId;
            ecPayHomeDelivery.SenderAddress = input.SenderAddress;
            ecPayHomeDelivery.SenderPostalCode = input.SenderPostalCode;
            ecPayHomeDelivery.City = input.City;

            await _logisticsProviderRepository.UpdateAsync(ecPayHomeDelivery);
        }
        else
        {
            ecPayHomeDelivery = new LogisticsProviderSettings
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
                LogisticProvider = LogisticProviders.EcPayHomeDelivery
            };
            await _logisticsProviderRepository.InsertAsync(ecPayHomeDelivery);
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
        LogisticsProviderSettings? homeDelivery = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.HomeDelivery);

        if (homeDelivery is not null)
        {
            homeDelivery.IsEnabled = input.IsEnabled;
            homeDelivery.CustomTitle = input.CustomTitle;
            homeDelivery.Freight = input.Freight;
            homeDelivery.MainIslands = input.MainIslands;
            homeDelivery.OuterIslands = input.OuterIslands;
            homeDelivery.IsOuterIslands = input.IsOuterIslands;

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
                IsOuterIslands = input.IsOuterIslands,
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
            postOffice.IsEnabled = input.IsEnabled;
            postOffice.Freight = input.Freight;
            postOffice.Weight = input.Weight;

            await _logisticsProviderRepository.UpdateAsync(postOffice);
        }
        else
        {
            postOffice = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new()
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new()
            {
                IsEnabled = input.IsEnabled,
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
            sevenToEleven.IsEnabled = input.IsEnabled;
            sevenToEleven.Freight = input.Freight;
            sevenToEleven.Payment = input.Payment;

            await _logisticsProviderRepository.UpdateAsync(sevenToEleven);
        }
        else
        {
            sevenToEleven = new()
            {
                IsEnabled = input.IsEnabled,
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
            tCat.SenderPostalCode = zipCode;
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
                SenderPostalCode = zipCode,
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
            Addresses = new[] { new { Search = search } }
        };

        string jsonContent = JsonConvert.SerializeObject(request);

        StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(_Configuration["EcPay:SenderZipCode"], content);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();

            RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(responseBody);

            if (rootObject is not null && rootObject.IsOK is "Y")
                return rootObject.Data.Addresses.First().PostNumber.Remove(0, 3).Replace("-", string.Empty);
        }

        return string.Empty;
    }

    public async Task UpdateBNormalAsync(BNormalCreateUpdateDto input)
    {
        var bNormal = await _logisticsProviderRepository.FirstOrDefaultAsync(x => x.LogisticProvider == LogisticProviders.BNormal);
        if (bNormal != null)
        {
            bNormal.IsEnabled = input.IsEnabled;
            bNormal.Freight = input.Freight;
            bNormal.OuterIslandFreight = input.OuterIslandFreight;
            bNormal.Size = input.Size;

            await _logisticsProviderRepository.UpdateAsync(bNormal);
        }
        else
        {
            bNormal = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            tCatNormal.IsEnabled = input.IsEnabled;
            tCatNormal.Freight = input.Freight;
            tCatNormal.OuterIslandFreight = input.OuterIslandFreight;
            tCatNormal.Size = input.Size;
            tCatNormal.Payment = input.Payment;
            tCatNormal.TCatPaymentMethod = input.TCatPaymentMethod;

            await _logisticsProviderRepository.UpdateAsync(tCatNormal);
        }

        else
        {
            tCatNormal = new()
            {
                IsEnabled = input.IsEnabled,
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
            tCatFreeze.IsEnabled = input.IsEnabled;
            tCatFreeze.Freight = input.Freight;
            tCatFreeze.OuterIslandFreight = input.OuterIslandFreight;
            tCatFreeze.Size = input.Size;
            tCatFreeze.Payment = input.Payment;
            tCatFreeze.TCatPaymentMethod = input.TCatPaymentMethod;

            await _logisticsProviderRepository.UpdateAsync(tCatFreeze);
        }

        else
        {
            tCatFreeze = new()
            {
                IsEnabled = input.IsEnabled,
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
            tCatFrozen.IsEnabled = input.IsEnabled;
            tCatFrozen.Freight = input.Freight;
            tCatFrozen.OuterIslandFreight = input.OuterIslandFreight;
            tCatFrozen.Size = input.Size;
            tCatFrozen.Payment = input.Payment;
            tCatFrozen.TCatPaymentMethod = input.TCatPaymentMethod;

            await _logisticsProviderRepository.UpdateAsync(tCatFrozen);
        }

        else
        {
            tCatFrozen = new()
            {
                IsEnabled = input.IsEnabled,
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
            bFreeze.IsEnabled = input.IsEnabled;
            bFreeze.Freight = input.Freight;
            bFreeze.OuterIslandFreight = input.OuterIslandFreight;
            bFreeze.Size = input.Size;

            await _logisticsProviderRepository.UpdateAsync(bFreeze);
        }
        else
        {
            bFreeze = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
            bFrozen.IsEnabled = input.IsEnabled;
            bFrozen.Freight = input.Freight;
            bFrozen.OuterIslandFreight = input.OuterIslandFreight;
            bFrozen.Size = input.Size;

            await _logisticsProviderRepository.UpdateAsync(bFrozen);
        }
        else
        {
            bFrozen = new LogisticsProviderSettings
            {
                IsEnabled = input.IsEnabled,
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
        List<LogisticsProviderSettingsDto> providers = ObjectMapper.Map<List<LogisticsProviderSettings>, List<LogisticsProviderSettingsDto>>(
            await _logisticsProviderRepository.GetListAsync()
        );

        foreach (LogisticsProviderSettingsDto provider in providers)
        {
            provider.LogisticProviderName = provider.LogisticProvider.ToString();
        }

        return providers;
    }

    public async Task<JsonObject> GetAsync(string shippingMethod)
    {
        if (shippingMethod.ToUpper() is "SELFPICKUP") return new JsonObject { { "Freight", 0 } };

        List<LogisticsProviderSettingsDto> providers = ObjectMapper.Map<List<LogisticsProviderSettings>, List<LogisticsProviderSettingsDto>>(
            await _logisticsProviderRepository.GetListAsync()
        );

        foreach (LogisticsProviderSettingsDto provider in providers)
        {
            provider.LogisticProviderName = provider.LogisticProvider.ToString();
        }

        var grrenB2C = providers.Where(x => x.LogisticProviderName.ToUpper() == "GREENWORLDLOGISTICS").FirstOrDefault();
        var grrenC2C = providers.Where(x => x.LogisticProviderName.ToUpper() == "GREENWORLDLOGISTICSC2C").FirstOrDefault();
        var tcat = providers.Where(x => x.LogisticProviderName.ToUpper() == "TCAT").FirstOrDefault();

        string deliveryNameToLogisticName = ConvertDeliveryNameToLogisticName(shippingMethod);

        var result = providers.Where(x => x.LogisticProviderName.ToUpper() == deliveryNameToLogisticName).FirstOrDefault();

        if (shippingMethod.ToUpper() is "DELIVEREDBYSTORE")
        {
            List<JsonObject> DBSkeyValuePairs = [];

            List<DeliveryTemperatureCostDto> deliveryTemps = await _DeliveryTemperatureAppService.GetListAsync();

            foreach (DeliveryTemperatureCostDto deliveryTemp in deliveryTemps)
            {
                deliveryNameToLogisticName = ConvertDeliveryNameToLogisticName(deliveryTemp.DeliveryMethod.ToString());

                result = providers.Where(x => x.LogisticProviderName.ToUpper() == deliveryNameToLogisticName).FirstOrDefault();

                if (deliveryNameToLogisticName is "HOMEDELIVERY")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {
                        { "Provider", result.LogisticProviderName },
                        { "IsEnable", result.IsEnabled },
                        { "Cost", result.Freight },
                        { "MainIslands", result.MainIslands },
                        { "IsOuterIslands", result.IsOuterIslands },
                        { "OuterIslands", result.OuterIslands }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "POSTOFFICE")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "Weight", result.Weight },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "FAMILYMART")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "SEVENTOELEVEN")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "SEVENTOELEVENFREEZE")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "SEVENTOELEVENFROZEN")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "BNORMAL")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "OuterIslandFreight", result.OuterIslandFreight },
                        { "Size", result.Size.ToString() },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "BFREEZE")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "OuterIslandFreight", result.OuterIslandFreight },
                        { "Size", result.Size.ToString() },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "BFROZEN")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "OuterIslandFreight", result.OuterIslandFreight },
                        { "Size", result.Size.ToString() },
                         {"MerchantID",grrenB2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "FAMILYMARTC2C")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"MerchantID",grrenC2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "SEVENTOELEVENC2C")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"MerchantID",grrenC2C.StoreCode }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "TCATNORMAL")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "OuterIslandFreight", result.OuterIslandFreight },
                        { "Size", result.Size.ToString() },
                        { "Payment", result.Payment },
                        { "PaymentMethod", result.TCatPaymentMethod.ToString() },
                         {"CustomerID",tcat.CustomerId }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "TCATFREEZE")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "OuterIslandFreight", result.OuterIslandFreight },
                        { "Size", result.Size.ToString() },
                        { "Payment", result.Payment },
                        { "PaymentMethod", result.TCatPaymentMethod.ToString() },
                         {"CustomerID",tcat.CustomerId }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "TCATFREOZEN")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "Freight", result.Freight },
                        { "OuterIslandFreight", result.OuterIslandFreight },
                        { "Size", result.Size.ToString() },
                        { "Payment", result.Payment },
                        { "PaymentMethod", result.TCatPaymentMethod.ToString() },
                         {"CustomerID",tcat.CustomerId }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "TCAT711NORMAL")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"CustomerID",tcat.CustomerId }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "TCAT711FREEZE")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                         {"CustomerID",tcat.CustomerId }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
                if (deliveryNameToLogisticName is "TCAT711FROZEN")
                {
                    JsonObject keyValuePairs = new JsonObject
                    {

                        { "Provider", result.LogisticProviderName },
                        { "COST", result.Freight },
                        { "Payment", result.Payment },
                        {"CustomerID",tcat.CustomerId }
                    };
                    DBSkeyValuePairs.Add(keyValuePairs);

                    continue;
                }
            }

            return CombineJsonObjects(DBSkeyValuePairs, deliveryTemps);
        }

        if (result is null)
        {
            return null; // or throw an exception, depending on how you want to handle this case
        }

        if (deliveryNameToLogisticName is "HOMEDELIVERY")
        {
            JsonObject keyValuePairs = new JsonObject
            {
                { "Provider", result.LogisticProviderName },
                { "IsEnable", result.IsEnabled },
                { "Cost", result.Freight },
                { "MainIslands", result.MainIslands },
                { "IsOuterIslands", result.IsOuterIslands },
                { "OuterIslands", result.OuterIslands }
            };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "POSTOFFICE")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "Weight", result.Weight },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "FAMILYMART")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "SEVENTOELEVEN")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "SEVENTOELEVENFREEZE")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "SEVENTOELEVENFROZEN")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "BNORMAL")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "OuterIslandFreight", result.OuterIslandFreight },
            { "Size", result.Size.ToString() },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "BFREEZE")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "OuterIslandFreight", result.OuterIslandFreight },
            { "Size", result.Size.ToString() },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "BFROZEN")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "OuterIslandFreight", result.OuterIslandFreight },
            { "Size", result.Size.ToString() },
             {"MerchantID",grrenB2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "FAMILYMARTC2C")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"MerchantID",grrenC2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "SEVENTOELEVENC2C")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"MerchantID",grrenC2C.StoreCode }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "TCATNORMAL")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "OuterIslandFreight", result.OuterIslandFreight },
            { "Size", result.Size.ToString() },
            { "Payment", result.Payment },
            { "PaymentMethod", result.TCatPaymentMethod.ToString() },
             {"CustomerID",tcat.CustomerId }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "TCATFREEZE")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "OuterIslandFreight", result.OuterIslandFreight },
            { "Size", result.Size.ToString() },
            { "Payment", result.Payment },
            { "PaymentMethod", result.TCatPaymentMethod.ToString() },
             {"CustomerID",tcat.CustomerId }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "TCATFROZEN")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "Freight", result.Freight },
            { "OuterIslandFreight", result.OuterIslandFreight },
            { "Size", result.Size.ToString() },
            { "Payment", result.Payment },
            { "PaymentMethod", result.TCatPaymentMethod.ToString() },
             {"CustomerID",tcat.CustomerId }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "TCAT711NORMAL")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"CustomerID",tcat.CustomerId }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "TCAT711FREEZE")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
             {"CustomerID",tcat.CustomerId }
        };

            return keyValuePairs;
        }
        if (deliveryNameToLogisticName is "TCAT711FROZEN")
        {
            JsonObject keyValuePairs = new JsonObject
        {

            { "Provider", result.LogisticProviderName },
            { "COST", result.Freight },
            { "Payment", result.Payment },
            {"CustomerID",tcat.CustomerId }
        };

            return keyValuePairs;
        }
        return null; // Return null or a default value if `shippingMethod` is not "HOMEDELIVERY"
    }
    #endregion

    private JsonObject CombineJsonObjects(List<JsonObject> jsonObjects, List<DeliveryTemperatureCostDto> deliveryTemps)
    {
        JsonObject combinedObject = new();

        foreach (DeliveryTemperatureCostDto deliveryTemp in deliveryTemps)
        {
            JsonObject jsonObject = jsonObjects[deliveryTemps.IndexOf(deliveryTemp)];

            combinedObject.Add(deliveryTemp.Temperature.ToString(), jsonObject);
        }
        return combinedObject;
    }

    private string ConvertDeliveryNameToLogisticName(string deliveryName)
    {
        List<string> deliveryMethods = [
            DeliveryMethod.FamilyMart1.ToString().ToUpper(),
            DeliveryMethod.HomeDelivery.ToString().ToUpper(),
            DeliveryMethod.PostOffice.ToString().ToUpper(),
            DeliveryMethod.SevenToEleven1.ToString().ToUpper(),
            DeliveryMethod.SevenToElevenFrozen.ToString().ToUpper(),
            DeliveryMethod.BlackCat1.ToString().ToUpper(),
            DeliveryMethod.BlackCatFreeze.ToString().ToUpper(),
            DeliveryMethod.BlackCatFrozen.ToString().ToUpper(),
            DeliveryMethod.FamilyMartC2C.ToString().ToUpper(),
            DeliveryMethod.SevenToElevenC2C.ToString().ToUpper(),
            DeliveryMethod.TCatDeliveryNormal.ToString().ToUpper(),
            DeliveryMethod.TCatDeliveryFreeze.ToString().ToUpper(),
            DeliveryMethod.TCatDeliveryFrozen.ToString().ToUpper(),
            DeliveryMethod.TCatDeliverySevenElevenNormal.ToString().ToUpper(),
            DeliveryMethod.TCatDeliverySevenElevenFreeze.ToString().ToUpper(),
            DeliveryMethod.TCatDeliverySevenElevenFrozen.ToString().ToUpper()
        ];

        List<string> logisticProviders = [
            LogisticProviders.FamilyMart.ToString().ToUpper(),
            LogisticProviders.HomeDelivery.ToString().ToUpper(),
            LogisticProviders.PostOffice.ToString().ToUpper(),
            LogisticProviders.SevenToEleven.ToString().ToUpper(),
            LogisticProviders.SevenToElevenFrozen.ToString().ToUpper(),
            LogisticProviders.BNormal.ToString().ToUpper(),
            LogisticProviders.BFreeze.ToString().ToUpper(),
            LogisticProviders.BFrozen.ToString().ToUpper(),
            LogisticProviders.FamilyMartC2C.ToString().ToUpper(),
            LogisticProviders.SevenToElevenC2C.ToString().ToUpper(),
            LogisticProviders.TCatNormal.ToString().ToUpper(),
            LogisticProviders.TCatFreeze.ToString().ToUpper(),
            LogisticProviders.TCatFrozen.ToString().ToUpper(),
            LogisticProviders.TCat711Normal.ToString().ToUpper(),
            LogisticProviders.TCat711Freeze.ToString().ToUpper(),
            LogisticProviders.TCat711Frozen.ToString().ToUpper()
        ];

        foreach (string deliveryMethod in deliveryMethods)
        {
            if (deliveryName.ToUpper() == deliveryMethod)
            {
                return logisticProviders[deliveryMethods.IndexOf(deliveryMethod)];
            }
        }

        return string.Empty;
    }
}

public class RootObject
{
    public string SrvTranId { get; set; }

    public string IsOK { get; set; }

    public string Message { get; set; }

    public Data Data { get; set; }
}

public class Data
{
    public List<Address> Addresses { get; set; }
}

public class Address
{
    public string Search { get; set; }

    public string PostNumber { get; set; }
}