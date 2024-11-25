using Blazorise;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantGoogleTagManager
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }

    private TenantGoogleTagManagerDto TenantGoogleTagManagerDto { get; set; }
    private UpdateTenantGoogleTagManagerDto Entity { get; set; }
    private Validations ValidationsRef { get; set; }

    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;

    public TenantGoogleTagManager()
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
                if (Entity.GtmEnabled && Entity.GtmContainerId.IsNullOrWhiteSpace())
                {
                    return;
                }

                IsLoading = true;

                await AppService.UpdateTenantGoogleTagManagerAsync(Entity);

                await Message.Success(L["GoogleTagManagerUpdated"]);

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
            TenantGoogleTagManagerDto = await AppService.GetTenantGoogleTagManagerAsync();
            Entity = ObjectMapper.Map<TenantGoogleTagManagerDto, UpdateTenantGoogleTagManagerDto>(TenantGoogleTagManagerDto);
            ValidationsRef?.ClearAll();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}