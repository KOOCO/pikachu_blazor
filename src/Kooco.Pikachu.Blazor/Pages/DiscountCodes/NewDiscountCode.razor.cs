using Blazorise;

using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.EnumValues;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Kooco.Pikachu.AddOnProducts;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Kooco.Pikachu.Blazor.Pages.DiscountCodes
{
    public partial class NewDiscountCode
    {
        [Parameter]
        public Guid Id { get; set; }
        private bool IsUpdating { get; set; }
        private ValidationMessageStore? messageStore;
        private EditContext? editContext;
        private CreateUpdateDiscountCodeDto CreateDiscountCode { get; set; }
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<Guid> SelectedGroupBuy { get; set; }
        IEnumerable<Guid> SelectedProducts { get; set; }
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
            editContext = new(CreateDiscountCode);
            messageStore = new(editContext);
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await FetchProducts();
                await FetchGroupBuys();
                if (Id != Guid.Empty)
                {
                    var discountCode = await DiscountCodeAppService.GetAsync(Id);
                    if (discountCode != null)
                    {

                        CreateDiscountCode = ObjectMapper.Map<DiscountCodeDto, CreateUpdateDiscountCodeDto>(discountCode);
                        CreateDiscountCode.GroupbuyIds = discountCode.DiscountSpecificGroupbuys.Select(x => x.GroupbuyId).ToList();
                        CreateDiscountCode.ProductIds = discountCode.DiscountSpecificProducts.Select(x => x.DiscountCodeId).ToList();
                        SelectedGroupBuy = discountCode.DiscountSpecificGroupbuys.Select(x => x.GroupbuyId);
                        SelectedProducts = discountCode.DiscountSpecificProducts.Select(x => x.ProductId);
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
        private async Task HandleValidSubmit()
        {
            messageStore?.Clear();
            IsUpdating = true;
            if (CreateDiscountCode.GroupbuysScope == "AllGroupbuys")
            {
                CreateDiscountCode.GroupbuyIds = Groupbuys.Select(x => x.Id).ToList();
            }
            else
            {
                CreateDiscountCode.GroupbuyIds = SelectedGroupBuy.ToList();
            }
            if (CreateDiscountCode.ShippingDiscountScope == "AllMethods")
            {
                CreateDiscountCode.SpecificShippingMethods = Enum.GetValues(typeof(DeliveryMethod)).Cast<int>().ToList();
            }
            else
            {
                CreateDiscountCode.SpecificShippingMethods = SelectedShippings.Cast<int>().ToList();
            }
            // Custom validation logic
            if (CreateDiscountCode.ProductsScope == "AllProducts")
            {
                CreateDiscountCode.ProductIds = Products.Select(x => x.Id).ToList();
            }
            else
            {
                CreateDiscountCode.ProductIds = SelectedProducts.ToList();
            }
            if (CreateDiscountCode.ProductIds.Count == 0)
            {
                messageStore?.Add(() => CreateDiscountCode.ProductIds, "Select at least one.");
                IsUpdating = false;
                return;
            }
            if (CreateDiscountCode.ProductIds.Count == 0)
            {
                messageStore?.Add(() => CreateDiscountCode.ProductIds, "Select at least one.");
                IsUpdating = false;
                return;
            }
            if (CreateDiscountCode.GroupbuyIds.Count == 0)
            {
                messageStore?.Add(() => CreateDiscountCode.GroupbuyIds, "Select at least one.");
                IsUpdating = false;
                return;
            }

            if (await ValidationsRef.ValidateAll())
            {
                if (Id == Guid.Empty)
                {
                    await DiscountCodeAppService.CreateAsync(CreateDiscountCode);
                }
                else
                {

                    await DiscountCodeAppService.UpdateAsync(Id, CreateDiscountCode);
                }
                IsUpdating = false;
                NavigateToDiscountCodes();
        }
        IsUpdating = false;
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
