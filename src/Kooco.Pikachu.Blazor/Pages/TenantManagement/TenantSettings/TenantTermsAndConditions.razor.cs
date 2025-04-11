using Blazored.TextEditor;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement.TenantSettings;

public partial class TenantTermsAndConditions
{
    [Parameter]
    public ITenantSettingsAppService AppService { get; set; }
    private bool IsLoading { get; set; } = false;
    private bool IsCancelling { get; set; } = false;
    private BlazoredTextEditor TermsAndConditionsHtml { get; set; }

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
            IsLoading = true;

            await AppService.UpdateTenantTermsAndConditionsAsync(await TermsAndConditionsHtml.GetHTML());

            await Message.Success(L["TermsAndConditionsUpdated"]);

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
            var termsAndConditions = await AppService.GetTenantTermsAndConditionsAsync();
            await TermsAndConditionsHtml.LoadHTMLContent(termsAndConditions);
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}