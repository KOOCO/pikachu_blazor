using Kooco.Pikachu.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Notifications;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Blazor.Helpers;

public class CopyService(IJSRuntime JSRuntime, IUiNotificationService UiNotificationService, IStringLocalizer<PikachuResource> L) : ITransientDependency
{
    public async Task Copy(object? text)
    {
        try
        {
            var _text = text?.ToString();
            if (!string.IsNullOrWhiteSpace(_text))
            {
                await JSRuntime.InvokeVoidAsync("clipboard.copy", _text);
                await UiNotificationService.Success(L["TextCopied", _text]);
            }
        }
        catch
        {
            await UiNotificationService.Error(L["UnableToCopyText"]);
        }
    }
}
