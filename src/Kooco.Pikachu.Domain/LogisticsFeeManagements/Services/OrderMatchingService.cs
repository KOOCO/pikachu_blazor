using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.OrderTradeNos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.LogisticsFeeManagements.Services
{
    public class OrderMatchingService : IOrderMatchingService, ITransientDependency
    {
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly ILogger<OrderMatchingService> _logger;
        private readonly IRepository<OrderTradeNo, Guid> _orderTradeNoRepository;
        public OrderMatchingService(
            IRepository<Order, Guid> orderRepository,
            ILogger<OrderMatchingService> logger, IRepository<OrderTradeNo, Guid> orderTradeNoRepository)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _orderTradeNoRepository = orderTradeNoRepository;
                
        }

        public async Task<OrderMatchingResult> FindOrderAsync(string merchantTradeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(merchantTradeNo))
                {
                    return new OrderMatchingResult { IsFound = false };
                }

                var orderQueryable = await _orderRepository.GetQueryableAsync();
                var tradeNoQueryable = await _orderTradeNoRepository.GetQueryableAsync();

                var order = (from orders in orderQueryable
                              join trade in tradeNoQueryable
                                  on orders.Id equals trade.OrderId into gj
                              from subTrade in gj.DefaultIfEmpty()
                              where orders.OrderNo == merchantTradeNo || subTrade.MarchentTradeNo == merchantTradeNo
                              select  orders
                                 
                              ).FirstOrDefault();

                if (order == null)
                {
                    return new OrderMatchingResult { IsFound = false };
                }

                return new OrderMatchingResult
                {
                    IsFound = true,
                    TenantId = order.TenantId,
                    OrderId = order.Id,
                    OrderNumber =order.OrderNo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error matching order for merchant trade no: {MerchantTradeNo}", merchantTradeNo);
                return new OrderMatchingResult { IsFound = false };
            }
        }
    }
}
