using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees
{
    public partial class TenantFeeSetting
    {
        private string TenantName = "";
        [Parameter] public Guid TenantId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await SetTenantName();
            await base.OnInitializedAsync();
        }

        async Task SetTenantName()
        {

            var tenant = await TenantAppService.GetAsync(TenantId);
            if (tenant != null)
            {
                TenantName = tenant.Name;
            }
            else
            {
                TenantName = "Unknown Tenant";
            }



        }

        private void NavigateToPlatformManagement()
        {
            NavigationManager.NavigateTo($"/platform-fee-settings");


        }

    }
}
