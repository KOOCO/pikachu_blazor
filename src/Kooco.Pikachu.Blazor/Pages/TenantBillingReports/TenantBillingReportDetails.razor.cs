using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Blazorise;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.TenantBillingReports
{
    public partial class TenantBillingReportDetails
    {
        [Parameter]
        public string Id { get; set; }
        private GroupBuyReportDetailsDto ReportDetails { get; set; }
        private List<OrderDto> Orders { get; set; } = new();
        private int TotalCount { get; set; }
        private OrderDto SelectedOrder { get; set; }
        private int PageIndex { get; set; } = 1;
        private int PageSize { get; set; } = 10;
        private string? Sorting { get; set; }
        private string? Filter { get; set; }

        private readonly HashSet<Guid> ExpandedRows = new();
        private LoadingIndicator loading { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ReportDetails = await _groupBuyAppService.GetGroupBuyTenantReportDetailsAsync(Guid.Parse(Id));
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
        }
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
                var result = await _orderAppService.GetTenantOrderListAsync(new GetOrderListDto
                {
                    Sorting = Sorting,
                    MaxResultCount = PageSize,
                    SkipCount = skipCount,
                    Filter = Filter,
                    GroupBuyId = Guid.Parse(Id)
                });
                Orders = result?.Items.ToList() ?? new List<OrderDto>();
                TotalCount = (int?)result?.TotalCount ?? 0;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                await loading.Hide();
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

        async Task DownloadExcel()
        {
            try
            {
                await loading.Show();
                var remoteStreamContent = await _groupBuyAppService.GetTenantsListAsExcelFileAsync(Guid.Parse(Id));
                using var responseStream = remoteStreamContent.GetStream();
                // Create Excel file from the stream
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Convert MemoryStream to byte array
                var excelData = memoryStream.ToArray();

                // Trigger the download using JavaScript interop
                await JSRuntime.InvokeVoidAsync("downloadFile", new
                {
                    ByteArray = excelData,
                    FileName = "TenantGroupBuyReport.xlsx",
                    ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                });
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync(ex.ToString());
            }
            finally
            {
                await loading.Hide();
            }
        }

    }
}
