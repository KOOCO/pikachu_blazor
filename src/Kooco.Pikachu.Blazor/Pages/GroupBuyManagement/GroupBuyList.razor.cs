using Blazorise;
using Blazorise.DataGrid;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.Blazor.Pages.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.GroupBuyManagement
{
    public partial class GroupBuyList
    {
        public string TestProperty { get; set; } = string.Empty;
        public string TenantNameList { get; set; } = string.Empty;
        public List<GroupBuyDto> GroupBuyListItem { get; set; }
        private readonly IItemAppService _itemAppService;
        public bool IsAllSelected { get; private set; } = false;
        private readonly IUiMessageService _uiMessageService;
        
        private readonly IGroupBuyAppService _groupBuyAppService;
        int _pageIndex = 1;
        int _pageSize = 10;
        int Total = 0;

        public GroupBuyList(ITenantAppService tenantAppService, IGroupBuyAppService groupBuyAppService, IUiMessageService messageService) 
        {
            _groupBuyAppService = groupBuyAppService;
            GroupBuyListItem=new List<GroupBuyDto>();
            _uiMessageService = messageService;
        }
        protected override async Task OnInitializedAsync()
        {
            await UpdateGroupBuyList();
        }
        private void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool)e.Value;
            GroupBuyListItem.ForEach(item =>
            {
                item.IsSelected = IsAllSelected;
            });
            StateHasChanged();
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
        private async Task DeleteSelectedAsync()
        {
            var groupBuyItemsids = GroupBuyListItem.Where(x => x.IsSelected).Select(x => x.Id).ToList();
            if (groupBuyItemsids.Count > 0)
            {
                var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                if (confirmed)
                {
                    await _groupBuyAppService.DeleteManyGroupBuyItemsAsync(groupBuyItemsids);
                    await UpdateGroupBuyList();
                    IsAllSelected = false;
                }
            }
        }
    }
}
