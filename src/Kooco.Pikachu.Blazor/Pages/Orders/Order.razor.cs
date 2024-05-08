using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.Blazor.Pages.ItemManagement;
using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.JSInterop;
using NUglify.Html;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace Kooco.Pikachu.Blazor.Pages.Orders
{
    public partial class Order
    {
        private bool IsAllSelected { get; set; } = false;
        private List<OrderDto> Orders { get; set; } = new();
        private int TotalCount { get; set; }
        private OrderDto SelectedOrder { get; set; }
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
        
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
        {
            await JSRuntime.InvokeVoidAsync("removeSelectClass", "mySelectElement");
            await JSRuntime.InvokeVoidAsync("removeInputClass", "startDate");
            await JSRuntime.InvokeVoidAsync("removeInputClass", "endDate");
            PageIndex = e.Page - 1;
            await UpdateItemList();
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
                    EndDate=EndDate
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

        async Task OnSearch(Guid? e=null)
        {
            if (e == Guid.Empty)
            {
                GroupBuyFilter = null;
            }
            else { GroupBuyFilter = e; }
                
            
          
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

            var id = e.Item.Id;
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
            if (ExpandedRows.Contains(e.Item.Id))
            {
                ExpandedRows.Remove(e.Item.Id);
            }
            else
            {
                ExpandedRows.Add(e.Item.Id);
            }

           
        }
        private async void MergeOrders() {
            var orderIds = Orders.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            await _orderAppService.MergeOrdersAsync(orderIds);
           await UpdateItemList();

        }

        public void NavigateToOrderPrint()
        {
            var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
            NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{selectedOrder.Id}");
        }
        public async void IssueInvoice()
        {
            var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
          await  _electronicInvoiceAppService.CreateInvoiceAsync(selectedOrder.Id);
           
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
            catch (Exception e)
            {

                throw e;
            }


        }

    }
}
