using Blazorise.DataGrid;
using Blazorise;
using Blazorise.LoadingIndicator;
using Kooco.Pikachu.AutomaticEmails;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.JSInterop;

namespace Kooco.Pikachu.Blazor.Pages.TenantEmailing
{
    public partial class AutomaticEmailing
    {
        List<AutomaticEmailDto> Items { get; set; }
        int _pageIndex = 1;
        int _pageSize = 10;
        int Total = 0;
        private string Sorting = nameof(AutomaticEmail.StartDate);
        private bool Loading { get; set; } = true;

        public AutomaticEmailing()
        {
            Items = new();
        }

        private async Task UpdateGroupBuyList()
        {
            try
            {
                Loading=true;
                int skipCount = _pageIndex * _pageSize;
                var result = await _automaticEmailAppService.GetListAsync(new GetAutomaticEmailListDto
                {
                    Sorting = Sorting,
                    MaxResultCount = _pageSize,
                    SkipCount = skipCount
                });
                Items = result.Items.ToList();
                Total = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
            finally
            {
               Loading=false;
            }
        }


        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AutomaticEmailDto> e)
        {
            try
            {
                _pageIndex = e.Page - 1;
                await UpdateGroupBuyList();
                await InvokeAsync(StateHasChanged);
               Loading=false;
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
                await JSRuntime.InvokeVoidAsync("console.error", ex.ToString());
            }
        }

        public void OnEditItem(DataGridRowMouseEventArgs<AutomaticEmailDto> e)
        {
            var id = e.Item.Id;
            NavigationManager.NavigateTo("/EditAutomaticEmails/" + id);
        }

        async void OnSortChange(DataGridSortChangedEventArgs e)
        {
            Sorting = e.FieldName + " " + (e.SortDirection != SortDirection.Default ? e.SortDirection : "");
            await UpdateGroupBuyList();
        }

        void CreateNew()
        {
            NavigationManager.NavigateTo("/AutomaticEmailing/Create");
        }
    }
}
