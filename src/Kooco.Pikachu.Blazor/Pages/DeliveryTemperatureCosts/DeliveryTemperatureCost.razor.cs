using Blazorise;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsProviders;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.ObjectMapping;

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

    private bool IsAllowOffshoreIslands = false;

    private bool IsLogisticProviderActivated = false;

    private readonly Dictionary<LogisticProviders, List<LogisticProviders>> LogisticProvidersMap = new()
    {
        { LogisticProviders.EcPayHomeDelivery, [LogisticProviders.PostOffice, LogisticProviders.BNormal, LogisticProviders.BFreeze, LogisticProviders.BFrozen] },
        { LogisticProviders.GreenWorldLogistics, [LogisticProviders.FamilyMart, LogisticProviders.SevenToEleven, LogisticProviders.SevenToElevenFrozen] },
        { LogisticProviders.GreenWorldLogisticsC2C, [LogisticProviders.FamilyMartC2C, LogisticProviders.SevenToElevenC2C] },
        { LogisticProviders.TCat, [LogisticProviders.TCatNormal, LogisticProviders.TCatFreeze, LogisticProviders.TCatFrozen, LogisticProviders.TCat711Normal, LogisticProviders.TCat711Freeze, LogisticProviders.TCat711Frozen] },
    };
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

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await GetCostsAsync();

            AllLogisticsProviderSetting = await _LogisticsProvidersAppService.GetAllAsync();

            StateHasChanged();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    public void OnAllowOffShoreChanged(bool e)
    {
        IsAllowOffshoreIslands = e;

        foreach (DeliveryTemperatureCostDto temperatureCost in temperatureCosts)
        {
            temperatureCost.IsAllowOffShoreIslands = IsAllowOffshoreIslands;

            temperatureCost.DeliveryMethod = null;
        }

        StateHasChanged();
    }

    public void OnLogisticProviderActivationChanged(bool e)
    {
        IsLogisticProviderActivated = e;

        foreach (DeliveryTemperatureCostDto temperatureCost in temperatureCosts)
        {
            temperatureCost.IsLogisticProviderActivated = IsLogisticProviderActivated;
        }
    }

    private async Task GetCostsAsync()
    {
        temperatureCosts = await _appService.GetListAsync();

        IsAllowOffshoreIslands = temperatureCosts.GroupBy(g => g.IsAllowOffShoreIslands).Select(s => s.Key).FirstOrDefault();

        IsLogisticProviderActivated = temperatureCosts.GroupBy(g => g.IsLogisticProviderActivated).Select(s => s.Key).FirstOrDefault();
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
        var logisticProvidersMap = temperature != ItemStorageTemperature.Normal
            ? LogisticProvidersMap.Where(l => l.Key != LogisticProviders.GreenWorldLogisticsC2C).ToDictionary()
            : LogisticProvidersMap;

        return [.. logisticProvidersMap
            .Where(providerMap => AllLogisticsProviderSetting.Any(a => providerMap.Value.Contains(a.LogisticProvider) && a.IsEnabled))
            .Select(providerMap => providerMap.Key)];
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
        List<DeliveryMethod> deliveryMethods = [];
        if (temperature is not null && logistic is not null)
        {
            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.EcPayHomeDelivery)
                deliveryMethods = IsAllowOffshoreIslands ?
                    [DeliveryMethod.BlackCat1] :
                    [DeliveryMethod.PostOffice, DeliveryMethod.BlackCat1];

            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.GreenWorldLogistics)
                deliveryMethods = IsAllowOffshoreIslands ?
                    [DeliveryMethod.SevenToEleven1] :
                    [DeliveryMethod.FamilyMart1, DeliveryMethod.SevenToEleven1];

            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.GreenWorldLogisticsC2C)
                deliveryMethods = IsAllowOffshoreIslands ?
                    [DeliveryMethod.SevenToElevenC2C] :
                    [DeliveryMethod.FamilyMartC2C, DeliveryMethod.SevenToElevenC2C];

            if (temperature is ItemStorageTemperature.Normal && logistic is LogisticProviders.TCat)
                deliveryMethods = IsAllowOffshoreIslands ?
                    [DeliveryMethod.TCatDeliveryNormal] :
                    [DeliveryMethod.TCatDeliveryNormal, DeliveryMethod.TCatDeliverySevenElevenNormal];

            if (temperature is ItemStorageTemperature.Freeze && logistic is LogisticProviders.EcPayHomeDelivery)
                deliveryMethods = [DeliveryMethod.BlackCatFreeze];

            if (temperature is ItemStorageTemperature.Freeze && logistic is LogisticProviders.TCat)
                deliveryMethods = IsAllowOffshoreIslands ?
                    [DeliveryMethod.TCatDeliveryFreeze] :
                    [DeliveryMethod.TCatDeliveryFreeze, DeliveryMethod.TCatDeliverySevenElevenFreeze];

            if (temperature is ItemStorageTemperature.Frozen && logistic is LogisticProviders.EcPayHomeDelivery)
                deliveryMethods = [DeliveryMethod.BlackCatFrozen];

            if (temperature is ItemStorageTemperature.Frozen && logistic is LogisticProviders.GreenWorldLogistics)
                deliveryMethods = [DeliveryMethod.SevenToElevenFrozen];

            if (temperature is ItemStorageTemperature.Frozen && logistic is LogisticProviders.TCat)
                deliveryMethods = IsAllowOffshoreIslands ?
                    [DeliveryMethod.TCatDeliveryFrozen] :
                    [DeliveryMethod.TCatDeliveryFrozen, DeliveryMethod.TCatDeliverySevenElevenFrozen];
        }

        var map = new Dictionary<DeliveryMethod, LogisticProviders>
        {
            { DeliveryMethod.BlackCat1, LogisticProviders.BNormal },
            { DeliveryMethod.PostOffice, LogisticProviders.PostOffice },
            { DeliveryMethod.SevenToEleven1, LogisticProviders.SevenToEleven },
            { DeliveryMethod.FamilyMart1, LogisticProviders.FamilyMart },
            { DeliveryMethod.SevenToElevenC2C, LogisticProviders.SevenToElevenC2C },
            { DeliveryMethod.FamilyMartC2C, LogisticProviders.FamilyMartC2C },
            { DeliveryMethod.TCatDeliveryNormal, LogisticProviders.TCatNormal },
            { DeliveryMethod.TCatDeliverySevenElevenNormal, LogisticProviders.TCat711Normal },
            { DeliveryMethod.BlackCatFreeze, LogisticProviders.BFreeze },
            { DeliveryMethod.TCatDeliveryFreeze, LogisticProviders.TCatFreeze },
            { DeliveryMethod.TCatDeliverySevenElevenFreeze, LogisticProviders.TCat711Freeze },
            { DeliveryMethod.BlackCatFrozen, LogisticProviders.BFrozen },
            { DeliveryMethod.SevenToElevenFrozen, LogisticProviders.SevenToElevenFrozen },
            { DeliveryMethod.TCatDeliveryFrozen, LogisticProviders.TCatFrozen },
            { DeliveryMethod.TCatDeliverySevenElevenFrozen, LogisticProviders.TCat711Frozen },
        };

        return [.. deliveryMethods
            .Where(method => AllLogisticsProviderSetting.Any(a => map.TryGetValue(method, out var provider) && provider == a.LogisticProvider && a.IsEnabled))
            ];
    }
}