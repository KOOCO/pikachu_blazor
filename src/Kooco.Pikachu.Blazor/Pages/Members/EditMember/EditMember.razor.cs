using Kooco.Pikachu.Members;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using Blazorise;
using System.Collections.Generic;
using System.Linq;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.Blazor.Pages.UserAddresses;

namespace Kooco.Pikachu.Blazor.Pages.Members.EditMember;

public partial class EditMember
{
    [Parameter]
    public Guid Id { get; set; }
    private MemberDto Member { get; set; }
    private UpdateMemberDto EditingMember { get; set; }
    private IReadOnlyList<UserAddressDto> UserAddresses { get; set; }

    private Validations ValidationsRef;
    private bool IsUpdating { get; set; }

    private UserAddressDto? SelectedAddress { get; set; }
    private Guid? SelectedAddressId { get; set; }
    private EditUserAddressModal EditUserAddressModal { get; set; }

    private bool IsFirstLoad { get; set; } = true;

    public EditMember()
    {
        EditingMember = new();
        UserAddresses = [];
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Member = await MemberAppService.GetAsync(Id);
            EditingMember = ObjectMapper.Map<MemberDto, UpdateMemberDto>(Member);
            await FetchUserAddresses();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        await base.OnInitializedAsync();
    }

    async Task OnSelectedValueChanged(Guid? value)
    {
        SelectedAddressId = value;
        SelectedAddress = UserAddresses.FirstOrDefault(a => a.Id == value);

        await SetSelectedAddress();
    }

    async Task SetSelectedAddress()
    {
        EditingMember.DefaultAddressId = SelectedAddress?.Id;
        EditingMember.Street = SelectedAddress?.Street;
        EditingMember.City = SelectedAddress?.City;
        await InvokeAsync(StateHasChanged);
    }

    void EditAddress()
    {
        if (!SelectedAddressId.HasValue || SelectedAddress == null) return;
        var editingUserAddress = ObjectMapper.Map<UserAddressDto, UpdateUserAddressDto>(SelectedAddress);
        EditUserAddressModal.OpenModal(SelectedAddressId.Value, editingUserAddress);
    }

    async Task FetchUserAddresses()
    {
        try
        {
            var data = await UserAddressAppService.GetListAsync(new GetUserAddressListDto
            {
                UserId = Id,
                MaxResultCount = 1000
            });
            UserAddresses = data.Items;

            if (IsFirstLoad)
            {
                var defaultAddress = UserAddresses.FirstOrDefault(a => a.IsDefault);
                SelectedAddressId = defaultAddress?.Id;
                SelectedAddress = defaultAddress;
                await SetSelectedAddress();
                IsFirstLoad = false;
            }
            await OnSelectedValueChanged(SelectedAddressId);
            return;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    async Task OnEditUserAddressModalClosed(bool value)
    {
        await FetchUserAddresses();
    }

    private async Task UpdateAsync()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                IsUpdating = true;
                await MemberAppService.UpdateAsync(Id, EditingMember);
                IsUpdating = false;
                NavigateToMemberDetails();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void NavigateToMemberDetails()
    {
        NavigationManager.NavigateTo("/Members/Details/" + Member.Id);
    }
}