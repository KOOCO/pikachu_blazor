using Blazorise;

using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.EnumValues;

namespace Kooco.Pikachu.Blazor.Pages.DiscountCodes
{
    public partial class NewDiscountCode
    {
        
        private CreateDiscountCodeDto CreateDiscountCode { get; set; }
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<GroupBuyDto> SelectedGroupBuy { get; set; }
        IEnumerable<KeyValueDto> SelectedProducts { get; set; }
        IEnumerable<DeliveryMethod> SelectedShippings { get; set; }
        private Validations ValidationsRef;
       
        private readonly ItemAppService _itemAppService;
        public NewDiscountCode(ItemAppService itemAppService)
        {
            CreateDiscountCode = new();
            Products = [];
             Groupbuys = [];
            SelectedGroupBuy = [];
            SelectedProducts = [];
            SelectedShippings = [];
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
        void NavigateToDiscountCodes()
        {
            NavigationManager.NavigateTo("/discount-code");
        
        }


    }
}
