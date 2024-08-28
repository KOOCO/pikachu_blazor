using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsProviders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement;

public partial class LogisticsProviderSettings
{
    #region Inject
    GreenWorldLogisticsCreateUpdateDto GreenWorld = new();
    GreenWorldLogisticsCreateUpdateDto GreenWorldC2C = new();
    HomeDeliveryCreateUpdateDto HomeDelivery = new();
    PostOfficeCreateUpdateDto PostOffice = new();
    SevenToElevenCreateUpdateDto SevenToEleven = new();
    SevenToElevenCreateUpdateDto SevenToElevenC2C = new();
    SevenToElevenCreateUpdateDto SevenToElevenFrozen = new();
    SevenToElevenCreateUpdateDto FamilyMart = new();
    SevenToElevenCreateUpdateDto FamilyMartC2C = new();
    BNormalCreateUpdateDto BNormal = new();
    BNormalCreateUpdateDto BFreeze = new();
    BNormalCreateUpdateDto BFrozen = new();
    TCatLogisticsCreateUpdateDto TCatLogistics = new();
    TCatNormalCreateUpdateDto TCatNormal = new();
    TCatFreezeCreateUpdateDto TCatFreeze = new();
    TCatFrozenCreateUpdateDto TCatFrozen = new();
    TCat711NormalCreateUpdate TCat711Normal = new();
    TCat711FreezeCreateUpdateDto TCat711Freeze = new();
    TCat711FrozenCreateUpdateDto TCat711Frozen = new();
    LoadingIndicator Loading { get; set; }
    #endregion

    #region Constructor
    public LogisticsProviderSettings()
    {
        GreenWorld = new();
        GreenWorldC2C = new();
        HomeDelivery = new(); 
        PostOffice = new();
        SevenToEleven = new();
        SevenToElevenC2C = new();
        SevenToElevenFrozen = new();
        FamilyMart = new();
        FamilyMartC2C = new();
        BNormal = new();
        BFreeze = new();
        BFrozen = new();
    }
    #endregion

    protected override async Task OnInitializedAsync()
    {
        Enum.GetValues(typeof(SizeEnum));

        try
        {
            await GetAllAsync();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
    }

    async Task GetAllAsync()
    {
        List<LogisticsProviderSettingsDto> providers = await _logisticProvidersAppService.GetAllAsync();

        LogisticsProviderSettingsDto? greenWorld = providers.Where(p => p.LogisticProvider is LogisticProviders.GreenWorldLogistics).FirstOrDefault();
        
        if (greenWorld is not null) GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);

        LogisticsProviderSettingsDto? greenWorldC2C = providers.Where(p => p.LogisticProvider is LogisticProviders.GreenWorldLogisticsC2C).FirstOrDefault();
        
        if (greenWorldC2C is not null) GreenWorldC2C = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorldC2C);

        LogisticsProviderSettingsDto? homeDelivery = providers.Where(p => p.LogisticProvider is LogisticProviders.HomeDelivery).FirstOrDefault();

