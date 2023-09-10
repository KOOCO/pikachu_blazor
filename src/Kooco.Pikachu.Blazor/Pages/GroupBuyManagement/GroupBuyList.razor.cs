using Kooco.Pikachu.GroupBuys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class GroupBuyList
    {
        public string TestProperty { get; set; } = string.Empty;
        public string TenantNameList { get; set; } = string.Empty;
        public List<GroupBuyDto> GroupBuyListItem { get; set; }
        private readonly IGroupBuyAppService _groupBuyAppService;
        int _pageIndex = 1;
        int _pageSize = 10;
        int Total = 0;
        public GroupBuyList(ITenantAppService tenantAppService, IGroupBuyAppService groupBuyAppService) 
        {
            TestProperty = "此為團購清單頁面";
            GetTenantsInput input = new GetTenantsInput
            {

            };
            //var data = tenantAppService.GetListAsync(input).Result;
            //TenantNameList = string.Join(",", data.Items.Select(t => t.Name).ToList());
            _groupBuyAppService = groupBuyAppService;
            GroupBuyListItem=new List<GroupBuyDto>();
        }
        protected override async Task OnInitializedAsync()
        {
            await UpdateGroupBuyList();
        }

        private async Task UpdateGroupBuyList()
        {
            int skipCount = (_pageIndex - 1) * _pageSize;
            var result = await _groupBuyAppService.GetListAsync(new GetGroupBuyInput
            {
                MaxResultCount = _pageSize,
                SkipCount = skipCount
            });
            GroupBuyListItem = result.Items.ToList();
            Total = (int)result.TotalCount;
        }

    }
}
