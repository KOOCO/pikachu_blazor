using Blazorise.DataGrid;
using Kooco.Pikachu.TenantManagement;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets;
public partial class TenantWallet
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
       previousPage: nameof(TenantWallet),
       id: e.Item.Id,
       currentPage: CurrentPage);
    async Task OnLoadDataAsync()
    {
        try
        {
            await Loading.Show();

            Entities =
            [
                new TenantWalletResultDto
                {
                    Id = Guid.NewGuid(),
                    TenantName = "Test",
                    Balance = 1000,
                    AwaitingReviewCount = 1,
                    CreationTime = DateTime.Now,
                },
            ];

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