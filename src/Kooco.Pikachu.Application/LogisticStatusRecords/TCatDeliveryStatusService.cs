using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Kooco.Pikachu.LogisticStatusRecords;

public class TCatDeliveryStatusService : ITransientDependency
{
    private readonly IOrderDeliveryRepository _orderDeliveryRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IEmailAppService _emailAppService;
    private readonly ILogger<TCatDeliveryStatusService> _logger;
    private readonly OrderHistoryManager _orderHistoryManager;
    private readonly IDataFilter<IMultiTenant> _multiTenantDataFilter;
    private readonly ICurrentTenant _currentTenant;

    public TCatDeliveryStatusService(
        IOrderDeliveryRepository orderDeliveryRepository,
        IOrderRepository orderRepository,
        IEmailAppService emailAppService,
        ILogger<TCatDeliveryStatusService> logger,
        OrderHistoryManager orderHistoryManager,
        IDataFilter<IMultiTenant> multiTenantDataFilter,
        ICurrentTenant currentTenant
        )
    {
        _orderDeliveryRepository = orderDeliveryRepository;
        _orderRepository = orderRepository;
        _emailAppService = emailAppService;
        _logger = logger;
        _orderHistoryManager = orderHistoryManager;
        _multiTenantDataFilter = multiTenantDataFilter;
        _currentTenant = currentTenant;
    }

    public async Task ExecuteAsync(List<LogisticStatusRecordDto> records)
    {
        _logger.LogInformation("Starting T-Cat Delivery Status service");
        _logger.LogInformation("Received {count} records as input", records.Count);

        List<OrderDelivery> orderDeliveries = [];
        var logisticIds = records.Select(r => r.OrderId).Distinct().ToList();

        using (_multiTenantDataFilter.Disable())
        {
            orderDeliveries = await _orderDeliveryRepository.GetListByLogisticsIdsAsync(logisticIds);
        }

        if (orderDeliveries.Count == 0)
        {
            _logger.LogInformation("No order deliveries found to update.");
            return;
        }

        List<Guid> orderIdsToUpdate = [];

        foreach (var record in records)
        {
            var orderDelivery = orderDeliveries.FirstOrDefault(od => od.AllPayLogisticsID == record.OrderId || od.DeliveryNo == record.OrderId);
            if (orderDelivery != null)
            {
                var deliveryStatus = DeliveryStatusMapper.MapStatus(orderDelivery.DeliveryMethod, record.StatusCode);
                if (deliveryStatus != null)
                {
                    if (deliveryStatus != orderDelivery.DeliveryStatus)
                    {
                        _logger.LogInformation("Updating delivery status to {status} for order delivery {delivery}", deliveryStatus, orderDelivery.Id);
                        orderDelivery.DeliveryStatus = deliveryStatus.Value;
                        orderIdsToUpdate.Add(orderDelivery.OrderId);
                    }
                }
                else
                {
                    using (_currentTenant.Change(orderDelivery.TenantId))
                    {
                        _logger.LogInformation("Abnormal status code {statusCode} for order delivery {orderDelivery}.", record.StatusCode, orderDelivery.Id);
                        await _orderHistoryManager.AddOrderHistoryAsync(
                            orderDelivery.OrderId,
                            "T-CatAbnormalStatus",
                            [orderDelivery.DeliveryNo ?? string.Empty, record.StatusCode, record.StatusMessage ?? string.Empty],
                            null,
                            null
                        );
                    }
                }
            }
        }

        orderIdsToUpdate = [.. orderIdsToUpdate.Distinct()];

        if (orderIdsToUpdate.Count > 0)
        {
            List<Order> ordersToUpdate;
            using (_multiTenantDataFilter.Disable())
            {
                ordersToUpdate = await _orderRepository.GetListAsync(o => orderIdsToUpdate.Contains(o.Id));
            }

            foreach (var order in ordersToUpdate)
            {
                var thisOrderDeliveries = orderDeliveries
                    .Where(od => od.OrderId == order.Id)
                    .Select(od => od.DeliveryStatus)
                    .ToList();

                if (thisOrderDeliveries.Count != 0)
                {
                    var lowestOrderValue = thisOrderDeliveries
                        .Min(GetStatusRank);

                    var latestStatusAllPassed = DeliveryStatusProgression[lowestOrderValue];

                    var shippingStatus = latestStatusAllPassed switch
                    {
                        DeliveryStatus.Processing => ShippingStatus.PrepareShipment,
                        DeliveryStatus.ToBeShipped => ShippingStatus.ToBeShipped,
                        DeliveryStatus.Shipped => ShippingStatus.Shipped,
                        DeliveryStatus.Delivered => ShippingStatus.Delivered,
                        DeliveryStatus.PickedUp => ShippingStatus.PickedUp,
                        _ => order.ShippingStatus
                    };

                    if (shippingStatus != order.ShippingStatus)
                    {
                        using (_currentTenant.Change(order.TenantId))
                        {
                            await _orderHistoryManager.AddOrderHistoryAsync(
                                order.Id,
                                "ShippingStatusChanged",
                                [order.ShippingStatus, shippingStatus],
                                null,
                                null
                            );

                            order.ShippingStatus = shippingStatus;
                            await _emailAppService.SendOrderStatusEmailAsync(order.Id, shippingStatus: shippingStatus);
                        }

                    }
                }
            }
        }

        _logger.LogInformation("Finished T-Cat SFTP service execution.");
    }


