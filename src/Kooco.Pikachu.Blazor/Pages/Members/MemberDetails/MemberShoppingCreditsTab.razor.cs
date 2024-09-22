using Kooco.Pikachu.Members;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberDetails;

public partial class MemberShoppingCreditsTab
{
    [Parameter]
    public MemberDto? Member { get; set; }

    public bool CanCreateShoppingCredits { get; set; }

    protected override async Task OnInitializedAsync()
    {
        CanCreateShoppingCredits = await AuthorizationService.IsGrantedAsync(PikachuPermissions.UserShoppingCredits.Create);
        await base.OnInitializedAsync();
    }

    private void GrantShoppingCredits()
    {
        if (Member is null || !CanCreateShoppingCredits) return;
        NavigationManager.NavigateTo("Members/ShoppingCredits/Grant/" + Member.Id);
    }
}