using Kooco.Pikachu.Members;
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

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Member = await MemberAppService.GetAsync(Id);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        await base.OnInitializedAsync();
    }

    private Task OnSelectedTabChanged(string name)
    {
        SelectedTab = name;

        return Task.CompletedTask;
    }
}