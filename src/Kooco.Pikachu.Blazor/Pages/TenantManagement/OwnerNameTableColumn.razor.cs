using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System;
using Volo.Abp.TenantManagement;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement
{
    public partial class OwnerNameTableColumn
    {
        [Parameter]
        public object Data { get; set; }
        public string OwnerName { get; set; }
        private readonly IIdentityUserAppService _identityUserAppService;
        public OwnerNameTableColumn(IIdentityUserAppService identityUserAppService) { 
        _identityUserAppService = identityUserAppService;
        
        
        }

        protected override async Task OnInitializedAsync()
        {
            base.OnInitializedAsync();
            var tenant = Data.As<TenantDto>();
            if (tenant.HasProperty("TenantOwner"))
            {
                var userid = tenant.GetProperty<Guid>("TenantOwner");
                if (userid != Guid.Empty)
                {
                    var user = await _identityUserAppService.GetAsync(userid);
                    OwnerName = user.Name;
                }
            }

        }
    }
}
