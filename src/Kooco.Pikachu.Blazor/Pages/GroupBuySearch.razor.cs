using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages
{
    public partial class GroupBuySearch
    {
        [Parameter]
        public string tenantName { get; set; }
        public string Url { get; set; }
        public string GroupBuy { get; set; }
       

        public GroupBuySearch() { 
        
       
        
        }
        protected override async Task OnInitializedAsync()
        {

            base.OnInitializedAsync();
            Url= configuration["EntryUrl"];
            if (tenantName == null || tenantName == "")
            {
                NavigationManager.NavigateTo("/");

            }

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
