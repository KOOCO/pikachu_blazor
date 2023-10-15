using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
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

        [HttpPut("{id}/{comment}")]
        public Task AddStoreCommentAsync(Guid id, string comment)
        {
            return _ordersAppService.AddStoreCommentAsync(id, comment);
        }

        [HttpPost]
        public Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            return _ordersAppService.CreateAsync(input);
        }

        [HttpGet("{id}")]
        public Task<OrderDto> GetAsync(Guid id)
        {
            return _ordersAppService.GetAsync(id);
        }
        
        [HttpGet("get-list")]
        public Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input)
        {
            return _ordersAppService.GetListAsync(input);
        }
        
        [HttpGet("with-details/{id}")]
        public Task<OrderDto> GetWithDetailsAsync(Guid id)
        {
            return _ordersAppService.GetWithDetailsAsync(id);
        }
        [HttpPut("{id}/{update}")]
        public Task<OrderDto> UpdateAsync(Guid id,CreateOrderDto input)
        {
            return _ordersAppService.UpdateAsync(id,input);
        }
        [HttpPut("{id}/{commentId}/{comment}")]
        public Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
        {
            return _ordersAppService.UpdateStoreCommentAsync(id, commentId, comment);
        }

        [HttpPost("callback")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        [Produces("text/html")]
        public async Task<IActionResult> HandleCallbackAsync([FromForm]PaymentResult info)
        {
            // Handle the callback data here
            // This is just a placeholder. Replace with your actual handling code.
            try
            {
                //await _ordersAppService.HandlePaymentAsync(result);
                return Ok("1|OK");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("dummy")]
        public Task HandlePaymentAsync(PaymentResult result)
        {
            throw new NotImplementedException();
        }
    }
}
