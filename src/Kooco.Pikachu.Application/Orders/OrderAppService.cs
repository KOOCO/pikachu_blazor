using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;

namespace Kooco.Pikachu.Orders
{
    [Authorize(PikachuPermissions.Orders.Default)]
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderManager _orderManager;
        private readonly IDataFilter _dataFilter;
        private readonly IGroupBuyRepository _groupBuyRepository;
        private readonly IConfiguration _configuration;
        private readonly ISettingManager _settingManager;
        private readonly IEmailSender _emailSender;

        public OrderAppService(
            IOrderRepository orderRepository,
            OrderManager orderManager,
            IDataFilter dataFilter,
            IGroupBuyRepository groupBuyRepository,
            IConfiguration configuration,
            ISettingManager settingManager,
            IEmailSender emailSender
            )
        {
            _orderRepository = orderRepository;
            _orderManager = orderManager;
            _dataFilter = dataFilter;
            _groupBuyRepository = groupBuyRepository;
            _configuration = configuration;
            _settingManager = settingManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        public async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            GroupBuy groupBuy = new();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                groupBuy = await _groupBuyRepository.GetAsync(input.GroupBuyId);
            }

            using (CurrentTenant.Change(groupBuy?.TenantId))
            {
                var order = await _orderManager.CreateAsync(
                        input.GroupBuyId,
                        input.IsIndividual,
                        input.CustomerName,
                        input.CustomerPhone,
                        input.CustomerEmail,
                        input.PaymentMethod,
                        input.InvoiceType,
                        input.InvoiceNumber,
                        input.UniformNumber,
                        input.IsAsSameBuyer,
                        input.RecipientName,
                        input.RecipientPhone,
                        input.RecipientEmail,
                        input.DeliveryMethod,
                        input.City,
                        input.District,
                        input.Road,
                        input.AddressDetails,
                        input.Remarks,
                        input.ReceivingTime,
                        input.TotalQuantity,
                        input.TotalAmount
                        );

                if (input.OrderItems != null)
                {
                    foreach (var item in input.OrderItems)
                    {
                        _orderManager.AddOrderItem(
                            order,
                            item.ItemId,
                            item.SetItemId,
                            item.FreebieId,
                            item.ItemType,
                            item.OrderId,
                            item.Spec,
                            item.ItemPrice,
                            item.TotalAmount,
                            item.Quantity
                            );
                    }
                }
                await _orderRepository.InsertAsync(order);
                return ObjectMapper.Map<Order, OrderDto>(order);
            }
        }

        public async Task<OrderDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Order, OrderDto>(await _orderRepository.GetAsync(id));
        }

        public async Task<OrderDto> GetWithDetailsAsync(Guid id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.CountAsync(input.Filter);

            var items = await _orderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }

        [Authorize(PikachuPermissions.Orders.AddStoreComment)]
        public async Task AddStoreCommentAsync(Guid id, string comment)
        {
            var order = await _orderRepository.GetAsync(id);
            await _orderRepository.EnsureCollectionLoadedAsync(order, o => o.StoreComments);
            _orderManager.AddStoreComment(order, comment);
        }

        [Authorize(PikachuPermissions.Orders.AddStoreComment)]
        public async Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment)
        {
            var order = await _orderRepository.GetAsync(id);
            await _orderRepository.EnsureCollectionLoadedAsync(order, o => o.StoreComments);
            var storeComment = order.StoreComments.First(c => c.Id == commentId);
            if (storeComment.CreatorId != CurrentUser.Id)
            {
                throw new UnauthorizedAccessException();
            }
            storeComment.Comment = comment;
        }

        public async Task<OrderDto> UpdateAsync(Guid id, CreateOrderDto input)
        {
            var order = await _orderRepository.GetAsync(id);
            order.RecipientName = input.RecipientName;
            order.RecipientPhone = input.RecipientPhone;
            order.District = input.District;
            order.City = input.City;
            order.Road = input.Road;
            order.AddressDetails = input.AddressDetails;
            order.OrderStatus = input.OrderStatus;
            await _orderRepository.UpdateAsync(order);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems)
        {
            var order = await _orderRepository.GetAsync(id);
            await _orderRepository.EnsureCollectionLoadedAsync(order, o => o.OrderItems);

            foreach (var item in orderItems)
            {
                var orderItem = order.OrderItems.First(o => o.Id == item.Id);
                orderItem.Quantity = item.Quantity;
                orderItem.ItemPrice = item.ItemPrice;
                orderItem.TotalAmount = item.TotalAmount;
            }

            order.TotalQuantity = order.OrderItems.Sum(o => o.Quantity);
            order.TotalAmount = order.OrderItems.Sum(o => o.TotalAmount);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
        {
            var order = await _orderRepository.GetAsync(id);
            order.DeliveryMethod = input.DeliveryMethod;
            order.ShippingNumber = input.ShippingNumber;
            await _orderRepository.UpdateAsync(order);
            
            var groupBuy = await _groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            var notifyMessage = groupBuy.NotifyMessage;
            
            await _emailSender.SendAsync(
                order.CustomerEmail,
                "Order has been shipped",
                notifyMessage
                );

            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        [AllowAnonymous]
        public async Task AddCheckMacValueAsync(Guid id, string checkMacValue)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                var order = await _orderRepository.GetAsync(id);
                order.CheckMacValue = checkMacValue;
                await _orderRepository.UpdateAsync(order);
            }
        }

        [AllowAnonymous]
        public async Task HandlePaymentAsync(PaymentResult paymentResult)
        {
            if (paymentResult.SimulatePaid == 0)
            {
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    var order = await _orderRepository
                                    .FirstOrDefaultAsync(o => o.OrderNo == paymentResult.MerchantTradeNo)
                                    ?? throw new EntityNotFoundException();

                    if (paymentResult.CustomField1 != order.CheckMacValue)
                    {
                        throw new Exception();
                    }
                    if (paymentResult.TradeAmt != order.TotalAmount)
                    {
                        throw new Exception();
                    }
                    order.ShippingStatus = ShippingStatus.PrepareShipment;
                    _ = DateTime.TryParse(paymentResult.PaymentDate, out DateTime parsedDate);
                    order.PaymentDate = parsedDate;

                    await _orderRepository.UpdateAsync(order);
                }
            }
        }
    }
}

