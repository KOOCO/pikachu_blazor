using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using System;
using Volo.Abp.AspNetCore.Components.Web.LeptonXLiteTheme.Themes.LeptonXLite;
using Volo.Abp.DependencyInjection;
using Volo.Abp.AspNetCore.Components.Web.LeptonXLiteTheme.Themes.LeptonXLite.Navigation;

namespace Kooco.Pikachu.Blazor.Pages.MainMenu
{
    [ExposeServices(typeof(MainMenuItem))]
   
    public partial class MyMainMenuItem 
    {
        

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        protected virtual void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            ActivateCurrentPage();
        }

        protected virtual void ActivateCurrentPage()
        {
            if (MenuItem.MenuItem.Url.IsNullOrEmpty())
            {
                return;
            }

            if (PageLayout.MenuItemName.IsNullOrEmpty())
            {
                string text = MenuItem.MenuItem.Url.Replace("~/", string.Empty).Trim('/');
                string value = new Uri(NavigationManager.Uri.TrimEnd('/')).AbsolutePath.Trim('/');
                if (text.TrimEnd('/').Equals(value, StringComparison.InvariantCultureIgnoreCase))
                {
                    Menu.Activate(MenuItem);
                }
            }

            if (PageLayout.MenuItemName == MenuItem.MenuItem.Name)
            {
                Menu.Activate(MenuItem);
            }
        }

        protected virtual void ToggleMenu()
        {
            Menu.ToggleOpen(MenuItem);
        }

        public virtual void Dispose()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
        }

        
    }
}
