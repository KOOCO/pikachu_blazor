using Blazorise;
using Blazorise.Components;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using ECPay.Payment.Integration.SPCheckOut.ExtendArguments;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.LogisticsProviders;
using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement;

public partial class GroupBuyList
{
    #region Inject
    public List<GroupBuyDto> GroupBuyListItem { get; set; }
    public bool IsAllSelected { get; private set; } = false;
    private readonly ILogisticsProvidersAppService _LogisticsProvidersAppService;
    private readonly IUiMessageService _uiMessageService;
    public List<LogisticsProviderSettingsDto> LogisticsProviders = [];
    private readonly IGroupBuyAppService _groupBuyAppService;
    int _pageIndex = 1;
    int _pageSize = 10;
    int Total = 0;
    public DateTime? CreatedDate { get; set; }
    private string Sorting = nameof(GroupBuy.GroupBuyName);
    private LoadingIndicator loading { get; set; } = new();
    private Autocomplete<KeyValueDto, Guid?> AutocompleteField { get; set; }
    private GetGroupBuyInput Filters { get; set; }
    private List<KeyValueDto> ItemsList { get; set; } = new();
    bool AdvanceFiltersVisible = false;
    bool CreditCard { get; set; }
    bool BankTransfer { get; set; }
    bool IsCashOnDelivery { get; set; }
    bool IsLinePay { get; set; }
    #endregion

    #region Constructor
    public GroupBuyList(
        IGroupBuyAppService groupBuyAppService,
        IUiMessageService messageService,
         ILogisticsProvidersAppService LogisticsProvidersAppService
    )
    {
        _groupBuyAppService = groupBuyAppService;
        _uiMessageService = messageService;
        GroupBuyListItem = new List<GroupBuyDto>();
        Filters= new GetGroupBuyInput();
        _LogisticsProvidersAppService= LogisticsProvidersAppService;
    }
    #endregion

