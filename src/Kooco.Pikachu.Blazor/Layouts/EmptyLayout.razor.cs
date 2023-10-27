using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web;

namespace Kooco.Pikachu.Blazor.Layouts
{
    public partial class EmptyLayout
    {
        [Inject]
        protected IAbpUtilsService UtilsService { get; set; }

        [Inject]
        IJSRuntime JSRuntime { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
              

            }
        }

      
    }
}
