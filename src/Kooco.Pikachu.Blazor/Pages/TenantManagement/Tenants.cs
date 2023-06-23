using Blazorise.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.TenantManagement
{
    public class TenantViewModel
    {
        //商戶名稱
        public string Name { get; set; } = string.Empty;
        //聯絡Email
        public string Email { get; set; } = string.Empty;
        //帳號狀態
        public string IsActive { get; set; } = string.Empty;
        //抽成%數
        //起始時間
        //結束時間
    }

    public partial class Tenants
    {
        public List<TenantViewModel> TenantList { get; private set; } = new List<TenantViewModel>();

        public Tenants(ITenantAppService tenantAppService, IIdentityUserAppService identityUserAppService)
        {
            GetTenantsInput input = new GetTenantsInput();
            var tenantData = tenantAppService.GetListAsync(input).Result;
            GetIdentityUsersInput usersInput = new GetIdentityUsersInput();
            var users = identityUserAppService.GetListAsync(usersInput).Result.Items;
            foreach (var item in tenantData.Items)
            {
                //var user = users.Where(u => u.TenantId == item.Id && u.UserName == "admin").FirstOrDefault();
                TenantViewModel viewModel = new TenantViewModel
                {
                    Name = item.Name,
                    //Email = user.Email,
                    Email = string.Empty,
                    IsActive = "已啟用"
                };
                TenantList.Add(viewModel);
            }
        }
    }
}
