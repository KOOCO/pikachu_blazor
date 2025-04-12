using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.PaymentGateways;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Kooco.Pikachu.BackgroundWorkers;
public class OrderStatusCheckerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public OrderStatusCheckerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
    {
        // Set the period for how frequently you want the task to run
        Timer.Period = 600000; // 10 minutes in milliseconds (adjust as necessary)
    }
    protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var _unitOfWorkManager = workerContext.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
        Logger.LogInformation("Starting: Checking and updating order statuses...");
        using (var uow = _unitOfWorkManager.Begin())
        {
            // Resolve dependencies
            var _paymentGatewayRepository = workerContext
        .ServiceProvider
        .GetRequiredService<IRepository<PaymentGateway, Guid>>();
            var _orderRepository = workerContext
              .ServiceProvider
              .GetRequiredService<IOrderRepository>();
            var _itemDetailsRepository = workerContext
            .ServiceProvider
            .GetRequiredService<IItemDetailsRepository>();
            var _freebieRepository = workerContext
            .ServiceProvider
            .GetRequiredService<IFreebieRepository>();
            var _dataFilter = workerContext
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
                    expirationTime = order.CreationTime.AddDays(validitySettings.Period.Value);
                }
                else if (validitySettings.Unit == "Hours")
                {
                    expirationTime = order.CreationTime.AddHours(validitySettings.Period.Value);
                }
                else if (validitySettings.Unit == "Minutes")
                {
                    expirationTime = order.CreationTime.AddMinutes(validitySettings.Period.Value);
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


