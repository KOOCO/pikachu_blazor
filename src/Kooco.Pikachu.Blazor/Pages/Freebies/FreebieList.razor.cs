using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Freebies.Dtos;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using System.Collections.Generic;
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
    }
}
