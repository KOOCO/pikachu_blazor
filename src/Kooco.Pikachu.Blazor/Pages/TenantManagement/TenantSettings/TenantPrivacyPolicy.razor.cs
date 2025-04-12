using Blazored.TextEditor;
using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.Tenants;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantPrivacyPolicy
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }
    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;
    private BlazoredTextEditor PrivacyPolicyHtml { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ResetAsync();
        }
    }

    async Task SetDefaultPrivacyPolicy()
    {
        await PrivacyPolicyHtml.LoadHTMLContent(TenantSettingsConsts.DefaultPrivacyPolicy);
    }

    private async Task UpdateAsync()
    {
        try
        {
            IsLoading = true;

            await AppService.UpdateTenantPrivacyPolicyAsync(await PrivacyPolicyHtml.GetHTML());

            await Message.Success(L["PrivacyPolicyUpdated"]);

            await ResetAsync();

            IsLoading = false;
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
            var privacyPolicy = await AppService.GetTenantPrivacyPolicyAsync();
            await PrivacyPolicyHtml.LoadHTMLContent(privacyPolicy);
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}