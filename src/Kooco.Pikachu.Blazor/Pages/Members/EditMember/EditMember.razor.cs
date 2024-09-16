using Kooco.Pikachu.Members;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using Blazorise;
using System.Collections.Generic;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.Members.EditMember;

public partial class EditMember
{
    [Parameter]
    public Guid Id { get; set; }
    private MemberDto Member { get; set; }
    private UpdateMemberDto EditingMember { get; set; }

    private Validations ValidationsRef;
    private bool IsUpdating { get; set; }

    private List<AddressModel> AddressList { get; set; }
    private AddressModel? SelectedAddress { get; set; }
    private Guid? SelectedAddressId { get; set; }

    public EditMember()
    {
        EditingMember = new();

        AddressList = [
            new AddressModel { City = "New York", Address = "47 W 13th St, New York, NY 10011, USA" },
            new AddressModel { City = "Taipei City 106409", Address = "No.55, Sec. 2, Jinshan S. Rd., Da-an District"}
        ];
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

    async Task OnSelectedValueChanged(Guid? value)
    {
        SelectedAddressId = value;
        SelectedAddress = AddressList.First(a => a.Id == value);

        EditingMember.Address = SelectedAddress.Address;
        EditingMember.City = SelectedAddress.City;
        await InvokeAsync(StateHasChanged);
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

// Temporary. Remove later when address is fetched from backend
public class AddressModel
{
    public Guid Id = Guid.NewGuid();
    public string Address { get; set; }
    public string City { get; set; }
}