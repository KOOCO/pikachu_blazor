using Blazorise.DataGrid;
using Blazorise;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsFeeManagements;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using Volo.Abp.Content;
using Kooco.Pikachu.TenantDeliveryFees;

namespace Kooco.Pikachu.Blazor.Pages.TenantDeliveryFees
{
    public partial class TenantDeliveryFee
    {
        // Properties
        private IReadOnlyList<TenantLogisticsFeeRowDto> TenantList = new List<TenantLogisticsFeeRowDto>();
        private int TotalCount = 0;
        private int CurrentPage = 1;
        private int PageSize = 10;
        private string? Filter = "";
        private string? StatusFilter = "";


        // Breadcrumb
        protected List<BreadcrumbItem> BreadcrumbItems = new();

        protected override async Task OnInitializedAsync()
        {
            await SetBreadcrumbItemsAsync();
            await LoadData();
        }

        private async Task SetBreadcrumbItemsAsync()
        {

            //BreadcrumbItems.Add(new BreadcrumbItem(L["LogisticsManagement"]));
            //BreadcrumbItems.Add(new BreadcrumbItem(L["FeeManagement"], "/logistics-management", true));
        }

        private async Task LoadData()
        {
            try
            {
                var result = await TenantDeliveryFeeAppService.GetTenantLogisticsFeeOverviewAsync(new TenantLogisticsFeeGetListInput
                {
                    Filter = Filter,
                    SkipCount = (CurrentPage - 1) * PageSize,

                    MaxResultCount = PageSize
                });

                TenantList = result.Items;
                TotalCount = (int)result.TotalCount;
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task OnDataGridReadData(DataGridReadDataEventArgs<TenantLogisticsFeeRowDto> e)
        {
            CurrentPage = e.Page;
            PageSize = e.PageSize;
            await LoadData();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OnSearchKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                CurrentPage = 1;
                await LoadData();
            }
        }

        private void ViewTenantDetails(Guid tenantId)
        {
            NavigationManager.NavigateTo($"/tenant-fee-settings/{tenantId}");
        }



        // Helper methods


        private string GetStatusColor(bool status)
        {
            var color = status switch
            {

                true => "success",

                false => "danger",

            };

            return $"pk-badge-{color}";
        }
        private string GetStatusText(bool status)
        {
            return status switch
            {
                false => "PlatformInactive",
                true => "PlatformActive",

            };
        }

    }
}


