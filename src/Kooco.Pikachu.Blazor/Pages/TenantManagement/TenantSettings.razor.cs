using Blazorise;
using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.TenantManagement;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement;

public partial class TenantSettings
{
    private TenantSettingsDto TenantSettingsDto { get; set; }
    private UpdateTenantSettingsDto Entity { get; set; }
    private bool CanEditTenantSettings { get; set; }
    private Validations ValidationsRef { get; set; }

    private bool EditingMode { get; set; } = false;
    private bool IsLoading { get; set; } = false;

    protected override async Task OnInitializedAsync()
    {
        CanEditTenantSettings = await AuthorizationService.IsGrantedAsync(PikachuPermissions.TenantSettings.Edit);
        await ResetAsync();
    }

    private async Task UpdateAsync() { }

    private async Task ResetAsync()
    {
        try
        {
            //TenantSettingsDto = await TenantSettingsAppService.FirstOrDefaultAsync();
            Entity = ObjectMapper.Map<TenantSettingsDto, UpdateTenantSettingsDto>(TenantSettingsDto);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}