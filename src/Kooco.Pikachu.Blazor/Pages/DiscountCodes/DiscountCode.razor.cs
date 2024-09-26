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

namespace Kooco.Pikachu.Blazor.Pages.DiscountCodes
{
    public partial class DiscountCode
    {
        public List<DiscountCodeDto> DiscountCodeList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }

        private GetDiscountCodeList Filters { get; set; }

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

                Filters.MaxResultCount = PageSize;
                Filters.SkipCount = (CurrentPage - 1) * PageSize;
                Filters.Sorting = CurrentSorting;






                // Adding dummy data one by one
                var discountCode1 = new DiscountCodeDto
                {
                    Id = Guid.NewGuid(),
                    EventName = "Holiday Sale",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(7),
                    DiscountCodeValue = "HOLIDAY2024",
                    SpecifiedCode = "HOLIDAY",
                    AvailableQuantity = 10,
                    TotalQuantity=90,
                    MaxUsePerPerson = 1,
                    GroupBuysScope = "All",
                    ProductsScope = "All",
                    DiscountMethod = "Percentage",
                    MinimumSpendAmount = 50,
                    ShippingDiscountScope = "Free",
                    DiscountPercentage = 20,
                    DiscountAmount = 0,
                    Status=true
                };
                DiscountCodeList.Add(discountCode1);

                var discountCode2 = new DiscountCodeDto
                {
                    Id = Guid.NewGuid(),
                    EventName = "New Year Special",
                    StartDate = DateTime.Now.AddDays(10),
                    EndDate = DateTime.Now.AddDays(17),
                    DiscountCodeValue = "NEWYEAR2024",
                    SpecifiedCode = "NEWYEAR",
                    AvailableQuantity = 50,
                    MaxUsePerPerson = 2,
                    GroupBuysScope = "VIP Members",
                    ProductsScope = "Selected Items",
                    DiscountMethod = "Amount",
                    MinimumSpendAmount = 100,
                    ShippingDiscountScope = "Standard",
                    DiscountPercentage = 0,
                    DiscountAmount = 25,
                    TotalQuantity=80,
                    Status=true,
                };
                DiscountCodeList.Add(discountCode2);




                TotalCount = 4;
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

        void CreateNewDiscountCode()
        {
            NavigationManager.NavigateTo("/new-discount-code");


        }
    }
}
