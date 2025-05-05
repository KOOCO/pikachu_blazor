using Blazorise;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Components.Forms;
using Kooco.Pikachu.ShoppingCredits;
using System.Text.Json;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.ShoppingCredits
{
    public partial class ShoppingCreditGetSetting

    {
        [Parameter]
        public Guid Id { get; set; }
        private CreateUpdateShoppingCreditEarnSettingDto CreateUpdateEarn { get; set; }
        private IReadOnlyList<GroupBuyDto> Groupbuys { get; set; }
        private IReadOnlyList<KeyValueDto> Products { get; set; }
        IEnumerable<Guid> SelectedGroupBuy { get; set; }
        IEnumerable<Guid> SelectedProducts { get; set; }
        private List<StagedSetting> stagedSettings = new List<StagedSetting>();
        private Validations ValidationsRef;
        private bool IsUpdating { get; set; }
        private ValidationMessageStore? messageStore;
        private EditContext? editContext;
        private readonly ItemAppService _itemAppService;
        private List<string> selectedApplicableItems = new List<string>();
        private bool IsAddOnProductsSelected { get; set; }
        private bool IsShippingFeesSelected { get; set; }
        public ShoppingCreditGetSetting(ItemAppService itemAppService)
        {

            Products = [];
            Groupbuys = [];
            SelectedGroupBuy = [];
            SelectedProducts = [];

            _itemAppService = itemAppService;
            CreateUpdateEarn = new();
            editContext = new(CreateUpdateEarn);
            messageStore = new(editContext);
            stagedSettings.Add(new StagedSetting());
        }
        private void NavigateToList()
        {
            NavigationManager.NavigateTo("/shopping-credit");
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var shoppingCreditEarn = await ShoppingCreditEarnSettingAppService.GetFirstAsync();
                if (shoppingCreditEarn != null)
                {
                    Id = shoppingCreditEarn.Id;
                    CreateUpdateEarn = ObjectMapper.Map<ShoppingCreditEarnSettingDto, CreateUpdateShoppingCreditEarnSettingDto>(shoppingCreditEarn);
                    stagedSettings = JsonSerializer.Deserialize<List<StagedSetting>>(shoppingCreditEarn.CashbackStagedSettings);
                    CreateUpdateEarn.SpecificGroupbuyIds = shoppingCreditEarn.SpecificGroupBuys.Select(x => x.GroupbuyId).ToList();
                    CreateUpdateEarn.SpecificGroupbuyIds = shoppingCreditEarn.SpecificProducts.Select(x => x.ProductId).ToList();
                    SelectedGroupBuy = shoppingCreditEarn.SpecificGroupBuys.Select(x => x.GroupbuyId);
                    SelectedProducts = shoppingCreditEarn.SpecificProducts.Select(x => x.ProductId);
                    if (!string.IsNullOrWhiteSpace(CreateUpdateEarn.CashbackApplicableItems))
                    {
                        var selected = System.Text.Json.JsonSerializer.Deserialize<List<string>>(CreateUpdateEarn.CashbackApplicableItems) ?? new List<string>();
                        IsAddOnProductsSelected = selected.Contains("AddOnProducts");
                        IsShippingFeesSelected = selected.Contains("ShippingFees");
                    }
                    await InvokeAsync(StateHasChanged);
                    await ValidationsRef.ClearAll();
                    await InvokeAsync(StateHasChanged);
                }
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
        private void OnCheckChanged(bool value, string itemName)
        {
            var selected = new List<string>();

            if (itemName == "AddOnProducts")
                IsAddOnProductsSelected = value;
            else if (itemName == "ShippingFees")
                IsShippingFeesSelected = value;

            if (IsAddOnProductsSelected)
                selected.Add("AddOnProducts");

            if (IsShippingFeesSelected)
                selected.Add("ShippingFees");

            CreateUpdateEarn.CashbackApplicableItems = System.Text.Json.JsonSerializer.Serialize(selected);
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
        private async Task HandleValidSubmit()
        {
            messageStore?.Clear();
            IsUpdating = true;

            if (CreateUpdateEarn.CashbackApplicableGroupbuys == "AllGroupbuys")
            {
                CreateUpdateEarn.SpecificGroupbuyIds = Groupbuys.Select(x => x.Id).ToList();
            }
            else
            {
                CreateUpdateEarn.SpecificGroupbuyIds = SelectedGroupBuy.ToList();
            }

            // Custom validation logic
            if (CreateUpdateEarn.CashbackApplicableProducts == "AllProducts")
            {
                CreateUpdateEarn.SpecificProductIds = Products.Select(x => x.Id).ToList();
            }
            else
            {
                CreateUpdateEarn.SpecificProductIds = SelectedProducts.ToList();
            }
            if (CreateUpdateEarn.SpecificGroupbuyIds.Count == 0)
            {
                messageStore?.Add(() => CreateUpdateEarn.SpecificGroupbuyIds, "Select at least one.");
                IsUpdating = false;
                return;
            }

            if (CreateUpdateEarn.SpecificProductIds.Count == 0)
            {
                messageStore?.Add(() => CreateUpdateEarn.SpecificProductIds, "Select at least one.");
                IsUpdating = false;
                return;
            }

            if (await ValidationsRef.ValidateAll())
            {
                CreateUpdateEarn.CashbackStagedSettings = GetJson();
                if (Id == Guid.Empty)
                {
                    await ShoppingCreditEarnSettingAppService.CreateAsync(CreateUpdateEarn);
                }
                else
                {

                    await ShoppingCreditEarnSettingAppService.UpdateAsync(Id, CreateUpdateEarn);
                }
                IsUpdating = false;
                NavigateToShoppingCredits();
            }
            IsUpdating = false;
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
        private string GetJson()
        {
            return JsonSerializer.Serialize(stagedSettings);
        }
        private void AddNewRow()
        {
            stagedSettings.Add(new StagedSetting());
        }

    }
}