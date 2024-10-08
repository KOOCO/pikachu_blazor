using Blazorise;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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
       
        private CreateUpdateAddOnProductDto CreateAddOnProduct { get; set; }
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<Guid> SelectedGroupBuy { get; set; }
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
            editContext = new(CreateAddOnProduct);
            messageStore = new(editContext);
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
            // Custom validation logic
            if (CreateAddOnProduct.ProductId==Guid.Empty)
            {
                messageStore?.Add(() => CreateAddOnProduct.ProductId, "Select at least one.");
                IsUpdating = false;
                return;
            }
            if (CreateAddOnProduct.GroupBuyIds.Count == 0)
            {
                messageStore?.Add(() => CreateAddOnProduct.GroupBuyIds, "Select at least one.");
                IsUpdating = false;
                return;
            }
            
            if (await ValidationsRef.ValidateAll())
            {
                if (Id == Guid.Empty)
                {
                    await AddOnProductAppService.CreateAsync(CreateAddOnProduct);
                }
                else {

                    await AddOnProductAppService.UpdateAsync(Id, CreateAddOnProduct);
                }
                IsUpdating = false;
                NavigateToAddOnProducts();
            }
            IsUpdating = false;
        }
        private async Task OnSelectedProductChangedHandler()
        {
            Item = await _itemAppService.GetAsync(CreateAddOnProduct.ProductId);
            itemimageUrl = await _itemAppService.GetFirstImageUrlAsync(CreateAddOnProduct.ProductId);
            StateHasChanged();
        }
        void NavigateToAddOnProducts() {

            NavigationManager.NavigateTo("/add-on-products");
        }
    }
}
