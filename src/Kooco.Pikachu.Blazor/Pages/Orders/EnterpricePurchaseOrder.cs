using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class EnterpricePurchaseOrder
    {
        #region Inject
        private Dictionary<Guid, List<OrderDeliveryDto>> OrderDeliveriesByOrderId { get; set; } = [];

        private bool IsAllSelected { get; set; } = false;
        private List<OrderDeliveryDto> OrderDeliveries { get; set; }
        private List<OrderDto> Orders { get; set; } = new();
        private int TotalCount { get; set; }
        private OrderDto? SelectedOrder { get; set; }
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
        private bool loading { get; set; } = true;
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
        private bool FiltersVisible { get; set; } = false;
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

        private async Task GetGroupBuyList()
        {
            loading=true;
            GroupBuyList = await _groupBuyAppService.GetGroupBuyLookupAsync();
            loading=false;

        }

        private async Task UpdateItemList()
        {
            try
            {
                loading=true;
                int skipCount = PageIndex * PageSize;
                var result = await _orderAppService.GetListAsync(new GetOrderListDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount,
                    Filter = Filter,
                    GroupBuyId = GroupBuyFilter,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    DeliveryMethod = DeliveryMethod
                });
                Orders = result?.Items.ToList() ?? new List<OrderDto>();
                TotalCount = (int?)result?.TotalCount ?? 0;

                loading=false;
            }
            catch (Exception ex)
            {
                loading=false;
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
                loading=true;

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

                loading=false;
            }
            catch (Exception ex)
            {
                loading=false;

                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }

        public void OnCheckboxValueChanged(bool e, OrderDto order)
        {
            order.IsSelected = e;

            if (SelectedTabName is not "ToBeShipped") return;

            if (order.IsSelected) OrdersSelected.Add(order);

            else OrdersSelected.Remove(order);

            NormalCount = OrdersSelected.Sum(order => order.OrderItems.Count(item => item.DeliveryTemperature is ItemStorageTemperature.Normal));

            FreezeCount = OrdersSelected.Sum(order => order.OrderItems.Count(item => item.DeliveryTemperature is ItemStorageTemperature.Freeze));

            FrozenCount = OrdersSelected.Sum(order => order.OrderItems.Count(item => item.DeliveryTemperature is ItemStorageTemperature.Frozen));
        }

        async Task OnSearch(Guid? e = null)
        {
            if (e == Guid.Empty)
            {
                GroupBuyFilter = null;
            }

            else
            {
                GroupBuyFilter = e;
            }

            SelectedGroupBuy = e;

            PageIndex = 0;

            await LoadTabAsPerNameAsync("EnterpricePurchase");
        }

        async Task OnDateChange(DateTime? startDate, DateTime? endDate)
        {
            StartDate = startDate;
            EndDate = endDate;

            PageIndex = 0;

            await LoadTabAsPerNameAsync("EnterpricePurchase");
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

        public async void NavigateToOrderDetails(OrderDto e)
        {
            loading=true;

            var id = e.OrderId;
            NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");

            loading=false;
        }

        bool ShowCombineButton()
        {
            var selectedOrders = Orders.Where(x => x.IsSelected).ToList();

            if (selectedOrders.Count > 1)
            {
                var firstSelectedOrder = selectedOrders.First();
                bool allMatch = true;

                foreach (var order in selectedOrders)
                {
                    if (order.GroupBuyId != firstSelectedOrder.GroupBuyId ||
                         order.CustomerName != firstSelectedOrder.CustomerName ||
                        order.CustomerEmail != firstSelectedOrder.CustomerEmail ||
                       order.ShippingStatus != firstSelectedOrder.ShippingStatus ||


                        order.OrderType != null
                        || order.ShippingStatus == ShippingStatus.Shipped
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
            SelectedOrder = e.Item;
        }

        private bool IsRowExpanded(OrderDto order)
        {
            return ExpandedOrderId == order.Id;
        }

        private async void MergeOrders()
        {
            var orderIds = Orders.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            await _orderAppService.MergeOrdersAsync(orderIds);
            await UpdateItemList();
        }

        public async Task NavigateToOrderPrint()
        {
            List<OrderDto> selectedOrders = [.. Orders.Where(w => w.IsSelected)];

            List<Guid> selectedIds = [.. selectedOrders.Select(s => s.Id)];

            string selectedIdsStr = JsonConvert.SerializeObject(selectedIds);

            await JSRuntime.InvokeVoidAsync("openInNewTab", $"Orders/OrderShippingDetails/{selectedIdsStr}");
        }

        public async void IssueInvoice()
        {
            try
            {
                loading=true;
                var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
                await _electronicInvoiceAppService.CreateInvoiceAsync(selectedOrder.Id);
                loading=false;
                await _uiMessageService.Success(L["InvoiceIssueSuccessfully"]);
                await UpdateItemList();


            }
            catch (Exception ex)
            {
                loading=false;
                await _uiMessageService.Error(ex.Message.ToString());


            }

        }

        async Task DownloadExcel()
        {
            try
            {
                int skipCount = PageIndex * PageSize;
                var orderIds = Orders.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                Sorting = Sorting != null ? Sorting : "OrderNo Ascending";

                var remoteStreamContent = await _orderAppService.GetListAsExcelFileAsync(new GetOrderListDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount,
                    Filter = Filter,
                    OrderIds = orderIds,
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
}
