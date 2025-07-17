using Blazorise;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.GroupBuys;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class GroupBuyReport
    {
        public List<GroupBuyReportDto> GroupBuyReportList { get; set; } = [];
        int PageIndex = 1;
        readonly int PageSize = 10;
        int Total = 0;
        private string Sorting = nameof(Groupbuys.GroupBuyReport.GroupBuyName);
        private bool Loading { get; set; } = true;
        private string? ReportBaseUrl { get; set; }

        private async Task UpdateGroupBuyReport()
        {
            int skipCount = PageIndex * PageSize;
            var result = await _groupBuyAppService.GetGroupBuyReportListAsync(new GetGroupBuyReportListDto
            {
                Sorting = Sorting,
                MaxResultCount = PageSize,
                SkipCount = skipCount
            });
            GroupBuyReportList = result.Items.ToList();
            Total = (int)result.TotalCount;
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<GroupBuyReportDto> e)
        {
            try
            {
               Loading=true;
                PageIndex = e.Page - 1;
                await UpdateGroupBuyReport();
                ReportBaseUrl = Configuration["App:GroupBuyReportUrl"]?.EnsureEndsWith('/');
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                Loading=false;
            }
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            try
            {
               Loading=true;
                Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
                await UpdateGroupBuyReport();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
                Loading=false;
            }
        }
    }
}
