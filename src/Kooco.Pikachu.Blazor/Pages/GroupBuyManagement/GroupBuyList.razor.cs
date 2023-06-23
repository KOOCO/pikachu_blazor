using System;
using System.Linq;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class GroupBuyList
    {
        public string TestProperty { get; set; } = string.Empty;
        public string TenantNameList { get; set; } = string.Empty;

        public GroupBuyList(ITenantAppService tenantAppService) 
        {
            TestProperty = "此為團購清單頁面";
            GetTenantsInput input = new GetTenantsInput
            {

            };
            var data = tenantAppService.GetListAsync(input).Result;
            TenantNameList = string.Join(",", data.Items.Select(t => t.Name).ToList());
        }
    }
}
