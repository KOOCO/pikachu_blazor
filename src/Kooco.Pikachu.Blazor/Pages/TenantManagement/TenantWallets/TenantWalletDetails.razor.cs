using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Tenants.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets;
public partial class TenantWalletDetails
{
    bool CanEdit;
    int UpperPage = default;
    string SelectedTab = nameof(ManageWallet);
    void OnNavigateBack() => NavigationManager.NavigateTo(
        segments: [nameof(TenantManagement), PreviousPage],
        currentPage: UpperPage);
    void OnSelectedTab(string name) => SelectedTab = name;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (NavigationManager.TryGetPage(out var page)) UpperPage = page;
            CanEdit = await AuthorizationService.IsGrantedAnyAsync(PikachuPermissions.TenantWallet.Edit);
            var (tenant, wallet) = await TenantWalletRepository.FindTenantAndWalletAsync(Id);
            Tenant = tenant;
            Wallet = wallet;
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }

    public Tenant? Tenant { get; set; }
    public TenantWallet? Wallet { get; set; }
}