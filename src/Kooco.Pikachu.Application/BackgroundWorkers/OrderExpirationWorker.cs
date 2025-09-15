using ECPay.Payment.Integration;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.InboxManagement.Managers;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.PaymentGateways;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Kooco.Pikachu.BackgroundWorkers
{
    public class OrderExpirationWorker : AsyncPeriodicBackgroundWorkerBase
    {

        public OrderExpirationWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            // Set the period to run twice a day (12 hours = 43,200,000 milliseconds)
            Timer.Period = 43200000; // 12 hours in milliseconds
            //Timer.Period = 360000;
        }

        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var _unitOfWorkManager = workerContext.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            var _currentTenant = workerContext.ServiceProvider.GetRequiredService<ICurrentTenant>();
            var _tenantStore = workerContext.ServiceProvider.GetRequiredService<ITenantStore>();


            Logger.LogInformation("Starting: Order expiration check and updates...");

            using (var uow = _unitOfWorkManager.Begin())
            {
                // Process host tenant (null tenant) first
                await ProcessExpiredOrdersForTenantAsync(null, workerContext);

                // Get all tenants and process each one
                var tenants = await _tenantStore.GetListAsync();

                foreach (var tenant in tenants)
                {
                    try
                    {
                        using (_currentTenant.Change(tenant.Id))
                        {
                            await ProcessExpiredOrdersForTenantAsync(tenant.Id, workerContext);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error processing expired orders for tenant {TenantId}: {Message}", tenant.Id, ex.Message);
                    }
                }

                await uow.CompleteAsync();
            }

            Logger.LogInformation("Completed: Order expiration check and updates.");
        }

        private async Task ProcessExpiredOrdersForTenantAsync(Guid? tenantId, PeriodicBackgroundWorkerContext workerContext)
        {
            Logger.LogInformation("Processing expired orders for tenant {TenantId}", tenantId?.ToString() ?? "Host");

            // Resolve dependencies for current tenant

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
            var _orderHistoryManager = workerContext
                .ServiceProvider
                .GetRequiredService<OrderHistoryManager>();
            var _notificationManager = workerContext
                .ServiceProvider
                .GetRequiredService<NotificationManager>();
            var _dataFilter = workerContext
                .ServiceProvider
                .GetRequiredService<IDataFilter>();
            var L = workerContext.ServiceProvider.GetRequiredService<IStringLocalizer<PikachuResource>>();
            // Get validity settings for current tenant
            var validitySettings = (await _paymentGatewayRepository.GetQueryableAsync())
                .Where(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod)
                .FirstOrDefault();

            if (validitySettings == null || !validitySettings.Period.HasValue)
            {
                Logger.LogInformation("No validity settings found for tenant {TenantId}, skipping", tenantId?.ToString() ?? "Host");
                return;
            }

            // Get orders waiting for payment that are not excluded payment methods
            var orders = (await _orderRepository.GetQueryableAsync())
                .Where(order => order.ShippingStatus == ShippingStatus.WaitingForPayment
                               && order.OrderStatus != OrderStatus.Closed
                               // Exclude Manual Bank Transfer and Free Order payment methods
                               && order.PaymentMethod != PaymentMethods.ManualBankTransfer
                               && order.PaymentMethod != PaymentMethods.FreeOrder)
                .Include(o => o.OrderItems)
                .ToList();

            int expiredCount = 0;

            foreach (var order in orders)
            {
                DateTime? expirationTime = null;

                // Calculate expiration time based on validity settings
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

                // Check if order has expired
                if (expirationTime.HasValue && expirationTime.Value < DateTime.Now)
                {
                    try
                    {
                        await ExpireOrderAsync(order, _orderRepository, _itemDetailsRepository, _freebieRepository,
                                             _orderHistoryManager, _notificationManager, _dataFilter, L);

                        expiredCount++;

                        Logger.LogInformation("Expired order {OrderId} ({OrderNo}) for tenant {TenantId}",
                                            order.Id, order.OrderNo, tenantId?.ToString() ?? "Host");
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Error expiring order {OrderId} for tenant {TenantId}: {Message}",
                                      order.Id, tenantId?.ToString() ?? "Host", ex.Message);
                    }
                }
            }

            Logger.LogInformation("Processed {ExpiredCount} expired orders out of {TotalCount} orders for tenant {TenantId}",
                                expiredCount, orders.Count, tenantId?.ToString() ?? "Host");
        }

        private async Task ExpireOrderAsync(
            Order order,
            IOrderRepository orderRepository,
            IItemDetailsRepository itemDetailsRepository,
            IFreebieRepository freebieRepository,
            OrderHistoryManager orderHistoryManager,
            NotificationManager notificationManager,
            IDataFilter dataFilter,
            IStringLocalizer<PikachuResource> l)
        {
            // Capture previous order status before updating
            var oldOrderStatus = order.OrderStatus;
            var oldShippingStatus = order.ShippingStatus;

            // Update order status
            order.OrderStatus = OrderStatus.Closed;
            order.ShippingStatus = ShippingStatus.Closed;

            // Restore inventory for order items
            foreach (var orderItem in order.OrderItems)
            {
                using (dataFilter.Disable<IMultiTenant>())
                {
                    var details = await itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == orderItem.ItemId && x.ItemName == orderItem.Spec);

                    if (details != null)
                    {
                        details.SaleableQuantity += orderItem.Quantity;
                        details.StockOnHand += orderItem.Quantity;
                        await itemDetailsRepository.UpdateAsync(details);
                    }

                    // Handle freebies
                    if (orderItem.FreebieId != null)
                    {
                        var freebie = await freebieRepository.FirstOrDefaultAsync(x => x.Id == orderItem.FreebieId);

                        if (freebie != null)
                        {
                            freebie.FreebieAmount += orderItem.Quantity;
                            await freebieRepository.UpdateAsync(freebie);
                        }
                    }
                }
            }

            await orderRepository.UpdateAsync(order);

            // Log order history with specific descriptions
            var currentUserId = Guid.Empty; // System user
            var editorName = l["CloseOrderJob"];
            var actionName = "OrderClosed";

            // English version - Shipping Status Change
            await orderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                actionName + "Shipping",
                [
                        l[oldShippingStatus.ToString()].Name,
                        l[order.ShippingStatus.ToString()].Name,

                    ], // Dynamic placeholders
                currentUserId,
                editorName
            );

            // English version - Order Status Change
            await orderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                actionName + "Status",
                   [l[oldShippingStatus.ToString()].Name,
                    l[order.OrderStatus.ToString()].Name,],
                currentUserId,
                editorName
            );

            // Send notification
            await notificationManager.OrderExpiredAsync(
                NotificationArgs.ForOrderWithUserName(
                    order.Id,
                    order.OrderNo,
                    l["CloseOrderJob"]
                ));
        }

        private async Task LogOrderExpirationHistoryAsync(Order order, OrderHistoryManager orderHistoryManager, IStringLocalizer<PikachuResource> l)
        {
            var currentUserId = Guid.Empty; // System user
            var editorName = l["CloseOrderJob"];
            var actionName = "OrderClosed";

            // English version - Shipping Status Change
            await orderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                actionName + "Shipping",
                [
                        l[order.ShippingStatus.ToString()].Name,
                        l[ShippingStatus.Closed.ToString()].Name,

                    ], // Dynamic placeholders
                currentUserId,
                editorName
            );

            // English version - Order Status Change
            await orderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                actionName + "Status",
                   [l[order.OrderStatus.ToString()].Name,
                    l[OrderStatus.Closed.ToString()].Name,],
                currentUserId,
                editorName
            );


        }
    }

}
