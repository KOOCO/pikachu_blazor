using Blazorise;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Kooco.Pikachu.Blazor.Pages.ShoppingCredits
{
    public partial class ShoppingCreditGetSetting

    {
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<GroupBuyDto> SelectedGroupBuy { get; set; }
        IEnumerable<KeyValueDto> SelectedProducts { get; set; }

        private Validations ValidationsRef;

        private readonly ItemAppService _itemAppService;
        public ShoppingCreditGetSetting(ItemAppService itemAppService)
        {

            Products = [];
            Groupbuys = [];
            SelectedGroupBuy = [];
            SelectedProducts = [];

            _itemAppService = itemAppService;
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await FetchProducts();
                await FetchGroupBuys();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
            await base.OnInitializedAsync();
        }
        async Task FetchGroupBuys()
        {
            try
            {
                var data = await GroupBuyAppService.GetListAsync(new GetGroupBuyInput
                {

                    MaxResultCount = 1000
                });
                Groupbuys = data.Items;


                return;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        async Task FetchProducts()
        {
            try
            {

                var data = await _itemAppService.GetAllItemsLookupAsync();
                Products = data;


                return;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        void NavigateToShoppingCredits()
        {
            NavigationManager.NavigateTo("/shopping-credit");

        }


    }
}