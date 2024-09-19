using Blazorise;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.UserAddresses;

public partial class EditUserAddressModal
{
    public Guid EditingUserAddressId { get; set; }
    public UpdateUserAddressDto EditingUserAddress { get; set; }

    private bool IsLoading { get; set; } = false;

    private Modal Modal;

    private Validations ValidationsRef;

    [Parameter]
    public EventCallback<bool> OnModalClosed { get; set; }

    public EditUserAddressModal()
    {
        EditingUserAddress = new();
        ValidationsRef?.ClearAll();
    }

    public void OpenModal(Guid editingUserAddressId, UpdateUserAddressDto editingUserAddress)
    {
        EditingUserAddress = editingUserAddress;
        EditingUserAddressId = editingUserAddressId;
        Modal.Show();
    }

    public void CloseModal()
    {
        Modal.Hide();
        if (OnModalClosed.HasDelegate)
        {
            OnModalClosed.InvokeAsync(false);
        }
    }

    private async Task UpdateAsync()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;
                await UserAddressAppService.UpdateAsync(EditingUserAddressId, EditingUserAddress);
                CloseModal();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsLoading = false;
        }
    }
}