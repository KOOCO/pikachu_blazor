using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders
{
    /// <summary>
    /// Service responsible for order inventory management operations
    /// Extracted from OrderAppService to follow Single Responsibility Principle
    /// </summary>
    public class OrderInventoryService : ApplicationService, IOrderInventoryService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<OrderItem, Guid> _orderItemRepository;
        private readonly IItemDetailsRepository _itemDetailsRepository;
        private readonly ISetItemRepository _setItemRepository;
        private readonly IFreebieRepository _freebieRepository;

        public OrderInventoryService(
            IOrderRepository orderRepository,
            IRepository<OrderItem, Guid> orderItemRepository,
            IItemDetailsRepository itemDetailsRepository,
            ISetItemRepository setItemRepository,
            IFreebieRepository freebieRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _itemDetailsRepository = itemDetailsRepository;
            _setItemRepository = setItemRepository;
            _freebieRepository = freebieRepository;
        }

        public async Task UpdateOrderItemsAsync(Guid orderId, List<UpdateOrderItemDto> orderItems)
        {
            Logger.LogInformation($"Updating order items for order: {orderId}");
            
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new UserFriendlyException($"Order not found: {orderId}");
            }
            
            // Validate inventory availability first
            var isAvailable = await ValidateInventoryAvailabilityAsync(orderItems);
            if (!isAvailable)
            {
                throw new UserFriendlyException("Insufficient inventory for order modification");
            }
            
            foreach (var updateItem in orderItems)
            {
                var existingOrderItem = order.OrderItems.FirstOrDefault(x => x.Id == updateItem.Id);
                if (existingOrderItem != null)
                {
                    var quantityDifference = updateItem.Quantity - existingOrderItem.Quantity;
                    
                    if (quantityDifference != 0)
                    {
                        // Update the order item
                        existingOrderItem.Quantity = updateItem.Quantity;
                        existingOrderItem.ItemPrice = updateItem.ItemPrice;
                        existingOrderItem.TotalAmount = updateItem.TotalAmount;
                        
                        // Adjust inventory if quantity changed
                        if (quantityDifference > 0)
                        {
                            // Quantity increased - reserve more inventory
                            var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.Id == updateItem.Id);
                            if (details != null)
                            {
                                details.SaleableQuantity -= quantityDifference;
                                details.StockOnHand -= quantityDifference;
                                await _itemDetailsRepository.UpdateAsync(details);
                            }
                        }
                        else
                        {
                            // Quantity decreased - release inventory
                            var releaseQuantity = Math.Abs(quantityDifference);
                            var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.Id == updateItem.Id);
                            if (details != null)
                            {
                                details.SaleableQuantity += releaseQuantity;
                                details.StockOnHand += releaseQuantity;
                                await _itemDetailsRepository.UpdateAsync(details);
                            }
                        }
                        
                        Logger.LogInformation($"Updated order item {existingOrderItem.Spec}: quantity change {quantityDifference}");
                    }
                }
            }
            
            // Recalculate order totals
            order.TotalQuantity = order.OrderItems.Sum(x => x.Quantity);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task ReserveInventoryAsync(Guid orderId)
        {
            Logger.LogInformation($"Reserving inventory for order: {orderId}");
            
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new UserFriendlyException($"Order not found: {orderId}");
            }
            
            var insufficientItems = new List<string>();
            
            foreach (var orderItem in order.OrderItems)
            {
                // Reserve regular item inventory
                var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == orderItem.ItemId && x.ItemName == orderItem.Spec);
                if (details != null)
                {
                    if (details.SaleableQuantity < orderItem.Quantity)
                    {
                        insufficientItems.Add($"Item: {details.ItemName}, Requested: {orderItem.Quantity}, Available: {details.SaleableQuantity}");
                    }
                    else
                    {
                        details.SaleableQuantity -= orderItem.Quantity;
                        details.StockOnHand -= orderItem.Quantity;
                        await _itemDetailsRepository.UpdateAsync(details);
                        
                        Logger.LogInformation($"Reserved inventory for item {orderItem.Spec}: -{orderItem.Quantity}");
                    }
                }
                
                // Reserve freebie inventory
                if (orderItem.FreebieId != null)
                {
                    var freebie = await _freebieRepository.FirstOrDefaultAsync(x => x.Id == orderItem.FreebieId);
                    if (freebie != null && freebie.FreebieAmount > 0)
                    {
                        freebie.FreebieAmount -= 1;
                        await _freebieRepository.UpdateAsync(freebie);
                        
                        Logger.LogInformation($"Reserved freebie inventory for order item: -1");
                    }
                    else
                    {
                        insufficientItems.Add("贈品庫存不足");
                    }
                }
            }
            
            if (insufficientItems.Count > 0)
            {
                string errorMessage = string.Join("; ", insufficientItems);
                throw new UserFriendlyException("409", "以下商品庫存不足,請刷新後再試: " + errorMessage);
            }
        }

        public async Task ReleaseInventoryAsync(Guid orderId)
        {
            Logger.LogInformation($"Releasing inventory for order: {orderId}");
            
            // Release inventory is the same as restore inventory
            await RestoreInventoryAsync(orderId);
        }

        public async Task<bool> ValidateInventoryAvailabilityAsync(List<UpdateOrderItemDto> orderItems)
        {
            Logger.LogInformation($"Validating inventory availability for {orderItems.Count} items");
            
            var insufficientItems = new List<string>();
            
            foreach (var item in orderItems)
            {
                // Validate regular item details
                var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (details != null)
                {
                    if (details.SaleableQuantity < item.Quantity)
                    {
                        insufficientItems.Add($"Item: {details.ItemName}, Requested: {item.Quantity}, Available: {details.SaleableQuantity}");
                    }
                }
                
                // TODO: Add set item validation logic when SetItemId is available in UpdateOrderItemDto
                // TODO: Add freebie validation logic when FreebieId is available in UpdateOrderItemDto
            }
            
            if (insufficientItems.Count > 0)
            {
                string errorMessage = string.Join("; ", insufficientItems);
                Logger.LogWarning($"Insufficient inventory detected: {errorMessage}");
                return false;
            }
            
            return true;
        }

        public async Task RestoreInventoryAsync(Guid orderId)
        {
            Logger.LogInformation($"Restoring inventory for order: {orderId}");
            
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                Logger.LogWarning($"Order not found for inventory restoration: {orderId}");
                return;
            }
            
            foreach (var orderItem in order.OrderItems)
            {
                // Restore regular item inventory
                var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == orderItem.ItemId && x.ItemName == orderItem.Spec);
                if (details != null)
                {
                    details.SaleableQuantity += orderItem.Quantity;
                    details.StockOnHand += orderItem.Quantity;
                    await _itemDetailsRepository.UpdateAsync(details);
                    
                    Logger.LogInformation($"Restored inventory for item {orderItem.Spec}: +{orderItem.Quantity}");
                }
                
                // Restore freebie inventory
                if (orderItem.FreebieId != null)
                {
                    var freebie = await _freebieRepository.FirstOrDefaultAsync(x => x.Id == orderItem.FreebieId);
                    if (freebie != null)
                    {
                        freebie.FreebieAmount += orderItem.Quantity;
                        await _freebieRepository.UpdateAsync(freebie);
                        
                        Logger.LogInformation($"Restored freebie inventory for order item: +{orderItem.Quantity}");
                    }
                }
            }
        }

        public async Task AdjustInventoryForOrderModificationAsync(Guid orderId, List<UpdateOrderItemDto> newOrderItems)
        {
            Logger.LogInformation($"Adjusting inventory for order modification: {orderId}");
            
            var order = await _orderRepository.GetWithDetailsAsync(orderId);
            if (order == null)
            {
                throw new UserFriendlyException($"Order not found: {orderId}");
            }
            
            // First, restore all current inventory
            await RestoreInventoryAsync(orderId);
            
            // Then, reserve inventory for the new order items
            var insufficientItems = new List<string>();
            
            foreach (var newItem in newOrderItems)
            {
                var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.Id == newItem.Id);
                if (details != null)
                {
                    if (details.SaleableQuantity < newItem.Quantity)
                    {
                        insufficientItems.Add($"Item: {details.ItemName}, Requested: {newItem.Quantity}, Available: {details.SaleableQuantity}");
                    }
                    else
                    {
                        details.SaleableQuantity -= newItem.Quantity;
                        details.StockOnHand -= newItem.Quantity;
                        await _itemDetailsRepository.UpdateAsync(details);
                        
                        Logger.LogInformation($"Adjusted inventory for item {details.ItemName}: -{newItem.Quantity}");
                    }
                }
            }
            
            if (insufficientItems.Count > 0)
            {
                // If there's insufficient inventory, restore the original reservation
                await ReserveInventoryAsync(orderId);
                
                string errorMessage = string.Join("; ", insufficientItems);
                throw new UserFriendlyException("Insufficient inventory for order modification: " + errorMessage);
            }
        }
    }
}