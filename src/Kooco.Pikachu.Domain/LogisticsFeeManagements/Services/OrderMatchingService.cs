using Kooco.Pikachu.Orders.Entities;
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

        public OrderMatchingService(
            IRepository<Order, Guid> orderRepository,
            ILogger<OrderMatchingService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderMatchingResult> FindOrderAsync(string merchantTradeNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(merchantTradeNo))
                {
                    return new OrderMatchingResult { IsFound = false };
                }

                var queryable = await _orderRepository.GetQueryableAsync();
                var order = queryable
                    .Where(o => o.MerchantTradeNo == merchantTradeNo || o.OrderNo == merchantTradeNo)
                    .Select(o => new { o.Id, o.TenantId, o.OrderNo })
                    .FirstOrDefault();

                if (order == null)
                {
                    return new OrderMatchingResult { IsFound = false };
                }

                return new OrderMatchingResult
                {
                    IsFound = true,
                    TenantId = order.TenantId,
                    OrderId = order.Id,
                    OrderNumber = order.OrderNo
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
