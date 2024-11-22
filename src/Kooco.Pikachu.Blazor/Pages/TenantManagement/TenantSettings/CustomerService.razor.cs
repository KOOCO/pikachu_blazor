using Blazorise;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class CustomerService
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }

    private CustomerServiceDto CustomerServiceDto { get; set; }
    private UpdateCustomerServiceDto Entity { get; set; }
    private Validations ValidationsRef { get; set; }

    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;

    public CustomerService()
    {
        Entity = new();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ResetAsync();
        }
    }

    private async Task UpdateAsync()
    {
        try
        {
            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;

                await AppService.UpdateCustomerServiceAsync(Entity);

                await Message.Success(L["CustomerServiceUpdated"]);

                await ResetAsync();

                IsLoading = false;
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

    async Task CancelAsync()
    {
        var confirmation = await Message.Confirm(L["AreYouSureToResetTheForm"]);
        if (confirmation)
        {
            IsCancelling = true;
            await ResetAsync();
            IsCancelling = false;
        }
    }

    private async Task ResetAsync()
    {
        try
        {
            CustomerServiceDto = await AppService.GetCustomerServiceAsync();
            Entity = ObjectMapper.Map<CustomerServiceDto, UpdateCustomerServiceDto>(CustomerServiceDto);
            ValidationsRef?.ClearAll();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}