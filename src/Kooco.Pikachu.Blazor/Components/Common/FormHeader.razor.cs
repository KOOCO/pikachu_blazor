using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;

namespace Kooco.Pikachu.Blazor.Components;
public partial class FormHeader
{
    [Parameter] public string? Title { get; set; }
    [Parameter] public EventCallback<string> Filter { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> Search { get; set; }

    public async Task OnKeyboardAsync(KeyboardEventArgs e) =>
        await HandleEnterKeyAsync(e, async () =>
        await Search.InvokeAsync(null));
    static async Task HandleEnterKeyAsync(KeyboardEventArgs e, Func<Task> func)
    {
        if (e.Key == "Enter") await func();
    }
}