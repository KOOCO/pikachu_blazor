using Blazorise.DataGrid;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets;
public partial class TenantWalletList
{
    async Task OnSearchAsync()
    {
        CurrentPage = 1;
        await OnLoadDataAsync();
    }
    void OnFilterChanged(string value) => Entrance.SearchTerm = value;
    async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TenantWalletResultDto> e)
    {
        if (!IsInitialLoad) CurrentPage = e.Page;
        await OnLoadDataAsync();
        IsInitialLoad = false;
    }
    void OnRowClick(DataGridRowMouseEventArgs<TenantWalletResultDto> e) => NavigationManager.NavigateTo(
       segments: [nameof(TenantManagement), nameof(TenantWalletDetails)],
       previousPage: nameof(TenantWalletList),
       id: e.Item.Id,
       currentPage: CurrentPage);
    async Task OnLoadDataAsync()
    {
        try
        {
            await Loading.Show();
            var (totalCount, values) = await TenantWalletRepository.GetPagedAllAsync(
                skipCount: (CurrentPage - 1) * PageSize,
                maxResultCount: PageSize,
                searchTerm: Entrance.SearchTerm);

            List<TenantWalletResultDto> entities = [];
            foreach (var (tenant, wallet) in values)
            {
                entities.Add(new TenantWalletResultDto
                {
                    Id = wallet.Id,
                    TenantName = tenant.Name,
                    Balance = wallet.WalletBalance,
                    AwaitingReviewCount = 0,
                    CreationTime = wallet.CreationTime,
                });
            }

            Entities = entities;
            TotalCount = totalCount;

            await InvokeAsync(StateHasChanged);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
        finally
        {
            await Loading.Hide();
        }
    }
}