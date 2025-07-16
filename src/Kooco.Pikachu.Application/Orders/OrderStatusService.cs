using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.OrderTransactions;
using Kooco.Pikachu.Refunds;
using Kooco.Pikachu.Tenants.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Kooco.Pikachu.Orders
{
    /// <summary>
    /// Service responsible for Order status management operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public class OrderStatusService : ApplicationService, IOrderStatusService
    {
        public required IOrderRepository OrderRepository { get; init; }
        public required OrderHistoryManager OrderHistoryManager { get; init; }
        public required IEmailAppService EmailAppService { get; init; }
        public required IOrderInvoiceAppService ElectronicInvoiceAppService { get; init; }
        public required ITenantTripartiteRepository TenantTripartiteRepository { get; init; }
        public required IRefundAppService RefundAppService { get; init; }
        public required IRepository<Refund, Guid> RefundRepository { get; init; }
        public required OrderTransactionManager OrderTransactionManager { get; init; }

        public async Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status)
        {
            var order = await OrderRepository.GetWithDetailsAsync(id);
            var oldStatus = order.ShippingStatus;
            order.ShippingStatus = status;
            
            if (status != ShippingStatus.Completed)
            {
                order.CancellationDate = DateTime.Now;
            }

            if (status == ShippingStatus.Completed || status == ShippingStatus.Closed)
            {
                order.OrderStatus = OrderStatus.Closed;
            }

            await OrderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            var returnResult = ObjectMapper.Map<Order, OrderDto>(order);

            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            if (status == ShippingStatus.Delivered)
            {
                var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(currentUserId);
                if (order.InvoiceNumber.IsNullOrEmpty())
                {
                    if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Delivered)
                    {
                        if (order.GroupBuy.IssueInvoice)
                        {
                            order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                            await UnitOfWorkManager.Current.SaveChangesAsync();
                            var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                            if (invoiceDely == 0)
                            {
                                var result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                                returnResult.InvoiceMsg = result?.ToString() ?? string.Empty;
                            }
                        }
                    }
                }
            }

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "StatusChanged",
                new object[] { L[oldStatus.ToString()]?.Value, L[status.ToString()]?.Value },
                currentUserId,
                currentUserName
            );
            
            return returnResult;
        }

        public async Task<OrderDto> OrderToBeShipped(Guid id)
        {
            var order = await OrderRepository.GetWithDetailsAsync(id);
            var oldShippingStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.ToBeShipped;
            order.ShippedBy = CurrentUser.Name;
            order.ShippingDate = DateTime.Now;

            await OrderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            var returnOrder = ObjectMapper.Map<Order, OrderDto>(order);
            
            if (order.InvoiceNumber.IsNullOrEmpty())
            {
                var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);
                if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.ToBeShipped)
                {
                    if (order.GroupBuy.IssueInvoice)
                    {
                        order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                        var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                        if (invoiceDely == 0)
                        {
                            var result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                            returnOrder.InvoiceMsg = result?.ToString() ?? string.Empty;
                        }
                    }
                }
            }
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderToBeShipped",
                new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );

            return returnOrder;
        }

        public async Task<OrderDto> OrderShipped(Guid id)
        {
            var order = await OrderRepository.GetWithDetailsAsync(id);
            var oldShippingStatus = order.ShippingStatus;

            order.ShippingStatus = ShippingStatus.Shipped;
            order.ShippedBy = CurrentUser.Name;
            order.ShippingDate = DateTime.Now;

            await OrderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await EmailAppService.SendLogisticsEmailAsync(order.Id);
            await SendEmailAsync(order.Id);
            var returnOrder = ObjectMapper.Map<Order, OrderDto>(order);
            
            if (order.InvoiceNumber.IsNullOrEmpty())
            {
                var invoiceSetting = await TenantTripartiteRepository.FindByTenantAsync(CurrentTenant.Id.Value);
                if (invoiceSetting.StatusOnInvoiceIssue == DeliveryStatus.Shipped)
                {
                    if (order.GroupBuy.IssueInvoice)
                    {
                        order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
                        var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
                        if (invoiceDely == 0)
                        {
                            var result = await ElectronicInvoiceAppService.CreateInvoiceAsync(order.Id);
                            returnOrder.InvoiceMsg = result?.ToString() ?? string.Empty;
                        }
                    }
                }
            }
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderShipped",
                new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );

            return returnOrder;
        }

        public async Task<OrderDto> OrderComplete(Guid id)
        {
            var order = await OrderRepository.GetWithDetailsAsync(id);
            var oldShippingStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.Completed;
            order.CompletedBy = CurrentUser.Name;
            order.CompletionTime = DateTime.Now;

            var oldOrderStatus = order.OrderStatus;
            order.OrderStatus = OrderStatus.Closed;

            await OrderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderCompleted",
                new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderStatusChanged",
                new object[] { L[oldOrderStatus.ToString()].Name, L[order.OrderStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task<OrderDto> OrderClosed(Guid id)
        {
            var order = await OrderRepository.GetWithDetailsAsync(id);
            var oldShippingStatus = order.ShippingStatus;
            order.ShippingStatus = ShippingStatus.Closed;
            order.ClosedBy = CurrentUser.Name;
            order.CancellationDate = DateTime.Now;

            var oldOrderStatus = order.OrderStatus;
            order.OrderStatus = OrderStatus.Closed;

            await OrderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderClosed",
                new object[] { L[oldShippingStatus.ToString()].Name, L[order.ShippingStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderClosed",
                new object[] { L[oldOrderStatus.ToString()].Name, L[order.OrderStatus.ToString()].Name },
                currentUserId,
                currentUserName
            );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task ReturnOrderAsync(Guid id)
        {
            var order = await OrderRepository.GetAsync(id);
            var oldReturnStatus = order.ReturnStatus;
            var oldOrderStatus = order.OrderStatus;
            order.ReturnStatus = OrderReturnStatus.Pending;
            order.OrderStatus = OrderStatus.Returned;

            await OrderRepository.UpdateAsync(order);
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderReturnInitiated",
                new object[]
                {
                    L[oldReturnStatus.ToString()].Name,
                    L[order.ReturnStatus.ToString()].Name,
                    L[oldOrderStatus.ToString()].Name,
                    L[order.OrderStatus.ToString()].Name
                },
                currentUserId,
                currentUserName
            );
        }

        public async Task ExchangeOrderAsync(Guid id)
        {
            var order = await OrderRepository.GetAsync(id);
            var oldReturnStatus = order.ReturnStatus;
            var oldOrderStatus = order.OrderStatus;
            order.ReturnStatus = OrderReturnStatus.Pending;
            order.OrderStatus = OrderStatus.Exchange;

            await OrderRepository.UpdateAsync(order);
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "OrderExchangeInitiated",
                new object[]
                {
                    L[oldReturnStatus.ToString()]?.Value,
                    L[order.ReturnStatus.ToString()]?.Value,
                    L[oldOrderStatus.ToString()]?.Value,
                    L[order.OrderStatus.ToString()]?.Value
                },
                currentUserId,
                currentUserName
            );
        }

        public async Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus, bool isRefund)
        {
            var order = await OrderRepository.GetAsync(id);
            var oldReturnStatus = order.ReturnStatus;
            order.ReturnStatus = orderReturnStatus;

            var oldShippingStatus = order.ShippingStatus;
            var oldOrderStatus = order.OrderStatus;

            if (orderReturnStatus == OrderReturnStatus.Reject)
            {
                order.OrderStatus = OrderStatus.Open;
            }

            if (orderReturnStatus == OrderReturnStatus.Approve)
            {
                order.ReturnStatus = OrderReturnStatus.Processing;
            }
            
            if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Returned)
            {
                order.ShippingStatus = ShippingStatus.Closed;
                order.OrderStatus = OrderStatus.Closed;

                if (isRefund)
                {
                    await RefundAppService.CreateAsync(id);
                    var refund = (await RefundRepository.GetQueryableAsync()).Where(x => x.OrderId == order.Id).FirstOrDefault();
                    await RefundAppService.UpdateRefundReviewAsync(refund.Id, RefundReviewStatus.Proccessing);
                    await RefundAppService.SendRefundRequestAsync(refund.Id);

                    var orderTransaction = new OrderTransaction(GuidGenerator.Create(), order.Id, order.OrderNo,
                        order.TotalAmount, TransactionType.Returned, TransactionStatus.Successful, order.PaymentMethod?.GetPaymentChannel());
                    await OrderTransactionManager.CreateAsync(orderTransaction);
                }
            }
            
            if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
            {
                order.ShippingStatus = ShippingStatus.Completed;
                order.OrderStatus = OrderStatus.Closed;
                order.ExchangeBy = CurrentUser.UserName;
                order.ExchangeTime = DateTime.Now;
            }

            await OrderRepository.UpdateAsync(order);
            
            var currentUserId = CurrentUser.Id ?? Guid.Empty;
            var currentUserName = CurrentUser.UserName ?? "System";

            await OrderHistoryManager.AddOrderHistoryAsync(
                order.Id,
                "ReturnStatusChanged",
                new object[]
                {
                    L[oldReturnStatus.ToString()]?.Value,
                    L[order.ReturnStatus.ToString()]?.Value,
                    L[oldOrderStatus.ToString()]?.Value,
                    L[order.OrderStatus.ToString()]?.Value
                },
                currentUserId,
                currentUserName
            );

            if (orderReturnStatus == OrderReturnStatus.Succeeded && order.OrderStatus == OrderStatus.Exchange)
            {
                await OrderHistoryManager.AddOrderHistoryAsync(
                    order.Id,
                    "OrderExchanged",
                    new object[] { order.OrderNo },
                    currentUserId,
                    currentUserName
                );
            }
        }

        private async Task SendEmailAsync(Guid orderId)
        {
            // Use the available email method from EmailAppService
            // TODO: Implement proper order email sending logic
            throw new NotImplementedException("Order email sending needs proper implementation");
        }
    }
}