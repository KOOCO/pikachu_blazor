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
        public List<GroupBuyReportDto> GroupBuyReportList { get; set; } = new List<GroupBuyReportDto>();
        int PageIndex = 1;
        int PageSize = 10;
        int Total = 0;
        private string Sorting = nameof(Groupbuys.GroupBuyReport.GroupBuyName);
        private LoadingIndicator loading { get; set; } = new();
        string? StoreUrl { get; set; }

        protected override void OnInitialized()
        {
            StoreUrl = $"{_configuration["EntryUrl"]?.TrimEnd('/')}";
            base.OnInitialized();
        }

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
                await loading.Show();
                PageIndex = e.Page - 1;
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
                await loading.Hide();
            }
        }

        private void OnNavigate(Guid id)
        {
            NavigationManager.NavigateTo("/GroupBuyManagement/GroupBuyReportDetails/" + id);
        }
    }
}