    #region Methods
    protected override async Task OnInitializedAsync()
    {
     

        LogisticsProviders = await _LogisticsProvidersAppService.GetAllAsync();
    }
    private void HandleSelectAllChange(ChangeEventArgs e)
    {
        IsAllSelected = (bool?)e.Value ?? false;
        GroupBuyListItem.ForEach(item =>
        {
            item.IsSelected = IsAllSelected;
        });
        StateHasChanged();

    }
    private async Task AdvanceFiltersVisibleChangedAsync() {
        AdvanceFiltersVisible = !AdvanceFiltersVisible;
        StateHasChanged();

    }
    void OnShippingMethodCheckedChange(string method, ChangeEventArgs e)
    {
        var value = (bool)(e?.Value ?? false);

        if (value)
        {
            // If the selected method is SevenToEleven or FamilyMart, uncheck the corresponding C2C method
            if (method == "SevenToEleven1" && Filters.ShippingMethodList.Contains("SevenToElevenC2C"))
            {
                Filters.ShippingMethodList.Remove("SevenToElevenC2C");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenC2C");
            }
            else if (method is "SevenToElevenFrozen" && Filters.ShippingMethodList.Contains("SevenToElevenC2C"))
            {
                Filters.ShippingMethodList.Remove("SevenToElevenC2C");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenC2C");
            }
            else if (method == "FamilyMart1" && Filters.ShippingMethodList.Contains("FamilyMartC2C"))
            {
                Filters.ShippingMethodList.Remove("FamilyMartC2C");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "FamilyMartC2C");
            }
            else if (method == "SevenToElevenC2C" && Filters.ShippingMethodList.Contains("SevenToEleven1"))
            {
                Filters.ShippingMethodList.Remove("SevenToEleven1");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToEleven1");
            }
            else if (method is "SevenToElevenC2C" && Filters.ShippingMethodList.Contains("SevenToElevenFrozen"))
            {
                Filters.ShippingMethodList.Remove("SevenToElevenFrozen");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "SevenToElevenFrozen");
            }
            else if (method == "FamilyMartC2C" && Filters.ShippingMethodList.Contains("FamilyMart1"))
            {
                Filters.ShippingMethodList.Remove("FamilyMart1");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "FamilyMart1");
            }
            else if (method == "BlackCat1" && Filters.ShippingMethodList.Contains("BlackCatFreeze"))
            {
                Filters.ShippingMethodList.Remove("BlackCatFreeze");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFreeze");
            }
            else if (method == "BlackCat1" && Filters.ShippingMethodList.Contains("BlackCatFrozen"))
            {
                Filters.ShippingMethodList.Remove("BlackCatFrozen");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFrozen");
            }
            else if (method == "BlackCatFreeze" && Filters.ShippingMethodList.Contains("BlackCat1"))
            {
                Filters.ShippingMethodList.Remove("BlackCat1");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCat1");
            }
            else if (method == "BlackCatFreeze" && Filters.ShippingMethodList.Contains("BlackCatFrozen"))
            {
                Filters.ShippingMethodList.Remove("BlackCatFrozen");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFrozen");
            }
            else if (method == "BlackCatFrozen" && Filters.ShippingMethodList.Contains("BlackCat1"))
            {
                Filters.ShippingMethodList.Remove("BlackCat1");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCat1");
            }
            else if (method == "BlackCatFrozen" && Filters.ShippingMethodList.Contains("BlackCatFreeze"))
            {
                Filters.ShippingMethodList.Remove("BlackCatFreeze");
                _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", "BlackCatFreeze");
            }
        }

        // Update the selected method in the Filters.ShippingMethodList
        if (value)
        {
            if (method == "DeliveredByStore")
            {
                foreach (var item in Filters.ShippingMethodList)
                {
                    _JSRuntime.InvokeVoidAsync("uncheckOtherCheckbox", item);

                }
                Filters.ShippingMethodList = new List<string>();
            }
            else
            {
                Filters.ShippingMethodList.Remove("DeliveredByStore");

            }
            Filters.ShippingMethodList.Add(method);
        }
        else
        {
            Filters.ShippingMethodList.Remove(method);
        }

        // Serialize the updated list and assign it to ExcludeShippingMethod
        Filters.ExcludeShippingMethod = JsonConvert.SerializeObject(Filters.ShippingMethodList);
    }
    private async Task UpdateGroupBuyList()
    {
        try
        {
            int skipCount = _pageIndex * _pageSize;
            Filters.Sorting = Sorting;
            Filters.MaxResultCount = _pageSize;
            Filters.SkipCount = skipCount;
            List<string> paymentMethods = new List<string>();

            if (CreditCard) paymentMethods.Add("Credit Card");
            if (BankTransfer) paymentMethods.Add("Bank Transfer");
            if (IsCashOnDelivery) paymentMethods.Add("Cash On Delivery");
            if (IsLinePay) paymentMethods.Add("LinePay");

            Filters.PaymentMethod = string.Join(" , ", paymentMethods);
            PagedResultDto<GroupBuyDto> result = await _groupBuyAppService.GetListAsync(Filters);

            GroupBuyListItem = [.. result.Items];

            var tenantUrl = (await MyTenantAppService.FindTenantUrlAsync(CurrentTenant.Id))?.TrimEnd('/');

            foreach (GroupBuyDto groupBuyItem in GroupBuyListItem)
            {
                groupBuyItem.EntryURL = $"{tenantUrl}/groupBuy/{groupBuyItem.Id}";
            }

            Total = (int)result.TotalCount;
            //ItemsLookup = await _groupBuyAppService.GetAllGroupBuyLookupAsync();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType()?.ToString());
            Console.WriteLine(ex.ToString());
        }
    }
    private async void ApplyFilters() {
        await UpdateGroupBuyList();
        await InvokeAsync(StateHasChanged);


    }
    private async void ResetFilters() {
        Filters = new();
        CreditCard =false;
        IsCashOnDelivery =false;
        IsLinePay = false;
        BankTransfer = false;
        await UpdateGroupBuyList();
        await InvokeAsync(StateHasChanged);
    }

    private async Task CopyAsync()
    {

        var id = GroupBuyListItem.Where(x => x.IsSelected == true).Select(x => x.Id).FirstOrDefault();

        var copy = await _groupBuyAppService.CopyAsync(id);
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/Edit/" + copy.Id);


    }

    public bool IsShippingMethodEnabled(string method)
    {
        if (LogisticsProviders is { Count: 0 }) return false;

        DeliveryMethod deliveryMethod = Enum.Parse<DeliveryMethod>(method);

        if (deliveryMethod is DeliveryMethod.HomeDelivery)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.HomeDelivery).FirstOrDefault()?.IsEnabled ?? false;

        else if (deliveryMethod is DeliveryMethod.PostOffice ||
                 deliveryMethod is DeliveryMethod.FamilyMart1 ||
                 deliveryMethod is DeliveryMethod.SevenToEleven1 ||
                 deliveryMethod is DeliveryMethod.SevenToElevenFrozen ||
                 deliveryMethod is DeliveryMethod.BlackCat1 ||
                 deliveryMethod is DeliveryMethod.BlackCatFreeze ||
                 deliveryMethod is DeliveryMethod.BlackCatFrozen)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.GreenWorldLogistics).FirstOrDefault()?.IsEnabled ?? false;

        else if (deliveryMethod is DeliveryMethod.FamilyMartC2C ||
                 deliveryMethod is DeliveryMethod.SevenToElevenC2C)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.GreenWorldLogisticsC2C).FirstOrDefault()?.IsEnabled ?? false;

        else if (deliveryMethod is DeliveryMethod.TCatDeliveryNormal ||
                 deliveryMethod is DeliveryMethod.TCatDeliveryFreeze ||
                 deliveryMethod is DeliveryMethod.TCatDeliveryFrozen ||
                 deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenNormal ||
                 deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                 deliveryMethod is DeliveryMethod.TCatDeliverySevenElevenFrozen)
            return LogisticsProviders.Where(w => w.LogisticProvider is LogisticProviders.TCat).FirstOrDefault()?.IsEnabled ?? false;

        else return false;
    }
    async Task OnSearch()
    {
        _pageIndex = 0;
        await UpdateGroupBuyList();
    }
    private async Task DeleteSelectedAsync()
    {
        try
        {
            var groupBuyItemsids = GroupBuyListItem.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (groupBuyItemsids.Count > 0)
            {
                var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                if (confirmed)
                {
                    await loading.Show();
                    await _groupBuyAppService.DeleteManyGroupBuyItemsAsync(groupBuyItemsids);
                    await UpdateGroupBuyList();
                    IsAllSelected = false;
                    await loading.Hide();
                }
            }
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.Message.ToString());
            Console.WriteLine(ex.ToString());
        }
    }
    public async Task OnGroupBuyAvaliablityChanged(Guid id)
    {
        try
        {
            await loading.Show();
            var freebie = GroupBuyListItem.Where(x => x.Id == id).First();
            freebie.IsGroupBuyAvaliable = !freebie.IsGroupBuyAvaliable;
            await _groupBuyAppService.ChangeGroupBuyAvailability(id);
            await UpdateGroupBuyList();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();
        }
        catch (BusinessException ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.Code.ToString());
            Console.WriteLine(ex.ToString());
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            Console.WriteLine(ex.ToString());
        }
    }
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GroupBuyDto> e)
    {
        try
        {
            await loading.Show();
            _pageIndex = e.Page - 1;
            await UpdateGroupBuyList();
            await InvokeAsync(StateHasChanged);
            await loading.Hide();
        }
        catch (Exception ex)
        {
            await loading.Hide();
            Console.WriteLine(ex.ToString());
        }
    }

    public void CreateNewItem()
    {
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/New");
    }

    public void OnEditItem(DataGridRowMouseEventArgs<GroupBuyDto> e)
    {
        var id = e.Item.Id;
        NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyList/Edit/" + id);
    }

    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        await loading.Show();
        Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
        await UpdateGroupBuyList();
        await loading.Hide();
    }

    private async Task CopyLinkToClipboard(string? entryUrl)
    {
        if (entryUrl.IsNullOrEmpty()) return;

        string path = entryUrl;

        await _JSRuntime.InvokeVoidAsync("parent.navigator.clipboard.writeText", path);
    }
    #endregion
}
