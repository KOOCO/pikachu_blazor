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
using Volo.Abp;
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
           
        }
        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<FreebieDto> e)
        {
            PageIndex = e.Page - 1;
            await UpdateFreebieList();
            await InvokeAsync(StateHasChanged);
        }
        private async Task UpdateFreebieList()
        {
            

            //FreebieListItems = await _freebieAppService.GetListAsync();
            int skipCount = PageIndex * PageSize;
            var result = await _freebieAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                Sorting = nameof(ItemDto.ItemName),
                MaxResultCount = PageSize,
                SkipCount = skipCount
            });
            FreebieListItems = result.Items.ToList();
            Total = (int)result.TotalCount;

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
             public async Task OnFreebieAvaliablityChange(Guid id)
        {
            try
            {
                var freebie =FreebieListItems.Where(x => x.Id == id).First();
                freebie.IsFreebieAvaliable = !freebie.IsFreebieAvaliable;
                await _freebieAppService.ChangeFreebieAvailability(id);
                await UpdateFreebieList();
                await InvokeAsync(StateHasChanged);
            }
            catch (BusinessException ex)
            {
                await _uiMessageService.Error(ex.Code.ToString());
            }
            catch (Exception ex)
            {
                await _uiMessageService.Error(ex.GetType().ToString());
            }
        }
    }
}
