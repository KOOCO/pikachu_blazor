using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Blazorise;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.JSInterop;
using System.IO;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowReconciliationStatements
{
    public partial class CashFlowReconciliationStatement
    {
        private bool IsAllSelected { get; set; } = false;
        private List<OrderDto> Orders { get; set; } = new();
        private int TotalCount { get; set; }
        private OrderDto SelectedOrder { get; set; }
        private int PageIndex { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private string? Sorting { get; set; }
        private string? Filter { get; set; }

        private readonly HashSet<Guid> ExpandedRows = new();
        private LoadingIndicator loading { get; set; }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderDto> e)
        {
            PageIndex = e.Page - 1;
            await UpdateItemList();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateItemList()
        {
            try
            {
                await loading.Show();
                int skipCount = PageIndex * PageSize;
                var result = await _orderAppService.GetReconciliationListAsync(new GetOrderListDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount,
                    Filter = Filter
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

        async Task OnSearch()
        {
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
        public void NavigateToOrderPrint()
        {
            var selectedOrder = Orders.SingleOrDefault(x => x.IsSelected);
            NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{selectedOrder.Id}");
        }
        async Task DownloadExcel()
        {
            try
            {
                int skipCount = PageIndex * PageSize;
                Sorting = Sorting != null ? Sorting : "OrderNo Ascending";
                var selectedOrder = Orders.Where(x => x.IsSelected).Select(x=>x.Id).ToList();
                var remoteStreamContent = await _orderAppService.GetReconciliationListAsExcelFileAsync(new GetOrderListDto
                {
                    Sorting = Sorting,
                    MaxResultCount =PageSize,
                    SkipCount = 0,
                    Filter = Filter,
                    OrderIds=selectedOrder
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
