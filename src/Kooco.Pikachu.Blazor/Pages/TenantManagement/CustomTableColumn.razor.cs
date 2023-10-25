using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement
{
    public partial class CustomTableColumn
    {
        [Parameter]
        public object Data { get; set; }
        public string LogoUrl { get; set; }

        protected override async Task OnInitializedAsync() {
            base.OnInitializedAsync();
            var tenant = Data.As<TenantDto>();
            LogoUrl = (string)tenant.ExtraProperties.Where(x => x.Key == "LogoUrl").Select(x=>x.Value).FirstOrDefault();
        
        }

    }
}
