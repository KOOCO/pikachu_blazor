using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Hangfire.Server;
using Kooco.Pikachu.Blazor.Pages.ItemManagement;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.StoreLogisticOrders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using NUglify.Html;
using OneOf.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Account.Web;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using static Kooco.Pikachu.Permissions.PikachuPermissions;

namespace Kooco.Pikachu.Blazor.Pages.Orders;

public partial class Order
{
    #region Inject
    private Dictionary<Guid, List<OrderDeliveryDto>> OrderDeliveriesByOrderId { get; set; } = [];

    private bool IsAllSelected { get; set; } = false;
    private List<OrderDeliveryDto> OrderDeliveries { get; set; }
    private List<OrderDto> Orders { get; set; } = new();
    private int TotalCount { get; set; }
    private OrderDto SelectedOrder { get; set; }
    private OrderItemDto SelectedOrderItem { get; set; }
    private OrderDeliveryDto SelectedOrderDelivery { get; set; }
    private Guid? GroupBuyFilter { get; set; }
    private int PageIndex { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    private Guid? SelectedGroupBuy { get; set; }
    private DateTime? StartDate { get; set; }
    private DateTime? EndDate { get; set; }
    private string? Sorting { get; set; }
    private string? Filter { get; set; }
    private bool isOrderCombine { get; set; } = false;
    private readonly HashSet<Guid> ExpandedRows = new();
    private LoadingIndicator loading { get; set; }
    private List<KeyValueDto> GroupBuyList { get; set; } = new();
    private List<ShippingStatus> ShippingStatuses { get; set; } = [];
    private List<DeliveryMethod> DeliveryMethods { get; set; } = [];
    private List<OrderDto> OrdersSelected = [];
    private string SelectedTabName = "All";

    private int NormalCount = 0;
    private int FreezeCount = 0;
    private int FrozenCount = 0;
    private DeliveryMethod? DeliveryMethod = null;

    private Guid? ExpandedOrderId = null;
    #endregion

    #region Methods
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
    {
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "mySelectElement");
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "shippingMethodSelectElem");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "startDate");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "endDate");
        PageIndex = e.Page - 1;
        await UpdateItemList();
        await GetGroupBuyList();
        await InvokeAsync(StateHasChanged);
    }

    public void SelectedTabChanged(string e)
    {
        SelectedTabName = e;

        TotalCount = 0;

        StateHasChanged();
    }

    public async Task OnGenerateDeliveryNumber(MouseEventArgs e)
    {
        await loading.Show();

        List<Guid> orderIds = [.. Orders.Where(w => w.IsSelected).Select(s => s.OrderId)];

        List<Dictionary<string, string>> responseResults = []; string wholeErrorMessage = string.Empty;

        foreach (Guid orderId in orderIds) 
        {
            OrderDto order = await _orderAppService.GetAsync(orderId);

            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

            foreach (OrderDeliveryDto orderDelivery in orderDeliveries)
            {
                if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.PostOffice || 
                    orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.BlackCat1)
                {
                    ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id);

                    if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                }

                else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToEleven1 ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMart1 ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C)
                {
                    ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId, orderDelivery.Id);

                    if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                }

                else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                {
                    PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id);

                    if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                }

                else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze ||
                         orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                {
                    PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id);

                    if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                }

                else if (orderDelivery.DeliveryMethod is EnumValues.DeliveryMethod.DeliveredByStore)
                {
                    LogisticProviders? logisticProvider = null; DeliveryMethod? deliveryMethod = null; ItemStorageTemperature? temperature = null;

                    List<DeliveryTemperatureCostDto> deliveryTemperatureCosts = await _DeliveryTemperatureCostAppService.GetListAsync();

                    foreach (DeliveryTemperatureCostDto entity in deliveryTemperatureCosts)
                    {
                        if (orderDelivery.Items.Any(a => a.DeliveryTemperature == entity.Temperature))
                        {
                            logisticProvider = entity.LogisticProvider;

                            deliveryMethod = entity.DeliveryMethod;

                            temperature = entity.Temperature;
                        }
                    }

                    if (temperature is ItemStorageTemperature.Normal)
                    {
                        if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.FamilyMart1 ||
                            logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.SevenToEleven1 ||
                            logisticProvider is LogisticProviders.GreenWorldLogisticsC2C && deliveryMethod is EnumValues.DeliveryMethod.FamilyMartC2C ||
                            logisticProvider is LogisticProviders.GreenWorldLogisticsC2C && deliveryMethod is EnumValues.DeliveryMethod.SevenToElevenC2C)
                        {
                            ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                        }

                        else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.PostOffice ||
                                 logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.BlackCat1)
                        {
                            ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                        }

                        else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryNormal)
                        {
                            PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                        }

                        else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenNormal)
                        {
                            PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                        }
                    }

                    else if (temperature is ItemStorageTemperature.Freeze)
                    {
                        if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.BlackCatFreeze)
                        {
                            ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                        }

                        else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFreeze)
                        {
                            PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                        }

                        else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFreeze)
                        {
                            PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                        }
                    }

                    else if (temperature is ItemStorageTemperature.Frozen)
                    {
                        if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliveryFrozen)
                        {
                            PrintObtResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCatDeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                        }

                        else if (logisticProvider is LogisticProviders.TCat && deliveryMethod is EnumValues.DeliveryMethod.TCatDeliverySevenElevenFrozen)
                        {
                            PrintOBTB2SResponse? response = await _StoreLogisticsOrderAppService.GenerateDeliveryNumberForTCat711DeliveryAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (response is null || response.Data is null) AddToDictionary(responseResults, order.OrderNo, response.Message);
                        }

                        else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.BlackCatFrozen)
                        {
                            ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateHomeDeliveryShipmentOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                        }

                        else if (logisticProvider is LogisticProviders.GreenWorldLogistics && deliveryMethod is EnumValues.DeliveryMethod.SevenToElevenFrozen)
                        {
                            ResponseResultDto result = await _StoreLogisticsOrderAppService.CreateStoreLogisticsOrderAsync(orderId, orderDelivery.Id, deliveryMethod);

                            if (result.ResponseCode is not "1") AddToDictionary(responseResults, order.OrderNo, result.ResponseMessage);
                        }
                    }
                }
            }
        }

        if (responseResults is { Count: > 0 })
        {
            foreach (Dictionary<string, string> response in responseResults)
            {
                if (wholeErrorMessage.IsNullOrEmpty())
                    wholeErrorMessage = wholeErrorMessage.Insert(0, response.Keys.First() + " -> " + response.Values.First());

                else
                    wholeErrorMessage = wholeErrorMessage.Insert(wholeErrorMessage.Length + 1, Environment.NewLine + response.Keys.First() + " -> " + response.Values.First());
            }

            await loading.Hide();

            await _uiMessageService.Error(wholeErrorMessage);

            return;
        }

        await loading.Hide();

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    public void AddToDictionary(List<Dictionary<string, string>> dictionaryList, string key, string value)
    {
        Dictionary<string, string> keyValuePairs = [];

        keyValuePairs.Add(key, value);

        dictionaryList.Add(keyValuePairs);
    }

    public async Task OnOrderDeliveryDataReadAsync(DataGridReadDataEventArgs<OrderDeliveryDto> e, Guid orderId)
    {
        OrderDeliveries = [];

        if (!OrderDeliveriesByOrderId.ContainsKey(orderId))
        {
            List<OrderDeliveryDto> orderDeliveries = await _OrderDeliveryAppService.GetListByOrderAsync(orderId);

            OrderDeliveriesByOrderId[orderId] = orderDeliveries;
        }

        StateHasChanged();
    }

    public List<OrderDeliveryDto> GetOrderDeliveries(Guid orderId)
    {
        return OrderDeliveriesByOrderId.ContainsKey(orderId) ? OrderDeliveriesByOrderId[orderId] : [];
    }

    public async Task OnTabLoadDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e, string tabName)
    {
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "mySelectElement");
        await JSRuntime.InvokeVoidAsync("removeSelectClass", "shippingMethodSelectElem");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "startDate");
        await JSRuntime.InvokeVoidAsync("removeInputClass", "endDate");

        PageIndex = e.Page - 1;

        await LoadTabAsPerNameAsync(tabName);

        await GetGroupBuyList();

        await InvokeAsync(StateHasChanged);
    }

    private async Task GetGroupBuyList() {
        await loading.Show();
        GroupBuyList = await _groupBuyAppService.GetGroupBuyLookupAsync();
        await loading.Hide();
    
    }
    
    private async Task UpdateItemList()
    {
        try
        {
            await loading.Show();
            int skipCount = PageIndex * PageSize;
            var result = await _orderAppService.GetListAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                GroupBuyId=GroupBuyFilter,
                StartDate=StartDate,
                EndDate=EndDate,
                DeliveryMethod = DeliveryMethod
            });
            Orders = result?.Items.ToList() ?? new List<OrderDto>();
            TotalCount = (int?)result?.TotalCount ?? 0;

            await loading.Hide();
        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.GetType().ToString());
            Console.WriteLine(ex.ToString());
        }
    }

    public async Task OnShippingMethodSelectChangeAsync(string e)
    {
        if (!e.IsNullOrWhiteSpace() && !e.IsNullOrEmpty()) DeliveryMethod = Enum.Parse<DeliveryMethod>(e);

        else DeliveryMethod = null;

        if (SelectedTabName is "All") await UpdateItemList();

        else await LoadTabAsPerNameAsync(SelectedTabName);
    }

    public async Task LoadTabAsPerNameAsync(string tabName)
    {
        try
        {
            await loading.Show();

            int skipCount = PageIndex * PageSize;
            
            PagedResultDto<OrderDto> result = await _orderAppService.GetListAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                GroupBuyId = GroupBuyFilter,
                StartDate = StartDate,
                EndDate = EndDate,
                ShippingStatus = Enum.Parse<ShippingStatus>(tabName),
                DeliveryMethod = DeliveryMethod
            });

            Orders = [];

            Orders = [.. result.Items];

            TotalCount = (int?)result?.TotalCount ?? 0;

            await loading.Hide();
        }
        catch (Exception ex)
        {
            await loading.Hide();

            await _uiMessageService.Error(ex.GetType().ToString());
        }
    }

    public void OnCheckboxValueChanged(bool e, OrderDto order)
    {
        order.IsSelected = e;

        if (SelectedTabName is not "ToBeShipped") return;

        if (order.IsSelected) OrdersSelected.Add(order);

        else OrdersSelected.Remove(order);

        NormalCount = OrdersSelected.Any(a => a.OrderItems.Any(ai => ai.DeliveryTemperature is ItemStorageTemperature.Normal)) ? NormalCount + 1 : NormalCount;

        FreezeCount = OrdersSelected.Any(a => a.OrderItems.Any(ai => ai.DeliveryTemperature is ItemStorageTemperature.Freeze)) ? FreezeCount + 1 : FreezeCount;

        FrozenCount = OrdersSelected.Any(a => a.OrderItems.Any(ai => ai.DeliveryTemperature is ItemStorageTemperature.Frozen)) ? FrozenCount + 1 : FrozenCount;
    }

    async Task OnSearch(Guid? e=null)
    {
        if (e == Guid.Empty) GroupBuyFilter = null;

        else GroupBuyFilter = e;

        PageIndex = 0;
      
        await UpdateItemList();
    }
  
    void HandleSelectAllChange(ChangeEventArgs e)
    {
        IsAllSelected = e.Value != null ? (bool)e.Value : false;
        Orders.ForEach(item =>
        {
            item.IsSelected = IsAllSelected;
        });
        StateHasChanged();
    }

    public async void NavigateToOrderDetails(DataGridRowMouseEventArgs<OrderDto> e)
    {
        await loading.Show();

        var id = e.Item.OrderId;
        NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");

        await loading.Hide();
    }

    bool ShowCombineButton()
    {
        var selectedOrders = Orders.Where(x => x.IsSelected).ToList();

        if (selectedOrders.Count>1)
        {
            var firstSelectedOrder = selectedOrders.First();
            bool allMatch = true;

            foreach (var order in selectedOrders)
            {
                if (order.GroupBuyId != firstSelectedOrder.GroupBuyId ||
                     order.CustomerName != firstSelectedOrder.CustomerName ||
                    order.CustomerEmail != firstSelectedOrder.CustomerEmail ||
                   order.ShippingStatus != firstSelectedOrder.ShippingStatus ||
                
                   
                    order.OrderType!=null
                    || order.ShippingStatus==ShippingStatus.Shipped
                    || order.ShippingStatus == ShippingStatus.Completed
                    || order.ShippingStatus == ShippingStatus.Closed)
                {
                    // If any property doesn't match, set allMatch to false and break the loop
                    allMatch = false;
                    break;
                }
            }

            if (allMatch)
            {
                // All selected orders have the same values for the specified properties
                Console.WriteLine("All selected orders have the same values.");
                return true;
            }
            else
            {
                // Not all selected orders have the same values for the specified properties
                Console.WriteLine("Selected orders have different values for the specified properties.");
                return false;
            }
        }
        else
        {
            // No selected orders found
            Console.WriteLine("No selected orders found.");
            return false;
        }


    }

    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
        await UpdateItemList();
    }

    void ToggleRow(DataGridRowMouseEventArgs<OrderDto> e)
    {
        if (ExpandedOrderId == e.Item.Id) ExpandedOrderId = null;

        else ExpandedOrderId = e.Item.Id;

        if (ExpandedRows.Contains(e.Item.Id))
        {
            ExpandedRows.Remove(e.Item.Id);
        }
        else
        {
            ExpandedRows.Add(e.Item.Id);
        }
    }

    private bool IsRowExpanded(OrderDto order)
    {
        return ExpandedOrderId == order.Id;
    }

    private async void MergeOrders() 
    {
        var orderIds = Orders.Where(x => x.IsSelected).Select(x => x.OrderId).ToList();
        await _orderAppService.MergeOrdersAsync(orderIds);
        await UpdateItemList();
    }

    public async Task NavigateToOrderPrint()
    {
        List<OrderDto> selectedOrders = [.. Orders.Where(w => w.IsSelected)];

        List<Guid> selectedIds = [.. selectedOrders.Select(s => s.OrderId)];

        string selectedIdsStr = JsonConvert.SerializeObject(selectedIds);

        await JSRuntime.InvokeVoidAsync("openInNewTab", $"Orders/OrderShippingDetails/{selectedIdsStr}");
    }
    
    public async void IssueInvoice()
    {
        try
        {
            await loading.Show();
            var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
            await _electronicInvoiceAppService.CreateInvoiceAsync(selectedOrder.OrderId);
            await loading.Hide();
            await _uiMessageService.Success(L["InvoiceIssueSuccessfully"]);
            await UpdateItemList();


        }
        catch (Exception ex)
        {
            await loading.Hide();
            await _uiMessageService.Error(ex.Message.ToString());
            

        }
       
    }
    
    async Task DownloadExcel()
    {
        try
        {
            int skipCount = PageIndex * PageSize;
            var orderIds = Orders.Where(x => x.IsSelected).Select(x=>x.Id).ToList();
            Sorting = Sorting != null ? Sorting : "OrderNo Ascending";

            var remoteStreamContent = await _orderAppService.GetListAsExcelFileAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter,
                OrderIds=orderIds,
            });

            using (var responseStream = remoteStreamContent.GetStream())
            {
                // Create Excel file from the stream
                using (var memoryStream = new MemoryStream())
                {
                    await responseStream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Convert MemoryStream to byte array
                    var excelData = memoryStream.ToArray();

                    // Trigger the download using JavaScript interop
                    await JSRuntime.InvokeVoidAsync("downloadFile", new
                    {
                        ByteArray = excelData,
                        FileName = "ReconciliationStatement.xlsx",
                        ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                    });
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion
}
