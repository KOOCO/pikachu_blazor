using Blazorise;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.AddOnProducts
{
    public partial class NewAddOnProduct
    {
        [Parameter]
        public Guid Id { get; set; }
        private bool IsUpdating { get; set; }
        private ValidationMessageStore? messageStore;
        private EditContext? editContext;
        string itemimageUrl = "";
        private AddOnProductDto AddOnProduct { get; set; }
        public string ButtonName { get; set; }
        private CreateUpdateAddOnProductDto CreateAddOnProduct { get; set; }
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<Guid> SelectedGroupBuy { get; set; }
        private Validations ValidationsRef;
        private ItemDto Item { get; set; }
        private List<ItemDetailsDto> ItemDetails { get; set; }
        private ItemDetailsDto ItemDetail { get; set; }
       
        private readonly ItemAppService _itemAppService;
        private readonly ItemDetailsAppService _itemDetailService;
        public NewAddOnProduct(ItemAppService itemAppService, ItemDetailsAppService itemDetailService)
        {
            CreateAddOnProduct = new();
            Item= new ItemDto();
            ItemDetail=new ItemDetailsDto();
            ItemDetails = [];
            Groupbuys = [];
            SelectedGroupBuy = [];
            _itemAppService = itemAppService;
            _itemDetailService = itemDetailService;
            editContext = new(CreateAddOnProduct);
            messageStore = new(editContext);
        }
        private void NavigateToList()
        {
            NavigationManager.NavigateTo("/add-on-products");
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
             
                await FetchProducts();
                await FetchGroupBuys();
                await InvokeAsync(StateHasChanged);
                if (Id != Guid.Empty)
                {
                    var addon = await AddOnProductAppService.GetAsync(Id);
                    if (addon != null)
                    {

                        CreateAddOnProduct = ObjectMapper.Map<AddOnProductDto, CreateUpdateAddOnProductDto>(addon);
                        CreateAddOnProduct.GroupBuyIds = addon.AddOnProductSpecificGroupbuys.Select(x => x.GroupbuyId).ToList();
                    SelectedGroupBuy= addon.AddOnProductSpecificGroupbuys.Select(x => x.GroupbuyId);
                        await JSRuntime.InvokeVoidAsync("console.log", CreateAddOnProduct.ProductId);
                        ItemDetail = await _itemDetailService.GetAsync(CreateAddOnProduct.ProductId);
                        await JSRuntime.InvokeVoidAsync("console.log", ItemDetail.ItemId);
                        CreateAddOnProduct.ItemId = ItemDetail.ItemId;
                        ItemDetails = await _itemDetailService.GetItemDetailByItemId(CreateAddOnProduct.ItemId);
                    }

                }
                
                await InvokeAsync(StateHasChanged);
                await ValidationsRef.ClearAll();
                await InvokeAsync(StateHasChanged);
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
        private async Task HandleValidSubmit()
        {
          
            messageStore?.Clear();
            IsUpdating = true;
            if (CreateAddOnProduct.GroupbuysScope == "AllGroupbuys")
            {
                CreateAddOnProduct.GroupBuyIds = Groupbuys.Select(x => x.Id).ToList();
            }
            else
            {
                CreateAddOnProduct.GroupBuyIds = SelectedGroupBuy.ToList();
            }
            await JSRuntime.InvokeVoidAsync("console.log", "ClickedHandler called!");
            // Custom validation logic
            if (CreateAddOnProduct.ItemId == Guid.Empty)
            {
                await JSRuntime.InvokeVoidAsync("console.log", "productId empty");
                messageStore?.Add(() => CreateAddOnProduct.ItemId, "Select at least one.");
                IsUpdating = false;
                return;
            }
            if (CreateAddOnProduct.ProductId == Guid.Empty)
            {
                messageStore?.Add(() => CreateAddOnProduct.ProductId, "Select at least one.");
                IsUpdating = false;
                return;
            }
            if (CreateAddOnProduct.GroupBuyIds.Count == 0)
            {
                await JSRuntime.InvokeVoidAsync("console.log", "groupbuy empty");
                messageStore?.Add(() => CreateAddOnProduct.GroupBuyIds, "Select at least one.");
                IsUpdating = false;
                return;
            }
            
            if (await ValidationsRef.ValidateAll())
            {
                if (Id == Guid.Empty)
                {
                    await JSRuntime.InvokeVoidAsync("console.log", "ClickedHandler called!");
                    await AddOnProductAppService.CreateAsync(CreateAddOnProduct);
                }
                else {
                    await JSRuntime.InvokeVoidAsync("console.log", "updating");
                    await AddOnProductAppService.UpdateAsync(Id, CreateAddOnProduct);
                }
                IsUpdating = false;
                NavigateToAddOnProducts();
            }
            IsUpdating = false;
        }
        private async Task OnSelectedProductChangedHandler()
        {
            CreateAddOnProduct.ProductId = Guid.Empty;
          ItemDetails = await _itemDetailService.GetItemDetailByItemId(CreateAddOnProduct.ItemId);
          
          await InvokeAsync(StateHasChanged);
        }
      async  void NavigateToAddOnProducts() {
            await JSRuntime.InvokeVoidAsync("console.log", "navigating");
            NavigationManager.NavigateTo("/add-on-products");
        }
    }
}
