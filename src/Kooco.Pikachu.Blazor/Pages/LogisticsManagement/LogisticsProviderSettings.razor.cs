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

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement
{
    public partial class LogisticsProviderSettings
    {
        GreenWorldLogisticsCreateUpdateDto GreenWorld { get; set; }
        HomeDeliveryCreateUpdateDto HomeDelivery { get; set; }
        PostOfficeCreateUpdateDto PostOffice { get; set; }
        SevenToElevenCreateUpdateDto SevenToEleven { get; set; }
        SevenToElevenCreateUpdateDto SevenToElevenFrozen { get; set; }
        SevenToElevenCreateUpdateDto FamilyMart { get; set; }
        BNormalCreateUpdateDto BNormal { get; set; }
        BNormalCreateUpdateDto BFreeze { get; set; }
        BNormalCreateUpdateDto BFrozen { get; set; }
        LoadingIndicator Loading { get; set; }
        
        public LogisticsProviderSettings()
        {
            GreenWorld = new();
            HomeDelivery = new(); 
            PostOffice = new();
            SevenToEleven = new();
            SevenToElevenFrozen = new();
            FamilyMart = new();
            BNormal = new();
            BFreeze = new();
            BFrozen = new();
        }

        protected override async Task OnInitializedAsync()
        {
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
            var providers = await _logisticProvidersAppService.GetAllAsync();
            
            var greenWorld = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.GreenWorldLogistics).FirstOrDefault();
            if(greenWorld != null)
            {
                GreenWorld = ObjectMapper.Map<LogisticsProviderSettingsDto, GreenWorldLogisticsCreateUpdateDto>(greenWorld);
            }

            var homeDelivery = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.HomeDelivery).FirstOrDefault();
            if (homeDelivery != null)
            {
                HomeDelivery = ObjectMapper.Map<LogisticsProviderSettingsDto, HomeDeliveryCreateUpdateDto>(homeDelivery);
            }
            var postOffice = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.PostOffice).FirstOrDefault();
            if (postOffice != null)
            {
                PostOffice = ObjectMapper.Map<LogisticsProviderSettingsDto, PostOfficeCreateUpdateDto>(postOffice);
            }
            var sevenToEleven = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.SevenToEleven).FirstOrDefault();
            if (sevenToEleven != null)
            {
                SevenToEleven = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToEleven);
            }
            var familyMart = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.FamilyMart).FirstOrDefault();
            if (familyMart != null)
            {
                FamilyMart = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(familyMart);
            }
            var sevenToElevenFrozen = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.SevenToElevenFrozen).FirstOrDefault();
            if (sevenToElevenFrozen != null)
            {
                SevenToElevenFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, SevenToElevenCreateUpdateDto>(sevenToElevenFrozen);
            }
            var bNormal = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BNormal).FirstOrDefault();
            if (bNormal != null)
            {
                BNormal = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bNormal);
            }
            var bFreeze = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BFreeze).FirstOrDefault();
            if (bFreeze != null)
            {
                BFreeze = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
            }
            var bFrozen = providers.Where(p => p.LogisticProvider == EnumValues.LogisticProviders.BFrozen).FirstOrDefault();
            if (bFrozen != null)
            {
                BFrozen = ObjectMapper.Map<LogisticsProviderSettingsDto, BNormalCreateUpdateDto>(bFreeze);
            }
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
        async Task UpdateBNormalAsync()
        {
            try
            {
                var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateBNormal?"]);
                if (!confirm)
                    return;
                await Loading.Show();
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
        async Task UpdateBFreezeAsync()
        {
            try
            {
                var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateBFreeze?"]);
                if (!confirm)
                    return;
                await Loading.Show();
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
}
