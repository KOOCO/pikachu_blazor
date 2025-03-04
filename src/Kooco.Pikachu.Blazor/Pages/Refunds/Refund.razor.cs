using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Refunds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Refunds;

public partial class Refund
{
    #region Inject
    private List<RefundDto> Refunds = new List<RefundDto>();
    private LoadingIndicator loading { get; set; }

    int PageIndex { get; set; }
    int PageSize { get; set; } = 10;
    string? Sorting { get; set; }
    string? Filter { get; set; }
    int TotalCount { get; set; }
    private bool CanProcessRefund { get; set; }
    private RefundDto SelectedRefund { get; set; }
    private Modal RejectReasonModal;
    private string? RejectReason { get; set; }
    private bool ViewRejectReason { get; set; }

    #endregion

    #region Methods
    protected override async Task OnInitializedAsync()
    {
        CanProcessRefund = await AuthorizationService.IsGrantedAsync(PikachuPermissions.Refund.RefundOrderProcess);
    }
    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<RefundDto> e)
    {
        PageIndex = e.Page - 1;
        await UpdateItemList();
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateItemList()
    {
        try
        {
            if (Sorting.IsNullOrWhiteSpace())
            {
                Sorting = $"{nameof(RefundDto.CreationTime)} desc";
            }
            await loading.Show();
            int skipCount = PageIndex * PageSize;
            var result = await _refundAppService.GetListAsync(new GetRefundListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount,
                Filter = Filter
            });
            Refunds = [.. result.Items];
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRunTime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }
    public async void NavigateToOrderDetails(RefundDto e)
    {
        await loading.Show();

        var id = e.OrderId;
        NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");

        await loading.Hide();
    }
    private async Task RefundApproved(RefundDto rowData)
    {
        try
        {
            
            var confirmed = await _uiMessageService.Confirm(L["Doyouapprovetherefund"]);
            if (!confirmed)
            {
                
                return;
            }
            await loading.Show();
            await _refundAppService.UpdateRefundReviewAsync(rowData.Id, RefundReviewStatus.Proccessing);
            if (rowData.Order?.PaymentMethod == PaymentMethods.LinePay)
            {
                await LinePayAppService.ProcessRefund(rowData.Id);
            }
            else
            {
                await _refundAppService.CheckStatusAndRequestRefundAsync(rowData.Id);
            }

            await UpdateItemList();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());

            await JSRunTime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }

    private async Task OpenRefundRejectModal(RefundDto refund, bool viewRejectReason)
    {
        ViewRejectReason = viewRejectReason;
        SelectedRefund = refund;
        await RejectReasonModal.Show();
    }

    private async Task RefundReject()
    {
        try
        {
            await loading.Show();

            if (SelectedRefund == null || string.IsNullOrEmpty(RejectReason))
            {
                return;
            }
            await _refundAppService.UpdateRefundReviewAsync(SelectedRefund.Id, RefundReviewStatus.ReturnedApplication, RejectReason);
            await RejectReasonModal.Hide();
            await UpdateItemList();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRunTime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }
    private async Task RefundReviewChanged(RefundReviewStatus selectedValue, RefundDto rowData)
    {
        try
        {
            await loading.Show();
            await _refundAppService.UpdateRefundReviewAsync(rowData.Id, selectedValue);
            await UpdateItemList();
        }
        catch (Exception ex)
        {
            await _uiMessageService.Error(ex.GetType().ToString());
            await JSRunTime.InvokeVoidAsync("console.error", ex.ToString());
        }
        finally
        {
            await loading.Hide();
        }
    }
    async void OnSortChange(DataGridSortChangedEventArgs e)
    {
        Sorting = e.FieldName + " " + (e.SortDirection is not SortDirection.Default ? e.SortDirection : string.Empty);

        await UpdateItemList();
    }
    async Task OnSearch()
    {
        PageIndex = 0;

        await UpdateItemList();
    }
    #endregion
}
