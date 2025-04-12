using Blazorise.LoadingIndicator;
using Kooco.Pikachu.Tenants;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantEmailing
{
    public partial class TenantEmailing
    {
        private CreateUpdateTenantEmailSettingsDto EmailSettings { get; set; }
        private LoadingIndicator Loading { get; set; }

        public TenantEmailing()
        {
            EmailSettings = new();
        }

        protected override async Task OnInitializedAsync()
        {
            await GetEmailSettingsAsync();
            await base.OnInitializedAsync();
        }

        private async Task UpdateEmailSettingsAsync()
        {
            try
            {
                if (EmailSettings.SenderName.IsNullOrEmpty()
                    || EmailSettings.Greetings.IsNullOrEmpty()
                    || EmailSettings.Subject.IsNullOrEmpty()
                    || EmailSettings.Footer.IsNullOrEmpty())
                {
                    return;
                }
                await Loading.Show();

                await _tenantEmailSettingsAppService.UpdateEmailSettingsAsync(EmailSettings);
                await GetEmailSettingsAsync();
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

        async Task GetEmailSettingsAsync()
        {
            try
            {
                var emailSettings = await _tenantEmailSettingsAppService.GetEmailSettingsAsync() ?? new();
                EmailSettings = ObjectMapper.Map<TenantEmailSettingsDto, CreateUpdateTenantEmailSettingsDto>(emailSettings);
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
        }
    }
}
