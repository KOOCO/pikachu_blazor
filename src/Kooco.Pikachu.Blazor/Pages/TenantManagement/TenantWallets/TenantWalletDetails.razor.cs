using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantWallets;
public partial class TenantWalletDetails
{
    bool CanEdit;
    int UpperPage = default;
    string SelectedTab = nameof(WalletManageWallet);
    void OnNavigateBack() => NavigationManager.NavigateTo(
        segments: [nameof(TenantManagement), PreviousPage],
        currentPage: UpperPage);
    void OnSelectedTab(string name) => SelectedTab = name;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (NavigationManager.TryGetPage(out var page)) UpperPage = page;
            // BackendMemberItemDto = await MemberRepository.GetMemberByIdAsync(Id);

            CanEdit = await AuthorizationService.IsGrantedAnyAsync(PikachuPermissions.TenantWallet.Edit);
        }
        catch (Exception e)
        {
            await HandleErrorAsync(e);
        }
    }
    public TenantWalletResultDto TenantWallet { get; set; } = new TenantWalletResultDto
    {
        Id = Guid.NewGuid(),
        TenantName = "Test",
        Balance = 1000,
        AwaitingReviewCount = 1,
        CreationTime = DateTime.Now,
    };
}