    private static readonly List<DeliveryStatus> DeliveryStatusProgression =
    [
        DeliveryStatus.Processing,
        DeliveryStatus.ToBeShipped,
        DeliveryStatus.Shipped,
        DeliveryStatus.Delivered,
        DeliveryStatus.PickedUp,
        DeliveryStatus.Returned
    ];

    int GetStatusRank(DeliveryStatus status) =>
        DeliveryStatusProgression.IndexOf(status);

    private static class DeliveryStatusMapper
    {
        private static readonly Dictionary<DeliveryMethod, Dictionary<string, DeliveryStatus>> StatusMap = new()
        {
            [DeliveryMethod.TCatDeliveryNormal] = new()
            {
                ["00000"] = DeliveryStatus.ToBeShipped,
                ["00001"] = DeliveryStatus.Shipped,
                ["00006"] = DeliveryStatus.Shipped,
                ["00013"] = DeliveryStatus.Shipped,
                ["00015"] = DeliveryStatus.Shipped,
                ["00019"] = DeliveryStatus.Shipped,
                ["00020"] = DeliveryStatus.Shipped,
                ["00219"] = DeliveryStatus.Shipped,
                ["00003"] = DeliveryStatus.Delivered,
                ["00017"] = DeliveryStatus.Returned,
                ["00301"] = DeliveryStatus.Returned
            },
            [DeliveryMethod.TCatDeliverySevenElevenNormal] = new()
            {
                ["00000"] = DeliveryStatus.ToBeShipped,
                ["00001"] = DeliveryStatus.Shipped,
                ["202"] = DeliveryStatus.Shipped,
                ["204"] = DeliveryStatus.Shipped,
                ["208"] = DeliveryStatus.Delivered,
                ["00003"] = DeliveryStatus.PickedUp,
                ["00017"] = DeliveryStatus.Returned,
                ["00301"] = DeliveryStatus.Returned
            },
            [DeliveryMethod.TCatDeliveryFreeze] = new()
            {
                ["00000"] = DeliveryStatus.ToBeShipped,
                ["00001"] = DeliveryStatus.Shipped,
                ["00006"] = DeliveryStatus.Shipped,
                ["00013"] = DeliveryStatus.Shipped,
                ["00015"] = DeliveryStatus.Shipped,
                ["00019"] = DeliveryStatus.Shipped,
                ["00020"] = DeliveryStatus.Shipped,
                ["00219"] = DeliveryStatus.Shipped,
                ["00003"] = DeliveryStatus.Delivered,
                ["00017"] = DeliveryStatus.Returned,
                ["00301"] = DeliveryStatus.Returned
            },
            [DeliveryMethod.TCatDeliverySevenElevenFreeze] = new()
            {
                ["00000"] = DeliveryStatus.ToBeShipped,
                ["00001"] = DeliveryStatus.Shipped,
                ["202"] = DeliveryStatus.Shipped,
                ["204"] = DeliveryStatus.Shipped,
                ["208"] = DeliveryStatus.Delivered,
                ["00003"] = DeliveryStatus.PickedUp,
                ["00017"] = DeliveryStatus.Returned,
                ["00301"] = DeliveryStatus.Returned
            },
            [DeliveryMethod.TCatDeliveryFrozen] = new()
            {
                ["00000"] = DeliveryStatus.ToBeShipped,
                ["00001"] = DeliveryStatus.Shipped,
                ["00006"] = DeliveryStatus.Shipped,
                ["00013"] = DeliveryStatus.Shipped,
                ["00015"] = DeliveryStatus.Shipped,
                ["00019"] = DeliveryStatus.Shipped,
                ["00020"] = DeliveryStatus.Shipped,
                ["00219"] = DeliveryStatus.Shipped,
                ["00003"] = DeliveryStatus.Delivered,
                ["00017"] = DeliveryStatus.Returned,
                ["00301"] = DeliveryStatus.Returned
            },
            [DeliveryMethod.TCatDeliverySevenElevenFrozen] = new()
            {
                ["00000"] = DeliveryStatus.ToBeShipped,
                ["00001"] = DeliveryStatus.Shipped,
                ["202"] = DeliveryStatus.Shipped,
                ["204"] = DeliveryStatus.Shipped,
                ["208"] = DeliveryStatus.Delivered,
                ["00003"] = DeliveryStatus.PickedUp,
                ["00017"] = DeliveryStatus.Returned,
                ["00301"] = DeliveryStatus.Returned
            }
        };

        public static DeliveryStatus? MapStatus(DeliveryMethod logistics, string statusId)
        {
            return StatusMap.TryGetValue(logistics, out var statusMap) &&
                   statusMap.TryGetValue(statusId, out var status)
                ? status
                : null;
        }
    }
}
