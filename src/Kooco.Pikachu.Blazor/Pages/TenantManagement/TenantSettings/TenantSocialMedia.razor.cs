using Blazorise;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantSocialMedia
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }

    [Parameter]
    public bool ViewMode { get; set; } = false;

    private TenantSocialMediaDto TenantSocialMediaDto { get; set; }
    private UpdateTenantSocialMediaDto Entity { get; set; }
    private Validations ValidationsRef { get; set; }

    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;
    public TenantSocialMedia()
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
            if (ViewMode) return;
            if (await ValidationsRef.ValidateAll())
            {
                IsLoading = true;

                await AppService.UpdateTenantSocialMediaAsync(Entity);

                await Message.Success(L["SocialMediaUpdated"]);

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
            TenantSocialMediaDto = await AppService.GetTenantSocialMediaAsync();
            Entity = ObjectMapper.Map<TenantSocialMediaDto, UpdateTenantSocialMediaDto>(TenantSocialMediaDto);
            ValidationsRef?.ClearAll();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}