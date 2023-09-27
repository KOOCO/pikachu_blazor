using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace Kooco.Pikachu.Controllers.Orders
{
    [RemoteService(IsEnabled = true)]
    [ControllerName("Orders")]
    [Area("app")]
    [Route("api/app/orders")]
    public class OrderController : AbpController, IOrderAppService
    {
        private readonly IOrderAppService _ordersAppService;
        public OrderController(
            IOrderAppService ordersAppService)
        {
            _ordersAppService = ordersAppService;
        }
        [HttpPost]
        public Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            return _ordersAppService.CreateAsync(input);
        }

        [HttpDelete("{id}")]
        public Task DeleteAsync(Guid id)
        {
            return _ordersAppService.DeleteAsync(id);
        }
        [HttpGet("{id}")]
        public Task<OrderDto> GetAsync(Guid id)
        {
            return _ordersAppService.GetAsync(id);
        }
        [HttpGet("get-list")]
        public Task<PagedResultDto<OrderDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            return _ordersAppService.GetListAsync(input);
        }
        [HttpPut("{id}")]
        public Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
        {
            return _ordersAppService.UpdateAsync(id, input);
        }
    }
}
