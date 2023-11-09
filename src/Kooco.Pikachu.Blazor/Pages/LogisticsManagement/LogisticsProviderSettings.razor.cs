using Blazorise.LoadingIndicator;
using Kooco.Pikachu.LogisticsProviders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.LogisticsManagement
{
    public partial class LogisticsProviderSettings
    {
        GreenWorldLogisticsCreateUpdateDto GreenWorld { get; set; }
        HomeDeliveryCreateUpdateDto HomeDelivery { get; set; }
        LoadingIndicator Loading { get; set; }

        public LogisticsProviderSettings()
        {
            GreenWorld = new();
            HomeDelivery = new();   
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
        }

        async Task UpdateGreenWorldLogisticsAsync()
        {
            try
            {
                var confirm = await _uiMessageService.Confirm(L["AreYouSureToUpdateGreenWorld?"]);
                if (!confirm)
                    return;
                await Loading.Show();
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

        void HandleTagDelete(string item)
        {
            GreenWorld.LogisticsSubTypesList.Remove(item);
            if (GreenWorld.LogisticsSubTypesList.Count > 0)
            {
                GreenWorld.LogisticsSubTypes = JsonConvert.SerializeObject(GreenWorld.LogisticsSubTypesList);
            }
            else
            {
                GreenWorld.LogisticsSubTypes = string.Empty;
            }
        }

        void OnSelectedValueChanged(string value)
        {
            GreenWorld.LogisticsType = value;
            GreenWorld.LogisticsSubTypesList = new();
            LogisticsConsts.LogisticsSubTypes.ForEach(item => GreenWorld.LogisticsSubTypesList.Add($"{item}({value})"));
            GreenWorld.LogisticsSubTypes = JsonConvert.SerializeObject(GreenWorld.LogisticsSubTypesList);
        }
    }
}
