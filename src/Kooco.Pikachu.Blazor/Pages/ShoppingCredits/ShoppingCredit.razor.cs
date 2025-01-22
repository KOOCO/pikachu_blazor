using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.ShoppingCredits;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.ShoppingCredits
{
    public partial class ShoppingCredit
    {
        private ShoppingCreditUsageSettingDto Usage { get; set; }
        private ShoppingCreditEarnSettingDto Earn { get; set; }
        private ShoppingCreditStatDto Stats { get; set; }

        public ShoppingCredit() {
            Usage = new();
            Earn = new();
            Stats = new();
        }
        protected override async Task OnInitializedAsync()
        {
            Usage = await ShoppingCreditUsageSettingAppService.GetFirstAsync()??new();
            Earn = await ShoppingCreditEarnSettingAppService.GetFirstAsync()??new();
            Stats=await UserShoppingCreditAppService.GetShoppingCreditStatsAsync()??new();
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
