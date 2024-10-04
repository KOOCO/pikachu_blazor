using Asp.Versioning;
using Kooco.Pikachu.AddOnProducts;
using Kooco.Pikachu.UserAddresses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.AddOnProducts
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("AddOnProduct")]
    [Area("app")]
    [Route("api/app/add-on-product")]
    public class AddOnProductController(IAddOnProductAppService addOnProductAppService) : PikachuController, IAddOnProductAppService
    {
        [HttpPost]
        public Task<AddOnProductDto> CreateAsync(CreateUpdateAddOnProductDto input)
        {
            return addOnProductAppService.CreateAsync(input);
        }
        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid Id)
        {
            return addOnProductAppService.DeleteAsync(Id);
        }
        [HttpGet("{id}")]
        public Task<AddOnProductDto> GetAsync(Guid Id)
        {
            return addOnProductAppService.GetAsync(Id);
        }
        [HttpGet]
        public Task<PagedResultDto<AddOnProductDto>> GetListAsync(GetAddOnProductListDto input)
        {
            return addOnProductAppService.GetListAsync(input);
        }
        [HttpPut("{id}")]
        public Task<AddOnProductDto> UpdateAsync(Guid Id, CreateUpdateAddOnProductDto input)
        {
            return addOnProductAppService.UpdateAsync(Id, input);
        }
        [HttpPut("update-status/{id}")]
        public Task UpdateStatusAsync(Guid id)
        {
            return addOnProductAppService.UpdateStatusAsync(id);
        }
    }
}
