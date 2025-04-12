using Blazorise;
using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantInformation
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }

    private TenantInformationDto TenantInformationDto { get; set; }
    private UpdateTenantInformationDto Entity { get; set; }
    private Validations ValidationsRef { get; set; }

    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;

    public TenantInformation()
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

                await AppService.UpdateTenantInformationAsync(Entity);

                await Message.Success(L["TenantInformationUpdated"]);

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
            TenantInformationDto = await AppService.GetTenantInformationAsync();
            Entity = ObjectMapper.Map<TenantInformationDto, UpdateTenantInformationDto>(TenantInformationDto);
            ValidationsRef?.ClearAll();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}