        if (homeDelivery is not null) HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);
        
        LogisticsProviderSettingsDto? postOffice = providers.Where(p => p.LogisticProvider is LogisticProviders.PostOffice).FirstOrDefault();

        if (postOffice is not null) PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);
        
        LogisticsProviderSettingsDto? sevenToEleven = providers.Where(p => p.LogisticProvider is LogisticProviders.SevenToEleven).FirstOrDefault();

        if (sevenToEleven is not null) SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);

        LogisticsProviderSettingsDto? sevenToElevenC2C = providers.Where(p => p.LogisticProvider is LogisticProviders.SevenToElevenC2C).FirstOrDefault();

        if (sevenToElevenC2C is not null) SevenToElevenC2C = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenC2C);

        LogisticsProviderSettingsDto? familyMart = providers.Where(p => p.LogisticProvider is LogisticProviders.FamilyMart).FirstOrDefault();
        
        if (familyMart is not null) FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);
        
        LogisticsProviderSettingsDto? familyMartC2C = providers.Where(p => p.LogisticProvider is LogisticProviders.FamilyMartC2C).FirstOrDefault();
        
        if (familyMartC2C is not null) FamilyMartC2C = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMartC2C);

        LogisticsProviderSettingsDto? sevenToElevenFrozen = providers.Where(p => p.LogisticProvider is LogisticProviders.SevenToElevenFrozen).FirstOrDefault();
        
        if (sevenToElevenFrozen is not null) SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);

        LogisticsProviderSettingsDto? bNormal = providers.Where(p => p.LogisticProvider is LogisticProviders.BNormal).FirstOrDefault();
        
        if (bNormal is not null) BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);

        LogisticsProviderSettingsDto? bFreeze = providers.Where(p => p.LogisticProvider is LogisticProviders.BFreeze).FirstOrDefault();
        
        if (bFreeze is not null) BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);

        LogisticsProviderSettingsDto? bFrozen = providers.Where(p => p.LogisticProvider is LogisticProviders.BFrozen).FirstOrDefault();

        if (bFrozen is not null) BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFrozen);

        LogisticsProviderSettingsDto? tCat = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat);

        if (tCat is not null) TCatLogistics = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatLogisticsCreateUpdateDto>(tCat);

        LogisticsProviderSettingsDto? tCatNormal = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCatNormal);

        if (tCatNormal is not null) TCatNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatNormalCreateUpdateDto>(tCatNormal);

        LogisticsProviderSettingsDto? tCatFreeze = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCatFreeze);

        if (tCatFreeze is not null) TCatFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatFreezeCreateUpdateDto>(tCatFreeze);

        LogisticsProviderSettingsDto? tCatFrozen = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCatFrozen);

        if (tCatFrozen is not null) TCatFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, TCatFrozenCreateUpdateDto>(tCatFrozen);

        LogisticsProviderSettingsDto? tCat711Normal = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat711Normal);

        if (tCat711Normal is not null) TCat711Normal = ObjectMapper.Map<LogisticsProviderSettingsDto, TCat711NormalCreateUpdate>(tCat711Normal);

        LogisticsProviderSettingsDto? tCat711Freeze = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat711Freeze);

        if (tCat711Freeze is not null) TCat711Freeze = ObjectMapper.Map<LogisticsProviderSettingsDto, TCat711FreezeCreateUpdateDto>(tCat711Freeze);

        LogisticsProviderSettingsDto? tCat711Frozen = providers.FirstOrDefault(f => f.LogisticProvider is LogisticProviders.TCat711Frozen);

        if (tCat711Frozen is not null) TCat711Frozen = ObjectMapper.Map<LogisticsProviderSettingsDto, TCat711FrozenCreateUpdateDto>(tCat711Frozen);
    }

    async Task UpdateGreenWorldLogisticsAsync()
    {
        try
        {
            var city = GreenWorld.City;
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateGreenWorld?"]);
            if (!confirm)
                return;
            await Loading.Show();
            GreenWorld.City = city;
            await _logisticProvidersAppService.UpdateGreenWorldAsync(GreenWorld);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateGreenWorldLogisticsC2CAsync()
    {
        try
        {
            var city = GreenWorldC2C.City;
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateGreenWorldC2C?"]);
            if (!confirm)
                return;
            await Loading.Show();
            GreenWorldC2C.City = city;
            await _logisticProvidersAppService.UpdateGreenWorldC2CAsync(GreenWorldC2C);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateHomeDeliveryAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateHomeDelivery?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdateHomeDeliveryAsync(HomeDelivery);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdatePostOfficeAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdatePostOffice?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdatePostOfficeAsync(PostOffice);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateSevenToElevenAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdate7-11?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdateSevenToElevenAsync(SevenToEleven);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateSevenToElevenC2CAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdate7-11C2C?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdateSevenToElevenC2CAsync(SevenToElevenC2C);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateSevenToElevenFrozenAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdate7-11Frozen?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdateSevenToElevenFrozenAsync(SevenToElevenFrozen);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateTCat711NormalAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdate7-11Normal?"]);

            if (!confirm) return;
            
            await Loading.Show();
            
            await _logisticProvidersAppService.UpdateTCat711NormalAsync(TCat711Normal);
            
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateTCat711FreezeAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdate7-11Freeze?"]);

            if (!confirm) return;
            
            await Loading.Show();
            
            await _logisticProvidersAppService.UpdateTCat711FreezeAsync(TCat711Freeze);
            
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateTCat711FrozenAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdate7-11Frozen?"]);

            if (!confirm) return;
            
            await Loading.Show();
            
            await _logisticProvidersAppService.UpdateTCat711FrozenAsync(TCat711Frozen);
            
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateFamilyMartAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateFamilyMart?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdateFamilyMartAsync(FamilyMart);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateFamilyMartC2CAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateFamilyMartC2C?"]);
            if (!confirm)
                return;
            await Loading.Show();
            await _logisticProvidersAppService.UpdateFamilyMartC2CAsync(FamilyMartC2C);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateBNormalAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateBNormal?"]);
            if (!confirm)
                return;
            await Loading.Show();

            if (BNormal.Size is 0)
            {
                await _uiMessageService.Error("Size Cannot be empty.");

                await Loading.Hide();

                return;
            }

            await _logisticProvidersAppService.UpdateBNormalAsync(BNormal);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateTCatNormalAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateTCatNormal?"]);
            
            if (!confirm) return;
            
            await Loading.Show();

            if (TCatNormal.Size is 0)
            {
                await _uiMessageService.Error("Size Cannot be empty.");

                await Loading.Hide();

                return;
            }

            if (TCatNormal.Payment && TCatNormal.TCatPaymentMethod is 0)
            {
                await _uiMessageService.Error("Payment Method Cannot be empty.");

                await Loading.Hide();

                return;
            }

            await _logisticProvidersAppService.UpdateTCatNormalAsync(TCatNormal);
            
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());

            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    public async Task UpdateTCatFreezeAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateTCatFreeze?"]);
            
            if (!confirm) return;
            
            await Loading.Show();

            if (TCatFreeze.Size is 0)
            {
                await _uiMessageService.Error("Size Cannot be empty.");

                await Loading.Hide();

                return;
            }

            if (TCatFreeze.Payment && TCatFreeze.TCatPaymentMethod is 0)
            {
                await _uiMessageService.Error("Payment Method Cannot be empty.");

                await Loading.Hide();

                return;
            }

            await _logisticProvidersAppService.UpdateTCatFreezeAsync(TCatFreeze);
            
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());

            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    public async Task UpdateTCatFrozenAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateTCatFrozen?"]);
            
            if (!confirm) return;
            
            await Loading.Show();

            if (TCatFrozen.Size is 0)
            {
                await _uiMessageService.Error("Size Cannot be empty.");

                await Loading.Hide();

                return;
            }

            if (TCatFrozen.Payment && TCatFrozen.TCatPaymentMethod is 0)
            {
                await _uiMessageService.Error("Payment Method Cannot be empty.");

                await Loading.Hide();

                return;
            }

            await _logisticProvidersAppService.UpdateTCatFrozenAsync(TCatFrozen);
            
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());

            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateBFreezeAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateBFreeze?"]);
            if (!confirm)
                return;
            await Loading.Show();

            if (BFreeze.Size is 0)
            {
                await _uiMessageService.Error("Size Cannot be empty.");

                await Loading.Hide();

                return;
            }

            await _logisticProvidersAppService.UpdateBFreezeAsync(BFreeze);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    async Task UpdateBFrozenAsync()
    {
        try
        {
            var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateBNFrozen?"]);
            if (!confirm)
                return;
            await Loading.Show();

            if (BFrozen.Size is 0)
            {
                await _uiMessageService.Error("Size Cannot be empty.");

                await Loading.Hide();

                return;
            }

            await _logisticProvidersAppService.UpdateBFrozenAsync(BFrozen);
            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }

    public async Task UpdateTCatAsync()
    {
        try
        {
            bool confirm = await _uiMessageService.Confirm(L["Are you sure you want to update TCat?"]);

            if (!confirm) return;

            await Loading.Show();

            await _logisticProvidersAppService.UpdateTCatAsync(TCatLogistics);

            await GetAllAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());

            await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await Loading.Hide();
        }
    }
    
    void OnMainIslandCheckedChange(string island, ChangeEventArgs e)
    {
        var value = (bool)(e?.Value ?? false);
        if (value)
        {
            HomeDelivery.MainIslandsList.Add(island);
        }
        else
        {
            HomeDelivery.MainIslandsList.Remove(island);
        }

        HomeDelivery.MainIslands = JsonConvert.SerializeObject(HomeDelivery.MainIslandsList);
    }

    void OnOuterIslandCheckedChange(string island, ChangeEventArgs e)
    {
        var value = (bool)(e?.Value ?? false);
        if (value)
        {
            HomeDelivery.OuterIslandsList.Add(island);
        }
        else
        {
            HomeDelivery.OuterIslandsList.Remove(island);
        }

        HomeDelivery.OuterIslands = JsonConvert.SerializeObject(HomeDelivery.OuterIslandsList);
    }

    //void HandleTagDelete(string item)
    //{
    //    GreenWorld.LogisticsSubTypesList.Remove(item);
    //    if (GreenWorld.LogisticsSubTypesList.Count > 0)
    //    {
    //        GreenWorld.LogisticsSubTypes = JsonConvert.SerializeObject(GreenWorld.LogisticsSubTypesList);
    //    }
    //    else
    //    {
    //        GreenWorld.LogisticsSubTypes = string.Empty;
    //    }
    //}

    //void OnSelectedValueChanged(string value)
    //{
    //    GreenWorld.LogisticsType = value;
    //    GreenWorld.LogisticsSubTypesList = new();
    //    LogisticsConsts.LogisticsSubTypes.ForEach(item => GreenWorld.LogisticsSubTypesList.Add($"{item}({value})"));
    //    GreenWorld.LogisticsSubTypes = JsonConvert.SerializeObject(GreenWorld.LogisticsSubTypesList);
    //}
}
