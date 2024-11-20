using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Kooco.Pikachu.Blazor.Pages.RedirectionPage;

public partial class RedirectionPage
{
    private readonly IJSRuntime _JSRuntime;

    public RedirectionPage(
        IJSRuntime JSRuntime    
    ) 
    {
        _JSRuntime = JSRuntime;
    }

    protected override Task OnInitializedAsync()
    {
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _JSRuntime.InvokeVoidAsync("promptToClose");
        }
    }
}
