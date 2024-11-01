using Asp.Versioning;
using Kooco.Pikachu.DiscountCodes;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Kooco.Pikachu.Controllers.DiscountCodes
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("DiscountCode")]
    [Area("app")]
    [Route("api/app/discount-code")]
    public class DiscountCodeController(IDiscountCodeAppService discountCodeAppService) : PikachuController, IDiscountCodeAppService
    {
        [HttpPost("check-for-discount")]
        public Task<DiscountCheckOutputDto> CheckDiscountCodeAsync(DiscountCheckInputDto input)
        {
            return discountCodeAppService.CheckDiscountCodeAsync(input);
        }

        [HttpPost]
        public Task<DiscountCodeDto> CreateAsync(CreateUpdateDiscountCodeDto input)
        {
            return discountCodeAppService.CreateAsync(input);
        }
        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return discountCodeAppService.DeleteAsync(id);
        }
        [HttpGet("{id}")]
        public Task<DiscountCodeDto> GetAsync(Guid id)
        {
            return discountCodeAppService.GetAsync(id);
        }
        [HttpGet("get-groupbuys/{id}")]
        public Task<List<GroupBuyDto>> GetGroupBuysAsync(Guid id)
        {
            return discountCodeAppService.GetGroupBuysAsync(id);
        }

        [HttpGet]
        public Task<PagedResultDto<DiscountCodeDto>> GetListAsync(GetDiscountCodeListDto input)
        {
            return discountCodeAppService.GetListAsync(input);
        }
        [HttpGet("get-products/{id}")]
        public Task<List<ItemDto>> GetProductsAsync(Guid id)
        {
            return discountCodeAppService.GetProductsAsync(id);
        }

        [HttpPut("{id}")]
        public Task<DiscountCodeDto> UpdateAsync(Guid id, CreateUpdateDiscountCodeDto input)
        {
            return discountCodeAppService.UpdateAsync(id, input);
        }
        [HttpPut("update-status/{id}")]
        public Task UpdateStatusAsync(Guid id)
        {
            return discountCodeAppService.UpdateStatusAsync(id);
        }
    }
}
