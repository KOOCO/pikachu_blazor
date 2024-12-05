using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;

namespace Kooco.Pikachu.BackgroundWorkers
{
    public class OrderStatusCheckerWorker : AsyncPeriodicBackgroundWorkerBase
    {
       
        public OrderStatusCheckerWorker(
            AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory
            )
            : base(timer, serviceScopeFactory)
        {
           

            // Set the period for how frequently you want the task to run
            Timer.Period = 60000; // 10 minutes in milliseconds (adjust as necessary)
        }

        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("Starting: Checking and updating order statuses...");
            using (var scope = workerContext.ServiceProvider.CreateScope())
            {
                // Resolve dependencies
                var _paymentGatewayRepository = scope
                .ServiceProvider
                .GetRequiredService<IRepository<PaymentGateway, Guid>>();
            var _orderRepository = scope
              .ServiceProvider
              .GetRequiredService<IOrderRepository>();
            var _itemDetailsRepository = scope
            .ServiceProvider
            .GetRequiredService<IItemDetailsRepository>();
            var _freebieRepository = scope
            .ServiceProvider
            .GetRequiredService<IFreebieRepository>();
            var _dataFilter = scope
           .ServiceProvider
           .GetRequiredService<IDataFilter>();
            // Resolve the necessary service dependencies
            var validitySettings = (await _paymentGatewayRepository.GetQueryableAsync()).Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).FirstOrDefault();

            var orders = (await _orderRepository.GetQueryableAsync())
                                .Where(order => order.ShippingStatus == ShippingStatus.WaitingForPayment).ToList();

                foreach (var order in orders)
                {
                    DateTime? expirationTime = null;

                    if (validitySettings.Unit == "Days")
                    {
                        expirationTime = order.PaymentDate?.AddDays(validitySettings.Period.Value);
                    }
                    else if (validitySettings.Unit == "Hours")
                    {
                        expirationTime = order.PaymentDate?.AddHours(validitySettings.Period.Value);
                    }
                    else if (validitySettings.Unit == "Minutes")
                    {
                        expirationTime = order.PaymentDate?.AddMinutes(validitySettings.Period.Value);
                    }

                    if (expirationTime.HasValue && expirationTime.Value < DateTime.Now)
                    {
                        order.OrderStatus = OrderStatus.Closed;
                        order.ShippingStatus = ShippingStatus.Closed;

                        foreach (var orderItem in order.OrderItems)
                        {
                            using (_dataFilter.Disable<IMultiTenant>())
                            {
                                var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == orderItem.ItemId && x.ItemName == orderItem.Spec);

                                if (details != null)
                                {
                                    details.SaleableQuantity += orderItem.Quantity;
                                    details.StockOnHand += orderItem.Quantity;

                                    await _itemDetailsRepository.UpdateAsync(details);
                                }

                                if (orderItem.FreebieId != null)
                                {
                                    var freebie = await _freebieRepository.FirstOrDefaultAsync(x => x.Id == orderItem.FreebieId);

                                    if (freebie != null)
                                    {
                                        freebie.FreebieAmount += orderItem.Quantity;
                                        await _freebieRepository.UpdateAsync(freebie);
                                    }
                                }
                            }
                        }

                        await _orderRepository.UpdateAsync(order);
                    }
                }
            }

            Logger.LogInformation("Completed: Order status check and updates.");

        }
    }
}
