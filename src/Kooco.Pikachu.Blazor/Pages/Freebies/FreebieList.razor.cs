using Blazorise.DataGrid;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu.Blazor.Pages.Freebies
{
    public partial class FreebieList
    {
       
        public List<FreebieDto> FreebieListItems { get; set; }
        private readonly IFreebieAppService _freebieAppService;
        private readonly IUiMessageService _uiMessageService;
        public bool IsAllSelected = false;
        int PageIndex = 1;
        int PageSize = 10;
        int Total = 0;
        public FreebieList(ITenantAppService tenantAppService, IFreebieAppService freebieAppService, IUiMessageService messageService)
        {
            _freebieAppService = freebieAppService;
            FreebieListItems = new List<FreebieDto>();
            _uiMessageService = messageService;
        }
        protected override async Task OnInitializedAsync()
        {
            await UpdateFreebieList();
        }
        private async Task UpdateFreebieList()
        {
            //int skipCount = PageIndex * PageSize;
          FreebieListItems = await _freebieAppService.GetListAsync();
            

        }
        public void OnEditItem(DataGridRowMouseEventArgs<FreebieDto> e)
        {
            var id = e.Item.Id;
            NavigationManager.NavigateTo($"Freebie/Edit/{id}");
        }
        private void HandleSelectAllChange(ChangeEventArgs e)
        {
            IsAllSelected = (bool)e.Value;
            FreebieListItems.ForEach(freebie =>
            {
                freebie.IsSelected = IsAllSelected;
            });
            StateHasChanged();
        }
        private async Task DeleteSelectedAsync()
        {
            try
            {
                var freebieIds = FreebieListItems.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                if (freebieIds.Count > 0)
                {
                    var confirmed = await _uiMessageService.Confirm(L["AreYouSureToDeleteSelectedItem"]);
                    if (confirmed)
                    {
                        await _freebieAppService.DeleteManyItemsAsync(freebieIds);
                        await UpdateFreebieList();
                        IsAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType()?.ToString());
            }
        }
    }
}
