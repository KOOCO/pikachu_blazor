using Kooco.Pikachu.OrderItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.Orders.Interfaces
{
    /// <summary>
    /// Service responsible for order inventory management operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public interface IOrderInventoryService : IApplicationService
    {
        /// <summary>
        /// Update order items quantities and inventory
        /// </summary>
        Task UpdateOrderItemsAsync(Guid orderId, List<UpdateOrderItemDto> orderItems);

        /// <summary>
        /// Reserve inventory for order items
        /// </summary>
        Task ReserveInventoryAsync(Guid orderId);

        /// <summary>
        /// Release reserved inventory for order items
        /// </summary>
        Task ReleaseInventoryAsync(Guid orderId);

        /// <summary>
        /// Validate inventory availability for order items
        /// </summary>
        Task<bool> ValidateInventoryAvailabilityAsync(List<UpdateOrderItemDto> orderItems);

        /// <summary>
        /// Restore inventory when order is cancelled or expired
        /// </summary>
        Task RestoreInventoryAsync(Guid orderId);

        /// <summary>
        /// Adjust inventory for order modifications
        /// </summary>
        Task AdjustInventoryForOrderModificationAsync(Guid orderId, List<UpdateOrderItemDto> newOrderItems);
    }
}