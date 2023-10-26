using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Data;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages
{
    public partial class GroupBuySearch
    {
        [Parameter]
        public string tenantName { get; set; }
        public string Url { get; set; }
        public string GroupBuy { get; set; }
        private readonly ITenantRepository _tenantAppService;
       public string LogoUrl { get; set; }
        public string BannerUrl { get; set; }

        public GroupBuySearch(ITenantRepository tenantAppService) {

            _tenantAppService = tenantAppService;
        
        }
        protected override async Task OnInitializedAsync()
        {

            base.OnInitializedAsync();
            Url= configuration["EntryUrl"];
            if (tenantName == null || tenantName == "")
            {
                NavigationManager.NavigateTo("/");

            }
          var tenant=  await _tenantAppService.FindByNameAsync(tenantName);
          LogoUrl=  tenant.GetProperty<string>("LogoUrl");
            BannerUrl = tenant.GetProperty<string>("BannerUrl");

        }
        public async Task RedirectToGroupBuy() {
            if (GroupBuy == null || GroupBuy == "")
            {
                await _message.Error(L["Please Enter GroupBuy Code"]);

            }
            else
            {
                Url = Url + GroupBuy;
                NavigationManager.NavigateTo(Url);
            }
        
        }

    }
}
