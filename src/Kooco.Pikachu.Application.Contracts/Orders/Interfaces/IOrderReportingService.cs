using Kooco.Pikachu.EnumValues;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for Order reporting and analytics operations
    /// Split from IOrderAppService to follow Interface Segregation Principle
    /// </summary>
    public interface IOrderReportingService : IApplicationService
    {
        /// <summary>
        /// Export order list to Excel
        /// </summary>
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input);

        /// <summary>
        /// Get return orders list
        /// </summary>
        Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input);

        /// <summary>
        /// Get tenant order list
        /// </summary>
        Task<PagedResultDto<OrderDto>> GetTenantOrderListAsync(GetOrderListDto input);

        /// <summary>
        /// Get reconciliation list
        /// </summary>
        Task<PagedResultDto<OrderDto>> GetReconciliationListAsync(GetOrderListDto input);

        /// <summary>
        /// Export reconciliation list to Excel
        /// </summary>
        Task<IRemoteStreamContent> GetReconciliationListAsExcelFileAsync(GetOrderListDto input);

        /// <summary>
        /// Get void orders list
        /// </summary>
        Task<PagedResultDto<OrderDto>> GetVoidListAsync(GetOrderListDto input);

        /// <summary>
        /// Get report list for group buy orders
        /// </summary>
        Task<PagedResultDto<GroupBuyReportOrderDto>> GetReportListAsync(GetOrderListDto input, bool hideCredentials = false);

        /// <summary>
        /// Get total delivery temperature counts
        /// </summary>
        Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync();

        /// <summary>
        /// Get order status amounts for user
        /// </summary>
        Task<(decimal PaidAmount, decimal UnpaidAmount, decimal RefundedAmount)> GetOrderStatusAmountsAsync(Guid userId);

        /// <summary>
        /// Get order status counts for user
        /// </summary>
        Task<(int Open, int Exchange, int Return)> GetOrderStatusCountsAsync(Guid userId);

        /// <summary>
        /// Get order history logs
        /// </summary>
        Task<List<OrderHistoryDto>> GetOrderLogsAsync(Guid orderId);

        /// <summary>
        /// Get return order notification count
        /// </summary>
        Task<long> GetReturnOrderNotificationCount();
    }
}