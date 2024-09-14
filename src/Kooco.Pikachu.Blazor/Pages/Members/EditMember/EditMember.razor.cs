using Kooco.Pikachu.Members;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using Blazorise;

namespace Kooco.Pikachu.Blazor.Pages.Members.EditMember;

public partial class EditMember
{
    [Parameter]
    public Guid Id { get; set; }
    private MemberDto Member { get; set; }
    private UpdateMemberDto EditingMember { get; set; }

    private Validations ValidationsRef;

    public EditMember()
    {
        EditingMember = new();
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Member = await MemberAppService.GetAsync(Id);
            EditingMember = ObjectMapper.Map<MemberDto, UpdateMemberDto>(Member);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        await base.OnInitializedAsync();
    }

    private async Task UpdateAsync()
    {
        await Task.CompletedTask;
    }

    private void NavigateToMemberDetails()
    {
        NavigationManager.NavigateTo("/Members/Details/" + Member.Id);
    }
}