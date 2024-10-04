using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.ShoppingCredits;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ShoppingCredits
{
    public partial class ShoppingCredit
    {
        private ShoppingCreditUsageSettingDto Usage { get; set; }
        private ShoppingCreditEarnSettingDto Earn { get; set; }

        public ShoppingCredit() {
            Usage = new();
            Earn = new();
        }
        protected override async Task OnInitializedAsync()
        {
            Usage = await ShoppingCreditUsageSettingAppService.GetFirstAsync()??new();
            Earn = await ShoppingCreditEarnSettingAppService.GetFirstAsync()??new();
            await InvokeAsync(StateHasChanged);

            await base.OnInitializedAsync();
        }
            void NavigateToUseSetting() {

            NavigationManager.NavigateTo("/use-setting");
        
        }
        void NavigateToGetSetting()
        {

            NavigationManager.NavigateTo("/get-setting");

        }
    }
}
