using Blazorise;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.AddOnProducts
{
    public partial class NewAddOnProduct
    {
        [Parameter]
        public Guid Id { get; set; }
        string itemimageUrl = "";
        private AddOnProductDto AddOnProduct { get; set; }
        private CreateAddOnProductDto CreateAddOnProduct { get; set; }
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<GroupBuyDto> SelectedGroupBuy { get; set; }
        private Validations ValidationsRef;
        private ItemDto Item { get; set; }
        private readonly ItemAppService _itemAppService;
        public NewAddOnProduct(ItemAppService itemAppService)
        {
            CreateAddOnProduct = new();
            Item= new ItemDto();
            Groupbuys = [];
            SelectedGroupBuy = [];
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

        private async void OnSelectedProductChangedHandler(KeyValueDto value)
        {
            Item = await _itemAppService.GetAsync(value.Id);
            itemimageUrl = await _itemAppService.GetFirstImageUrlAsync(value.Id);
            StateHasChanged();
        }
       
    }
}
