using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;

namespace Kooco.Pikachu.Blazor.Pages.InboxManagement.Toolbar;

public class InboxToolbarContributor : IToolbarContributor
{
    public Task ConfigureToolbarAsync(IToolbarConfigurationContext context)
    {
        if (context.Toolbar.Name == StandardToolbars.Main)
        {
            context.Toolbar.Items.Insert(0, new ToolbarItem(typeof(InboxToolbarComponent)));
        }

        return Task.CompletedTask;
    }
}
