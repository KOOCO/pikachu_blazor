using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.OrderTransactions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.CashFlowReconciliationStatements;
public partial class CashFlowReconciliationStatement
{
    #region Inject
    private bool IsLoading { get; set; } = false;
    private bool IsAllSelected { get; set; } = false;
    private int TotalCount { get; set; }
    private int PageIndex { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    private string? Sorting { get; set; }
    private string? Filter { get; set; }
    private Modal CreateVoidReasonModal { get; set; }
    private VoidReason VoidReason { get; set; } = new();
    private readonly HashSet<Guid> ExpandedRows = [];
    private Modal CreateCreditNoteReasonModal { get; set; }
    private CreditReason CreditReason { get; set; } = new();
    private IReadOnlyList<OrderTransactionDto> OrderTransactions { get; set; } = [];
    #endregion

    #region Methods
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderTransactionDto> e)
    {
        PageIndex = e.Page - 1;
        await UpdateItemList();
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateItemList()
    {
        try
        {
            IsLoading = true;
            var result = await OrderTransactionAppService.GetListAsync(new GetOrderTransactionListDto
            {
                SkipCount = PageIndex * PageSize,
                MaxResultCount = PageSize,
                Sorting = Sorting,
                Filter = Filter
            });

            OrderTransactions = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task FetchItemDetailsAsync(List<OrderItemDto> orderItems)
    {
        foreach (OrderItemDto item in orderItems)
        {
            if (item.ItemId is null || item.ItemId == Guid.Empty) continue;

            item.Item = await _ItemAppService.GetAsync(item.ItemId.Value);
        }
    }

    async Task IssueInvoice(Guid orderId)
    {
        var msg = await _orderInvoiceAppService.CreateInvoiceAsync(orderId);
        if (!msg.IsNullOrWhiteSpace())
        {
            await _uiMessageService.Error(msg);
        }
        await UpdateItemList();
    }

    async Task OnSearch()
    {
        PageIndex = 0;
        await UpdateItemList();
    }

    void HandleSelectAllChange(ChangeEventArgs e)
    {
        IsAllSelected = e.Value != null && (bool)e.Value;
        foreach (var item in OrderTransactions)
        {
            item.IsSelected = IsAllSelected;
        }
        StateHasChanged();
    }

    public void NavigateToOrderDetails(DataGridRowMouseEventArgs<OrderDto> e)
    {
        var id = e.Item.OrderId;
        NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");
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
        var selectedOrder = OrderTransactions.SingleOrDefault(x => x.IsSelected);
        NavigationManager.NavigateTo($"Orders/OrderShippingDetails/{selectedOrder.OrderId}");
    }

    async Task DownloadExcel()
    {
        try
        {
            int skipCount = PageIndex * PageSize;
            Sorting ??= "OrderNo Ascending";
            var selectedOrder = OrderTransactions.Where(x => x.IsSelected).Select(x => x.OrderId).ToList();
            var remoteStreamContent = await _orderAppService.GetReconciliationListAsExcelFileAsync(new GetOrderListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = 0,
                Filter = Filter,
                OrderIds = selectedOrder
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

    private void CloseVoidReasonModal()
    {
        CreateVoidReasonModal.Hide();
    }

    private async Task ApplyVoidReasonAsync()
    {
        var selectedOrder = OrderTransactions.SingleOrDefault(x => x.IsSelected);
        await _orderAppService.VoidInvoice(selectedOrder.OrderId, VoidReason.Reason);
        var order=await _orderAppService.GetAsync(selectedOrder.OrderId);
        if (order.InvoiceStatus == EnumValues.InvoiceStatus.InvoiceVoided)
        {
            await _uiMessageService.Success(L["Invoicehasvoided"]);
        }
        await CreateVoidReasonModal.Hide();
        await UpdateItemList();
        await InvokeAsync(StateHasChanged);
    }
    private void OpenVoidReasonModal()
    {
        CreateVoidReasonModal.Show();
    }

    private void CloseCreditReasonModal()
    {
        CreateCreditNoteReasonModal.Hide();
    }

    private async Task ApplyCreditReasonAsync()
    {
        var selectedOrder = OrderTransactions.SingleOrDefault(x => x.IsSelected);

        await _orderAppService.CreditNoteInvoice(selectedOrder.OrderId, CreditReason.Reason);

        await CreateCreditNoteReasonModal.Hide();

        await UpdateItemList();

        await InvokeAsync(StateHasChanged);
    }
    private void OpenCreditReasonModal()
    {
        CreditReason.Reason = null;
        CreateCreditNoteReasonModal.Show();
    }
    #endregion
}
public class VoidReason
{
    [Required(ErrorMessage = "This Field Is Required")]
    public string? Reason { get; set; }
}
public class CreditReason
{
    [Required(ErrorMessage = "This Field Is Required")]
    public string? Reason { get; set; }
}