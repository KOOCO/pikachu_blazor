using Blazorise;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using System.Threading.Tasks;
using System;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;
using System.Collections.Generic;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Microsoft.AspNetCore.Components;
using Kooco.Pikachu.LogisticsProviders;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.DeliveryTemperatureCosts;

public partial class DeliveryTemperatureCost
{
    #region Inject
    public Guid Id { get; set; }

    private List<DeliveryTemperatureCostDto> temperatureCosts = [];

    protected Validations CreateValidationsRef;
    
    private readonly IObjectMapper _objectMapper;
    
    private readonly IUiMessageService _uiMessageService;
    
    private readonly IDeliveryTemperatureCostAppService _appService;

    private int Index = 0;

    private LogisticProviders? LogisticProviderNormal;

    private LogisticProviders? LogisticProviderFreeze;
    
    private LogisticProviders? LogisticProviderFrozen;

    private readonly ILogisticsProvidersAppService _LogisticsProvidersAppService;

    private List<LogisticsProviderSettingsDto> AllLogisticsProviderSetting = [];
    #endregion

    #region Constructor
    public DeliveryTemperatureCost(
        IDeliveryTemperatureCostAppService appService, 
        IObjectMapper objectMapper, 
        IUiMessageService uiMessageService,
        ILogisticsProvidersAppService LogisticsProvidersAppService
    )
    {
        _appService = appService;
        _objectMapper = objectMapper;
        _uiMessageService = uiMessageService;
        _LogisticsProvidersAppService = LogisticsProvidersAppService;
    }
    #endregion

    #region Methods
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetCostsAysnc();

            AllLogisticsProviderSetting = await _LogisticsProvidersAppService.GetAllAsync();
            
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    private async Task GetCostsAysnc() 
    { 
        temperatureCosts = await _appService.GetListAsync();
    }

    protected virtual async Task UpdateCostAsync()
    {
        List<UpdateDeliveryTemperatureCostDto> costs = 
            _objectMapper.Map<List<DeliveryTemperatureCostDto>, List<UpdateDeliveryTemperatureCostDto>>(temperatureCosts);

        await _appService.UpdateCostAsync(costs);

        StateHasChanged();

        await _uiMessageService.Success(L["CostUpdateSuccessfully"]);
    }

    public List<LogisticProviders> GetLogisticsProviders(ItemStorageTemperature temperature)
    {
        List<LogisticProviders> logisticProviders = [];

        if (temperature is ItemStorageTemperature.Normal)
        {
            if (AllLogisticsProviderSetting.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.GreenWorldLogistics)?.IsEnabled ?? false)
                logisticProviders.Add(LogisticProviders.GreenWorldLogistics);

            if (AllLogisticsProviderSetting.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.GreenWorldLogisticsC2C)?.IsEnabled ?? false)
                logisticProviders.Add(LogisticProviders.GreenWorldLogisticsC2C);

            if (AllLogisticsProviderSetting.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat)?.IsEnabled ?? false)
                logisticProviders.Add(LogisticProviders.TCat);
        }

        else if (temperature is ItemStorageTemperature.Freeze || temperature is ItemStorageTemperature.Frozen)
        {
            if (AllLogisticsProviderSetting.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.GreenWorldLogistics)?.IsEnabled ?? false)
                logisticProviders.Add(LogisticProviders.GreenWorldLogistics);

            if (AllLogisticsProviderSetting.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat)?.IsEnabled ?? false)
                logisticProviders.Add(LogisticProviders.TCat);
        }

        return logisticProviders;
    }

    public void OnDeliveryMethodChange(ChangeEventArgs e, DeliveryTemperatureCostDto entity)
    {
        if (e.Value is "100") entity.DeliveryMethod = null;

        else entity.DeliveryMethod = Enum.Parse<DeliveryMethod>(e.Value.ToString());

        StateHasChanged();
    }

    public void OnLogisticProviderChange(ChangeEventArgs e, DeliveryTemperatureCostDto entity)
    {
        List<ItemStorageTemperature> temperatureCosts = [ItemStorageTemperature.Normal, ItemStorageTemperature.Freeze, ItemStorageTemperature.Frozen];

        foreach (ItemStorageTemperature temperature in temperatureCosts)
        {
            if (entity.Temperature == temperature)
            {
                if (e.Value is "100") entity.LogisticProvider = null;

                else entity.LogisticProvider = Enum.Parse<LogisticProviders>(e.Value.ToString());
            }
        }

        StateHasChanged();
    }

    public void OnLogisticProviderChange(ChangeEventArgs e, ItemStorageTemperature temperature)
    {
        if (temperature is ItemStorageTemperature.Normal)
        {
            if (e.Value is "100") LogisticProviderNormal = null;

            else LogisticProviderNormal = Enum.Parse<LogisticProviders>(e.Value.ToString());
        }

        if (temperature is ItemStorageTemperature.Freeze)
        {
            if (e.Value is "100") LogisticProviderFreeze = null;

            else LogisticProviderFreeze = Enum.Parse<LogisticProviders>(e.Value.ToString());
        }

        if (temperature is ItemStorageTemperature.Frozen)
        {
            if (e.Value is "100") LogisticProviderFrozen = null;

            else LogisticProviderFrozen = Enum.Parse<LogisticProviders>(e.Value.ToString());
        }

        StateHasChanged();
    }

    public List<DeliveryMethod> GetDeliveryMethods(ItemStorageTemperature? temperature, LogisticProviders? logistic) 
    {
        if (temperature is not null && logistic is not null)
        {
            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.GreenWorldLogistics)
                return [DeliveryMethod.PostOffice, DeliveryMethod.FamilyMart1, DeliveryMethod.SevenToEleven1, DeliveryMethod.BlackCat1];

            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.GreenWorldLogisticsC2C)
                return [DeliveryMethod.FamilyMartC2C, DeliveryMethod.SevenToElevenC2C];

            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.TCat)
                return [DeliveryMethod.TCatDeliveryNormal, DeliveryMethod.TCatDeliverySevenElevenNormal];

            if (temperature is ItemStorageTemperature.Freeze && logistic is LogisticProviders.GreenWorldLogistics)
                return [DeliveryMethod.BlackCatFreeze];

            if (temperature is ItemStorageTemperature.Freeze && logistic is LogisticProviders.TCat)
                return [DeliveryMethod.TCatDeliveryFreeze, DeliveryMethod.TCatDeliverySevenElevenFreeze];

            if (temperature is ItemStorageTemperature.Frozen && logistic is LogisticProviders.GreenWorldLogistics)
                return [DeliveryMethod.BlackCatFrozen];

            if (temperature is ItemStorageTemperature.Frozen && logistic is LogisticProviders.TCat)
                return [DeliveryMethod.TCatDeliveryFrozen, DeliveryMethod.TCatDeliverySevenElevenFrozen];
        }

        return [];
    }
    #endregion
}