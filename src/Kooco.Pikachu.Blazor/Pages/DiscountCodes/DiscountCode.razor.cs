using Blazorise.DataGrid;
using Kooco.Pikachu.AddOnProducts;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Kooco.Pikachu.DiscountCodes;
using System.Linq;
using Blazorise;
using AntDesign;
using SortDirection = Blazorise.SortDirection;

namespace Kooco.Pikachu.Blazor.Pages.DiscountCodes
{
    public partial class DiscountCode
    {
        public IReadOnlyList<DiscountCodeDto> DiscountCodeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }

        private GetDiscountCodeListDto Filters { get; set; }

        private Dictionary<string, string> _gradients = new()
    {
        { "0%", "#fc3030" },
        { "100%", "#87d068" }
    };

        public DiscountCode()
        {
            DiscountCodeList = [];
            Filters = new();
        }
        protected override async Task OnInitializedAsync()
        {
            await GetDiscountCodesAsync();
            await base.OnInitializedAsync();
        }
        private async Task GetDiscountCodesAsync()
        {
            try
            {
                var result=await DiscountCodeAppService.GetListAsync(new GetDiscountCodeListDto
                    {
                    MaxResultCount = PageSize,
                    SkipCount = (CurrentPage - 1) * PageSize,
                    Sorting = CurrentSorting,
                    Filter=Filters.Filter,
                    StartDate=Filters.StartDate,
                    EndDate=Filters.EndDate,
                });

                DiscountCodeList = result.Items;
                TotalCount = (int)result.TotalCount;



            
            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex);
            }
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<DiscountCodeDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;

            await GetDiscountCodesAsync();

            await InvokeAsync(StateHasChanged);
        }
        private static bool RowSelectableHandler(RowSelectableEventArgs<DiscountCodeDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
        private void OnDiscountCodeStatusChanged(Guid id)
        {
            return;
        }
        async void DeleteAddOn(Guid id)
        {
            await DiscountCodeAppService.DeleteAsync(id);
            await _message.Success("DiscountCodeDeletedSucessfully");
            await GetDiscountCodesAsync();
            await InvokeAsync(StateHasChanged);
        }
        
       void NavigateToEditPage(Guid id)
        {
            NavigationManager.NavigateTo("/new-discount-code/" + id);

        }
        void CreateNewDiscountCode()
        {
            NavigationManager.NavigateTo("/new-discount-code");


        }
    }
}
