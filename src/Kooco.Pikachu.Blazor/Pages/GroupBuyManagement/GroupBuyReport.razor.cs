using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Messages;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class GroupBuyReport
    {
        public List<OrderReportDto> GroupBuyReportList { get; set; }
        public bool IsAllSelected { get; private set; } = false;

        private readonly IUiMessageService _uiMessageService;

        private readonly IOrderAppService _orderAppService;
        int _pageIndex = 1;
        int _pageSize = 10;
        int Total = 0;
        private string Sorting = nameof(OrderReport.GroupBuyName);
        private LoadingIndicator loading { get; set; } = new();

        public GroupBuyReport(
            IOrderAppService orderAppService,
            IUiMessageService messageService
            )
        {
            _orderAppService = orderAppService;
            _uiMessageService = messageService;
            GroupBuyReportList = new List<OrderReportDto>();
        }
        private async Task UpdateGroupBuyReport()
        {
            try
            {
                int skipCount = _pageIndex * _pageSize;
                var result = await _orderAppService.GetOrderReportAsync(new OrderReportDto
                {
                    Sorting = Sorting,
                    MaxResultCount = _pageSize,
                    SkipCount = skipCount
                });
                GroupBuyReportList = result.Items.ToList();
                Total = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrderReportDto> e)
        {
            try
            {
                await loading.Show();
                _pageIndex = e.Page - 1;
                await UpdateGroupBuyReport();
                await InvokeAsync(StateHasChanged);
                await loading.Hide();
            }
            catch (Exception ex)
            {
                await loading.Hide();
                Console.WriteLine(ex.ToString());
            }
        }
    }
   
}
