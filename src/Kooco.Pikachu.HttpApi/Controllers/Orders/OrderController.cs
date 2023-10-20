using Kooco.Pikachu.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
        public Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
        {
            return _ordersAppService.UpdateAsync(id, input);
        }
        [HttpPut("{id}/{update-shipping}")]
        public Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
        {
            return _ordersAppService.UpdateAsync(id, input);
        }

        [HttpPut("{id}/{commentId}/{comment}")]
        public Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
        {
            return _ordersAppService.UpdateStoreCommentAsync(id, commentId, comment);
        }

        [HttpPost("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleCallbackAsync()
        {
            try
            {
                string requestBody = string.Empty;
                // Read the request body into a string
                using (StreamReader reader = new(Request.Body, Encoding.UTF8))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                var form = await Request.ReadFormAsync();

                bool validRtnCode = int.TryParse(form["RtnCode"], out int rtnCode);
                bool validTradeAmt = int.TryParse(form["TradeAmt"], out int tradeAmt);
                bool validPaymentTypeChargeFee = int.TryParse(form["PaymentTypeChargeFee"], out int paymentTypeChargeFee);
                bool validSimulatePaid = int.TryParse(form["SimulatePaid"], out int simulatePaid);

                var paymentResult = new PaymentResult
                {
                    MerchantID = form["MerchantID"],
                    MerchantTradeNo = form["MerchantTradeNo"],
                    StoreID = form["StoreID"],
                    RtnCode = validRtnCode ? rtnCode : 0,
                    RtnMsg = form["RtnMsg"],
                    TradeNo = form["TradeNo"],
                    TradeAmt = validTradeAmt ? tradeAmt : 0,
                    PaymentDate = form["PaymentDate"],
                    PaymentType = form["PaymentType"],
                    PaymentTypeChargeFee = validPaymentTypeChargeFee ? paymentTypeChargeFee : 0,
                    TradeDate = form["TradeDate"],
                    SimulatePaid = validSimulatePaid ? simulatePaid : 0,
                    CheckMacValue = form["CheckMacValue"],
                    CustomField1 = form["CustomField1"]
                };

                await _ordersAppService.HandlePaymentAsync(paymentResult);
                return Ok("1|OK");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("dummy")]
        public Task HandlePaymentAsync(PaymentResult paymentResult)
        {
            throw new NotImplementedException();
        }

        [HttpPut("check-mac-value/{id}/{checkMacValue}")]
        public Task AddCheckMacValueAsync(Guid id, string checkMacValue)
        {
            return _ordersAppService.AddCheckMacValueAsync(id, checkMacValue);
        }

    }
}
