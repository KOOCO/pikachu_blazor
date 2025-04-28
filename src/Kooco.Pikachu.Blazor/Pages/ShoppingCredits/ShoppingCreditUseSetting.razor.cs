using Blazorise;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Items;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Kooco.Pikachu.ShoppingCredits;
using Microsoft.AspNetCore.Components.Forms;
using Kooco.Pikachu.AddOnProducts;
using System.Linq;
using System.Text.Json;
using Kooco.Pikachu.Blazor.Pages.DiscountCodes;

namespace Kooco.Pikachu.Blazor.Pages.ShoppingCredits
{
    public partial class ShoppingCreditUseSetting
    {
        [Parameter]
        public Guid Id { get; set; }
        private CreateUpdateShoppingCreditUsageSettingDto CreateUpdateUsage { get; set; }
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
        public ShoppingCreditUseSetting(ItemAppService itemAppService)
        {
           
            Products = [];
            Groupbuys = [];
            SelectedGroupBuy = [];
            SelectedProducts = [];
           
            _itemAppService = itemAppService;
            CreateUpdateUsage = new();
            editContext = new(CreateUpdateUsage);
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
                var shoppingCreditUsage = await ShoppingCreditUsageSettingAppService.GetFirstAsync();
                if (shoppingCreditUsage != null)
                {
                    Id = shoppingCreditUsage.Id;
                    CreateUpdateUsage=ObjectMapper.Map<ShoppingCreditUsageSettingDto,CreateUpdateShoppingCreditUsageSettingDto>(shoppingCreditUsage);
                    stagedSettings = JsonSerializer.Deserialize<List<StagedSetting>>(shoppingCreditUsage.StagedSettings);
                    CreateUpdateUsage.GroupbuyIds = shoppingCreditUsage.SpecificGroupbuys.Select(x => x.GroupbuyId).ToList();
                    CreateUpdateUsage.ProductIds = shoppingCreditUsage.SpecificProducts.Select(x => x.ProductId).ToList();
                    SelectedGroupBuy = shoppingCreditUsage.SpecificGroupbuys.Select(x => x.GroupbuyId);
                    SelectedProducts = shoppingCreditUsage.SpecificProducts.Select(x => x.ProductId);
                    await InvokeAsync(StateHasChanged);
                    await ValidationsRef.ClearAll();
                    await InvokeAsync(StateHasChanged);
                }
                if (!string.IsNullOrWhiteSpace(CreateUpdateUsage.ApplicableItems))
                {
                    var selected = System.Text.Json.JsonSerializer.Deserialize<List<string>>(CreateUpdateUsage.ApplicableItems) ?? new List<string>();
                    IsAddOnProductsSelected = selected.Contains("AddOnProducts");
                    IsShippingFeesSelected = selected.Contains("ShippingFees");
                }
                await FetchProducts();
                await FetchGroupBuys();
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

            if (CreateUpdateUsage.UsableGroupbuysScope == "AllGroupbuys")
            {
                CreateUpdateUsage.GroupbuyIds = Groupbuys.Select(x => x.Id).ToList();
            }
            else
            {
                CreateUpdateUsage.GroupbuyIds = SelectedGroupBuy.ToList();
            }
          
            // Custom validation logic
            if (CreateUpdateUsage.UsableProductsScope == "AllProducts")
            {
                CreateUpdateUsage.ProductIds = Products.Select(x => x.Id).ToList();
            }
            else
            {
                CreateUpdateUsage.ProductIds = SelectedProducts.ToList();
            }
            if (CreateUpdateUsage.ProductIds.Count == 0)
            {
                messageStore?.Add(() => CreateUpdateUsage.ProductIds, "Select at least one.");
                IsUpdating = false;
                return;
            }
          
            if (CreateUpdateUsage.GroupbuyIds.Count == 0)
            {
                messageStore?.Add(() => CreateUpdateUsage.GroupbuyIds, "Select at least one.");
                IsUpdating = false;
                return;
            }

            if (await ValidationsRef.ValidateAll())
            {
                CreateUpdateUsage.StagedSettings = GetJson();
                if (Id == Guid.Empty)
                {
                    await ShoppingCreditUsageSettingAppService.CreateAsync(CreateUpdateUsage);
                }
                else
                {

                    await ShoppingCreditUsageSettingAppService.UpdateAsync(Id, CreateUpdateUsage);
                }
                IsUpdating = false;
                NavigateToShoppingCredits();
            }
            IsUpdating = false;
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

            CreateUpdateUsage.ApplicableItems = System.Text.Json.JsonSerializer.Serialize(selected);
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
public class StagedSetting
{
    public int Spend { get; set; }
    public int Points { get; set; }
}