using Kooco.Pikachu.Members;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberDetails;

public partial class MemberDetails
{
    [Parameter]
    public Guid Id { get; set; }
    private MemberDto Member { get; set; }

    string SelectedTab = "Details";

    private bool CanEditMember { get; set; }
    private bool CanDeleteMember { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetMemberAsync();
    }

    private async Task SetPermissionsAsync()
    {
        CanEditMember = await AuthorizationService.IsGrantedAnyAsync(PikachuPermissions.Members.Edit);
        CanDeleteMember = await AuthorizationService.IsGrantedAnyAsync(PikachuPermissions.Members.Delete);
    }

    private async Task GetMemberAsync()
    {
        try
        {
            Member = await MemberAppService.GetAsync(Id);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private Task OnSelectedTabChanged(string name)
    {
        SelectedTab = name;

        return Task.CompletedTask;
    }

    private void NavigateToMember()
    {
        NavigationManager.NavigateTo("/Members");
    }
}