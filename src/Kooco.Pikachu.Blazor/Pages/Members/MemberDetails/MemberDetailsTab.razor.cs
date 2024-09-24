using Kooco.Pikachu.Members;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Members.MemberDetails;

public partial class MemberDetailsTab
{
    [Parameter]
    public MemberDto? Member { get; set; }

    [Parameter]
    public EventCallback<MemberDto> OnDelete { get; set; }

    [Parameter]
    public bool CanDeleteMember { get; set; }

    [Parameter]
    public MemberOrderStatsDto? OrderStats { get; set; }

    bool IsDeleting = false;

    private UserAddressDto? DefaultAddress { get; set; }

    public MemberDetailsTab()
    {
        Member = new();
        OrderStats = new();
        DefaultAddress = new();
    }

    protected async override Task OnInitializedAsync()
    {
        try
        {
            if (Member == null)
            {
                Logger.Log(LogLevel.Debug, "Member is null in OnAfterRenderAsync");
                return;
            }

            DefaultAddress = await MemberAppService.GetDefaultAddressAsync(Member.Id) ?? new();

            if (DefaultAddress == null)
            {
                Logger.Log(LogLevel.Debug, "DefaultAddress is null after API call");
            }

            StateHasChanged();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteAsync()
    {
        try
        {
            if (!CanDeleteMember) return;

            var confirmation = await Message.Confirm(L["AreYouSureToDeleteThisMember"]);
            if (confirmation)
            {
                IsDeleting = true;
                StateHasChanged();
                await MemberAppService.DeleteAsync(Member.Id);
                await OnDelete.InvokeAsync(Member);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsDeleting = false;
            StateHasChanged();
        }
    }
}