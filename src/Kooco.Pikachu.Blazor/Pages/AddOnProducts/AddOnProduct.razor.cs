using AutoMapper;
using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Members;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Blazor.Pages.AddOnProducts
{
    public partial class AddOnProduct
    {
        public List<AddOnProductDto> AddOnProductList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; }
        private int TotalCount { get; set; }

        private GetAddOnProductList Filters { get; set; }

        private AddOnProductDto SelectedAddOnProduct { get; set; }
        private List<AddOnProductDto> SelectedAddOnProducts { get; set; }
        public AddOnProduct()
        {
            AddOnProductList = [];
            Filters = new();
        }
        protected override async Task OnInitializedAsync()
        {
            await GetAddOnProductsAsync();
            await base.OnInitializedAsync();
        }
        private async Task GetAddOnProductsAsync()
        {
            try
            {

                Filters.MaxResultCount = PageSize;
                Filters.SkipCount = (CurrentPage - 1) * PageSize;
                Filters.Sorting = CurrentSorting;






             AddOnProductList.Add(new AddOnProductDto
                {
                    Id = 1,
                    ProductId = 101,
                    AddOnAmount = 10,
                    AddOnLimitPerOrder = 5,
                 ProductName = "Mother's Day Sale",
                 QuantitySetting = "Limited",
                    AvailableQuantity = 100,
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now.AddDays(10),
                    AddOnConditions = "Must purchase main product",
                    MinimumSpendAmount = 50,
                    GroupBuysScope = "GroupA",
                    Status=true,
                    SellingQuantity=10,
                    
                });
                AddOnProductList.Add(new AddOnProductDto
                {
                    Id = 2,
                    ProductId = 102,
                    AddOnAmount = 15,
                    AddOnLimitPerOrder = 3,
                    ProductName = "Mother's Day Sale",
                    QuantitySetting = "Unlimited",
                    AvailableQuantity = 500,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(15),
                    AddOnConditions = "Limited to one per customer",
                    MinimumSpendAmount = 100,
                    GroupBuysScope = "GroupB",
                    Status = true,
                    SellingQuantity = 10,
                });
                AddOnProductList.Add(new AddOnProductDto
                {
                    Id = 3,
                    ProductId = 103,
                    ProductName="Mother's Day Sale",
                    AddOnAmount = 20,
                    AddOnLimitPerOrder = 2,
                    QuantitySetting = "Limited",
                    AvailableQuantity = 200,
                    StartDate = DateTime.Now.AddDays(-3),
                    EndDate = DateTime.Now.AddDays(7),
                    AddOnConditions = "Available for VIP members only",
                    MinimumSpendAmount = 150,
                    GroupBuysScope = "GroupC",
                    Status = true,
                    SellingQuantity = 10,
                });
                AddOnProductList.Add(new AddOnProductDto
                {
                    Id = 4,
                    ProductId = 104,
                    AddOnAmount = 25,
                    ProductName = "Mother's Day Sale",
                    AddOnLimitPerOrder = 4,
                    QuantitySetting = "Unlimited",
                    AvailableQuantity = 300,
                    StartDate = DateTime.Now.AddDays(-8),
                    EndDate = DateTime.Now.AddDays(12),
                    AddOnConditions = "Valid for weekend purchases",
                    MinimumSpendAmount = 75,
                    GroupBuysScope = "GroupD",
                    Status = true,
                    SellingQuantity = 10,
                });
        

             
            
                TotalCount = 4;
            }
            catch (Exception ex)
            {

                await HandleErrorAsync(ex);
            }
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<AddOnProductDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;

            await GetAddOnProductsAsync();

            await InvokeAsync(StateHasChanged);
        }
        private static bool RowSelectableHandler(RowSelectableEventArgs<AddOnProductDto> rowSelectableEventArgs)
        => rowSelectableEventArgs.SelectReason is not DataGridSelectReason.RowClick;
       private  void OnAddOnProductStatusChanged(int id) 
        {
            return;
        }
    }
} 
