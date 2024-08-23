using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Refunds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.Refunds
{
    public partial class Refund
    {

        private List<RefundDto> Refunds = new List<RefundDto>();
        private LoadingIndicator loading { get; set; }

        int PageIndex { get; set; }
        int PageSize { get; set; } = 10;
        string? Sorting { get; set; }
        string? Filter { get; set; }
        int TotalCount { get; set; }
        private bool CanProcessRefund { get; set; }
        protected override async Task OnInitializedAsync()
        {
            CanProcessRefund = await AuthorizationService
         .IsGrantedAsync(PikachuPermissions.Refund.RefundOrderProcess);
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
                Refunds = result.Items.ToList();
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
        public async void NavigateToOrderDetails(DataGridRowMouseEventArgs<RefundDto> e)
        {
            await loading.Show();

            var id = e.Item.OrderId;
            NavigationManager.NavigateTo($"Orders/OrderDetails/{id}");

            await loading.Hide();
        }
        private async Task RefundApproved(RefundDto rowData)
        {
            try
            {
                await loading.Show();
                
                await _refundAppService.UpdateRefundReviewAsync(rowData.Id, RefundReviewStatus.Proccessing);
                
                await _refundAppService.SendRefundRequestAsync(rowData.Id);
                
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
        private async Task RefundReject(RefundDto rowData)
        {
            try
            {
                await loading.Show();

                await _refundAppService.UpdateRefundReviewAsync(rowData.Id, RefundReviewStatus.ReturnedApplication);


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
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateItemList();
        }
        async Task OnSearch()
        {
            PageIndex = 0;
            await UpdateItemList();
        }
    }
}
