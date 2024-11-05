using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeFinancials;
using Microsoft.AspNetCore.Components;
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

    private UserCumulativeFinancialDto? CumulativeFinancials { get; set; }
    decimal PaidAmount = 0;
    decimal UnpaidAmount = 0;
    decimal RefundedAmount = 0;

    bool IsDeleting = false;

    private UserAddressDto? DefaultAddress { get; set; }

    protected async override Task OnParametersSetAsync()
    {
        try
        {
            if (Member != null)
            {
                DefaultAddress = await MemberAppService.GetDefaultAddressAsync(Member.Id);
                CumulativeFinancials = await MemberAppService.GetMemberCumulativeFinancialsAsync(Member.Id);
                (PaidAmount,UnpaidAmount,RefundedAmount)=await OrderAppService.GetOrderStatusAmountsAsync(Member.Id);
                StateHasChanged();
            }
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