using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using Volo.Abp.TenantManagement;
using System.Linq;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement
{
    public partial class BannerTableColumn
    {
        [Parameter]
        public object Data { get; set; }
        public string BannerUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            var tenant = Data.As<TenantDto>();
            BannerUrl = (string)tenant.ExtraProperties.Where(x => x.Key == "BannerUrl").Select(x => x.Value).FirstOrDefault();

        }
    }
}
