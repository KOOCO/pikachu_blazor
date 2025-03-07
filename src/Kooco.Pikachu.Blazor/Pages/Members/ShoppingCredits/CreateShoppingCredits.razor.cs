using Blazorise;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.Members.ShoppingCredits;

public partial class CreateShoppingCredits
{
    [Parameter]
    public Guid MemberId { get; set; }
    private MemberDto? Member { get; set; }
    private CreateUserShoppingCreditDto NewUserShoppingCredit { get; set; }

    private Validations ValidationsRef;
    private bool IsLoading { get; set; }

    public CreateShoppingCredits()
    {
        NewUserShoppingCredit = new()
        {
            ShoppingCreditType = UserShoppingCreditType.Grant
        };
    }

    protected override void OnParametersSet()
    {
        if (NewUserShoppingCredit != null)
        {
            NewUserShoppingCredit.UserId = MemberId;
        }
        base.OnParametersSet();
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Member = await MemberAppService.GetAsync(MemberId);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        await base.OnInitializedAsync();
    }

    private async Task CreateAsync()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;
                await UserShoppingCreditAppService.CreateAsync(NewUserShoppingCredit);
                IsLoading = false;
                NavigateToMemberDetails();
            }
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    private void NavigateToMemberDetails()
    {
        NavigationManager.NavigateTo("/Members/Details/" + Member.Id);
    }
}