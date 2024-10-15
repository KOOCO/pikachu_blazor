using Blazorise;
using Kooco.Pikachu.LoginConfigurations;
using System;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.LoginConfigurations;

public partial class LoginConfigurations
{
    private UpdateLoginConfigurationDto Entity { get; set; }

    private Validations ValidationsRef;

    private bool IsLoading { get; set; }
    private bool IsCancelling { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ResetAsync();
        }
    }

    async Task UpdateAsync()
    {
        try
        {
            var validate = await ValidationsRef.ValidateAll();
            if (!validate) return;

            IsLoading = true;
            await LoginConfigurationAppService.UpdateAsync(Entity);
            IsLoading = false;
        }
        catch (Exception ex)
        {
            IsLoading = false;
            await HandleErrorAsync(ex);
        }
    }

    async Task ResetAsync()
    {
        try
        {
            IsCancelling = true;
            var loginConfigurations = await LoginConfigurationAppService.FirstOrDefaultAsync();
            Entity = ObjectMapper.Map<LoginConfigurationDto, UpdateLoginConfigurationDto>(loginConfigurations);
            Entity ??= new();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsCancelling = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}