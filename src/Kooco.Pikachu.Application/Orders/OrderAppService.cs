using Kooco.Pikachu.ElectronicInvoiceSettings;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.OrderDeliveries;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Volo.Abp.Validation.Localization;


namespace Kooco.Pikachu.Orders
{
    [RemoteService(IsEnabled = false)]
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderManager _orderManager;
        private readonly IDataFilter _dataFilter;
        private readonly IRepository<OrderDelivery, Guid> _orderDeliveryRepository;
        private readonly IGroupBuyRepository _groupBuyRepository;
        private readonly IEmailSender _emailSender;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly IRepository<OrderItem, Guid> _orderItemRepository;
        private readonly IStringLocalizer<PikachuResource> _l;
        private readonly IItemRepository _itemRepository;
        private readonly IItemDetailsRepository _itemDetailsRepository;
        private readonly IElectronicInvoiceAppService _electronicInvoiceAppService;
        private readonly IElectronicInvoiceSettingRepository _electronicInvoiceSettingRepository;
        private readonly IFreebieRepository _freebieRepository;
        public OrderAppService(
            IOrderRepository orderRepository,
            OrderManager orderManager,
            IDataFilter dataFilter,
            IGroupBuyRepository groupBuyRepository,
            IEmailSender emailSender,
            IRepository<PaymentGateway, Guid> paymentGatewayRepository,
            IStringEncryptionService stringEncryptionService,
            IStringLocalizer<PikachuResource> l,
            IRepository<OrderItem, Guid> orderItemRepository,
            IRepository<OrderDelivery, Guid> orderDeliveryRepository,
            IItemRepository itemRepository,
            IItemDetailsRepository itemDetailsRepository,
            IElectronicInvoiceAppService electronicInvoiceAppService,
            IFreebieRepository freebieRepository,
            IBackgroundJobManager backgroundJobManager,
            IElectronicInvoiceSettingRepository electronicInvoiceSettingRepository
            )
        {
            _orderRepository = orderRepository;
            _orderManager = orderManager;
            _dataFilter = dataFilter;
            _groupBuyRepository = groupBuyRepository;
            _emailSender = emailSender;
            _paymentGatewayRepository = paymentGatewayRepository;
            _stringEncryptionService = stringEncryptionService;
            _orderItemRepository = orderItemRepository;
            _orderDeliveryRepository = orderDeliveryRepository;
            _l = l;
            _itemRepository= itemRepository;
            _orderItemRepository= orderItemRepository;
            _itemDetailsRepository = itemDetailsRepository;
            _electronicInvoiceAppService = electronicInvoiceAppService;
            _freebieRepository = freebieRepository;
            _backgroundJobManager = backgroundJobManager;
            _electronicInvoiceSettingRepository= electronicInvoiceSettingRepository;
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
                        input.CarrierId,
                        input.TaxTitle,
                        input.IsAsSameBuyer,
                        input.RecipientName,
                        input.RecipientPhone,
                        input.RecipientEmail,
                        input.DeliveryMethod,
                        input.PostalCode,
                        input.City,

                        input.District,
                        input.Road,
                        input.AddressDetails,
                        input.Remarks,
                        input.ReceivingTime,
                        input.TotalQuantity,
                        input.TotalAmount,
                        input.ReturnStatus,
                        input.OrderType
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
                            item.Quantity,
                            item.SKU,
                            item.DeliveryTemperature,
                            item.DeliveryTemperatureCost
                            );
                        using (_dataFilter.Disable<IMultiTenant>())
                        {
                            //var orderItem = await _orderItemRepository.GetAsync(item.ItemId.Value);
                            var details = await _itemDetailsRepository.FirstOrDefaultAsync(x => x.ItemId == item.ItemId);
                            if (details != null)
                            {
                                details.SaleableQuantity = details.SaleableQuantity - item.Quantity;
                                await _itemDetailsRepository.UpdateAsync(details);
                            }

                            if (item.FreebieId != null)
                            {
                              var freebie=  await _freebieRepository.FirstOrDefaultAsync(x => x.Id == item.FreebieId);
                                freebie.FreebieAmount -= item.Quantity;
                                await _freebieRepository.UpdateAsync(freebie);
                            }
                            }
                    }
                }
                await _orderRepository.InsertAsync(order);
                await UnitOfWorkManager.Current.SaveChangesAsync();

                if (groupBuy?.IsEnterprise == true)
                {
                    await SendEmailAsync(order.Id);
                }

                return ObjectMapper.Map<Order, OrderDto>(order);
            }
        }

        public async Task<OrderDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Order, OrderDto>(await _orderRepository.GetAsync(id));
        }

        public async Task<OrderDto> MergeOrdersAsync(List<Guid> Ids)
        {
            decimal TotalAmount = 0;
            int TotalQuantity = 0;
            var ord = await _orderRepository.GetWithDetailsAsync(Ids[0]);
            GroupBuy groupBuy = new();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                groupBuy = await _groupBuyRepository.GetAsync(ord.GroupBuyId);
            }
            List<OrderItemsCreateDto> orderItems = new List<OrderItemsCreateDto>();
            using (CurrentTenant.Change(groupBuy?.TenantId))
            {
                foreach (var id in Ids)
                {

                    var order = await _orderRepository.GetWithDetailsAsync(id);
                    TotalAmount += order.TotalAmount;
                    TotalQuantity += order.TotalQuantity;
                    

                    foreach (var item in order.OrderItems)
                    {

                        if (orderItems.Any(x => x.ItemId == item.ItemId && x.FreebieId==null) || orderItems.Any(x => x.FreebieId == item.FreebieId&& x.ItemId==null))
                        {
                            if (item.ItemId != null)
                            {
                                var orderItem = orderItems.Where(x => x.ItemId == item.ItemId).FirstOrDefault();
                                orderItem.TotalAmount += item.TotalAmount;
                                orderItem.Quantity += item.Quantity;
                            }
                            else {
                                var orderItem = orderItems.Where(x => x.FreebieId == item.FreebieId).FirstOrDefault();
                                orderItem.TotalAmount += item.TotalAmount;
                                orderItem.Quantity += item.Quantity;

                            }



                        }
                        else {
                            OrderItemsCreateDto orderItem = new OrderItemsCreateDto();
                            orderItem.ItemId = item.ItemId;
                            orderItem.SetItemId = item.SetItemId;
                            orderItem.FreebieId = item.FreebieId;
                            orderItem.ItemType = item.ItemType;

                            orderItem.Spec = item.Spec;
                            orderItem.ItemPrice = item.ItemPrice;
                            orderItem.TotalAmount = item.TotalAmount;
                            orderItem.Quantity = item.Quantity;
                            orderItem.SKU = item.SKU;
                            orderItems.Add(orderItem);
                        }

                    }

                }
                var order1 = await _orderManager.CreateAsync(
                          ord.GroupBuyId,
                          ord.IsIndividual,
                          ord.CustomerName,
                          ord.CustomerPhone,
                          ord.CustomerEmail,
                          ord.PaymentMethod,
                          ord.InvoiceType,
                          ord.InvoiceNumber,
                          ord.UniformNumber,
                          ord.CarrierId,
                          ord.TaxTitle,
                          ord.IsAsSameBuyer,
                          ord.RecipientName,
                          ord.RecipientPhone,
                          ord.RecipientEmail,
                          ord.DeliveryMethod,
                          ord.PostalCode,
                          ord.City,
                          ord.District,
                          ord.Road,
                          ord.AddressDetails,
                          ord.Remarks,
                          ord.ReceivingTime,
                          TotalQuantity,
                          TotalAmount,
                          ord.ReturnStatus,
                          OrderType.NewMarge
                          );

                foreach (var item in orderItems)
                {
                    _orderManager.AddOrderItem(
                                   order1,
                                   item.ItemId,
                                   item.SetItemId,
                                   item.FreebieId,
                                   item.ItemType,
                                   order1.Id,
                                   item.Spec,
                                   item.ItemPrice,
                                   item.TotalAmount,
                                   item.Quantity,
                                   item.SKU,
                                   item.DeliveryTemperature,
                                   item.DeliveryTemperatureCost
                                   );

                }
                await _orderRepository.InsertAsync(order1);
                foreach (var id in Ids)
                {
                    var ord1 = await _orderRepository.GetAsync(id);
                    ord1.OrderType = OrderType.MargeToNew;
                    await _orderRepository.UpdateAsync(ord1, autoSave: true);
                }
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return ObjectMapper.Map<Order, OrderDto>(order1);
            }
        }

        public async Task<OrderDto> SplitOrderAsync(List<Guid> OrderItemIds, Guid OrderId)
        {
            Order newOrder= new Order();
            var ord = await _orderRepository.GetWithDetailsAsync(OrderId);
            decimal TotalAmount = ord.TotalAmount;
            int TotalQuantity = ord.TotalQuantity;
            GroupBuy groupBuy = new();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                groupBuy = await _groupBuyRepository.GetAsync(ord.GroupBuyId);
            }
            List<OrderItemsCreateDto> orderItems = new List<OrderItemsCreateDto>();
            using (CurrentTenant.Change(groupBuy?.TenantId))
            {
                foreach (var item in ord.OrderItems)
                {
                    if (OrderItemIds.Any(x => x == item.Id))
                    {
                        OrderItemsCreateDto orderItem = new OrderItemsCreateDto();
                        orderItem.ItemId = item.ItemId;
                        orderItem.SetItemId = item.SetItemId;
                        orderItem.FreebieId = item.FreebieId;
                        orderItem.ItemType = item.ItemType;

                        orderItem.Spec = item.Spec;
                        orderItem.ItemPrice = item.ItemPrice;
                        orderItem.TotalAmount = item.TotalAmount;
                        orderItem.Quantity = item.Quantity;
                        orderItem.SKU = item.SKU;

                        TotalAmount -= item.TotalAmount;
                        TotalQuantity -= item.Quantity;
                        await _orderItemRepository.DeleteAsync(item.Id);
                        var order1 = await _orderManager.CreateAsync(
                         ord.GroupBuyId,
                         ord.IsIndividual,
                         ord.CustomerName,
                         ord.CustomerPhone,
                         ord.CustomerEmail,
                         ord.PaymentMethod,
                         ord.InvoiceType,
                         ord.InvoiceNumber,
                         ord.UniformNumber,
                         ord.CarrierId,
                         ord.TaxTitle,
                         ord.IsAsSameBuyer,
                         ord.RecipientName,
                         ord.RecipientPhone,
                         ord.RecipientEmail,
                         ord.DeliveryMethod,
                         ord.PostalCode,
                         ord.City,
                         ord.District,
                         ord.Road,
                         ord.AddressDetails,
                         ord.Remarks,
                         ord.ReceivingTime,
                         orderItem.Quantity,
                         orderItem.TotalAmount,
                         ord.ReturnStatus,
                         OrderType.NewSplit,
                         ord.Id
                         );
                        order1.ShippingStatus = ord.ShippingStatus;

                        _orderManager.AddOrderItem(
                                       order1,
                                       orderItem.ItemId,
                                       orderItem.SetItemId,
                                       orderItem.FreebieId,
                                       orderItem.ItemType,
                                       order1.Id,
                                       orderItem.Spec,
                                       orderItem.ItemPrice,
                                       orderItem.TotalAmount,
                                       orderItem.Quantity,
                                       orderItem.SKU, orderItem.DeliveryTemperature,
                                       orderItem.DeliveryTemperatureCost

                                       );
                        await _orderRepository.InsertAsync(order1);
                        await UnitOfWorkManager.Current.SaveChangesAsync();
                        newOrder = order1;
                        if (order1.ShippingStatus == ShippingStatus.PrepareShipment)
                        {
                            if (orderItem.Item?.IsFreeShipping == true)
                            {
                                if (orderItem.DeliveryTemperature == ItemStorageTemperature.Normal && orderItem.Item != null || orderItem.Item?.IsFreeShipping == true)
                                {
                                    var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order1.Id);
                                    OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                    order1.UpdateOrderItem(order1.OrderItems.Where(x => (x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == true)).ToList(), OrderDelivery.Id);
                                }
                                if (orderItem.DeliveryTemperature == ItemStorageTemperature.Freeze && orderItem.Item?.IsFreeShipping == true)
                                {
                                    var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order1.Id);
                                    OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                    order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                                }
                                if (orderItem.DeliveryTemperature == ItemStorageTemperature.Frozen && orderItem.Item?.IsFreeShipping == true)
                                {
                                    var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order1.Id);
                                    OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                    order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x?.Item.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                                }
                            }
                            if (orderItem.Item?.IsFreeShipping == false)
                            {
                                if (orderItem.DeliveryTemperature == ItemStorageTemperature.Normal && orderItem.Item?.IsFreeShipping == false)
                                {
                                    var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order1.Id);
                                    OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                    order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                                }
                                if (orderItem.DeliveryTemperature == ItemStorageTemperature.Freeze && orderItem.Item?.IsFreeShipping == false)
                                {
                                    var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order1.Id);
                                    OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                    order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                                }
                                if (orderItem.DeliveryTemperature == ItemStorageTemperature.Frozen && orderItem.Item?.IsFreeShipping == false)
                                {
                                    var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order1.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order1.Id);
                                    OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                                    order1.UpdateOrderItem(order1.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                                }

                            }
                            await _orderRepository.UpdateAsync(order1);
                        }
                    }

                }
                if (ord.OrderItems.Count <=0)
                {
                    //var newOrder = await _orderRepository.GetAsync(newOrderId);
                    newOrder.TotalAmount += TotalAmount;
                    await _orderRepository.UpdateAsync(newOrder);
                    ord.TotalAmount = 0;
                    ord.TotalQuantity = 0;
                }
                else {
                    ord.TotalAmount = TotalAmount;
                    ord.TotalQuantity = TotalQuantity;
                }
                
                ord.OrderType = OrderType.SplitToNew;
                await _orderRepository.UpdateAsync(ord);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                return ObjectMapper.Map<Order, OrderDto>(ord);
            }
        }

        public async Task<OrderDto> GetWithDetailsAsync(Guid id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input, bool hideCredentials = false)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.CountAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate, input.OrderStatus);

            var items = await _orderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate, input.OrderStatus);

            try
            {
                var dtos = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

                if (hideCredentials)
                {
                    var groupbuy = await _groupBuyRepository.GetAsync(input.GroupBuyId.Value);
                    if (groupbuy.ProtectPrivacyData)
                    {
                        dtos.HideCredentials();
                    }
                }

                return new PagedResultDto<OrderDto>
                {
                    TotalCount = totalCount,
                    Items = dtos
                };
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<PagedResultDto<OrderDto>> GetTenantOrderListAsync(GetOrderListDto input)
        {
            using (_dataFilter.Disable<IMultiTenant>())
            {
                if (input.Sorting.IsNullOrEmpty())
                {
                    input.Sorting = $"{nameof(Order.CreationTime)} desc";
                }

                var totalCount = await _orderRepository.CountAsync(input.Filter, input.GroupBuyId);

                var items = await _orderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);

                return new PagedResultDto<OrderDto>
                {
                    TotalCount = totalCount,
                    Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
                };
            }
        }

        public async Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.ReturnOrderCountAsync(input.Filter, input.GroupBuyId);

            var items = await _orderRepository.GetReturnListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }

        public async Task<PagedResultDto<OrderDto>> GetReconciliationListAsync(GetOrderListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.CountReconciliationAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate);

            var items = await _orderRepository.GetReconciliationListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
        }
        public async Task<PagedResultDto<OrderDto>> GetVoidListAsync(GetOrderListDto input)
        {
            if (input.Sorting.IsNullOrEmpty())
            {
                input.Sorting = $"{nameof(Order.CreationTime)} desc";
            }

            var totalCount = await _orderRepository.CountVoidAsync(input.Filter, input.GroupBuyId, input.StartDate, input.EndDate);

            var items = await _orderRepository.GetVoidListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds, input.StartDate, input.EndDate);

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
            order.PostalCode = input.PostalCode;
            order.District = input.District;
            order.City = input.City;
            order.Road = input.Road;
            order.AddressDetails = input.AddressDetails;
            order.OrderStatus = input.OrderStatus;
            await _orderRepository.UpdateAsync(order);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }

        public async Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus)
        {
            var order = await _orderRepository.GetAsync(id);
            order.ReturnStatus = orderReturnStatus;
            if (orderReturnStatus == OrderReturnStatus.Reject)
            {
                order.OrderStatus = OrderStatus.Open;

            }
            await _orderRepository.UpdateAsync(order);
        }

        public async Task ExchangeOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetAsync(id);
            order.ReturnStatus = OrderReturnStatus.WaitingForApprove;
            order.OrderStatus = OrderStatus.Returned;

            await _orderRepository.UpdateAsync(order);


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
        public async Task CancelOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            order.OrderStatus = OrderStatus.Closed;
            order.CancellationDate = DateTime.Now;
            await _orderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id, OrderStatus.Closed);
        }
        public async Task VoidInvoice(Guid id, string reason)
        {
            var order = await _orderRepository.GetAsync(id);
            order.IsVoidInvoice = true;
            order.VoidReason = reason;
            order.VoidUser = CurrentUser.Name;
            order.VoidDate = DateTime.Now;
            order.InvoiceStatus = InvoiceStatus.InvoiceVoided;
            order.LastModificationTime = DateTime.Now;
            order.LastModifierId = CurrentUser.Id;
            await _orderRepository.UpdateAsync(order);
            await _electronicInvoiceAppService.CreateVoidInvoiceAsync(id, reason);
        
        }
        public async Task CreditNoteInvoice(Guid id, string reason)
        {
            var order = await _orderRepository.GetAsync(id);
          
           // order.IsVoidInvoice = true;
            order.CreditNoteReason = reason;
            order.CreditNoteUser = CurrentUser.Name;
            order.CreditNoteDate = DateTime.Now;
            order.InvoiceStatus = InvoiceStatus.CreditNote;
          
            await _orderRepository.UpdateAsync(order);
            await _electronicInvoiceAppService.CreateCreditNoteAsync(id);

        }
        public async Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            order.DeliveryMethod = input.DeliveryMethod;
            order.ShippingNumber = input.ShippingNumber;
            order.ShippingStatus = ShippingStatus.Shipped;
            order.ShippedBy = CurrentUser.Name;
            order.ShippingDate = DateTime.Now;

            await _orderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        public async Task<OrderDto> OrderShipped(Guid id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
           
            order.ShippingStatus = ShippingStatus.Shipped;
            order.ShippedBy = CurrentUser.Name;
            order.ShippingDate = DateTime.Now;
            order.IssueStatus = IssueInvoiceStatus.SentToBackStage;
            await _orderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            var invoiceSetting = await _electronicInvoiceSettingRepository.FirstOrDefaultAsync();
            var invoiceDely = invoiceSetting.DaysAfterShipmentGenerateInvoice;
            var delay = DateTime.Now.AddMinutes(1)-DateTime.Now;
            GenerateInvoiceBackgroundJobArgs args = new GenerateInvoiceBackgroundJobArgs { OrderId = order.Id };
          var jobid=  await _backgroundJobManager.EnqueueAsync(args, BackgroundJobPriority.High, delay);
           // await _electronicInvoiceAppService.CreateInvoiceAsync(order.Id);
            await SendEmailAsync(order.Id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        public async Task<OrderDto> OrderClosed(Guid id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);

            order.ShippingStatus = ShippingStatus.Closed;
            order.ClosedBy = CurrentUser.Name;
            order.CancellationDate = DateTime.Now;

            await _orderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        public async Task<OrderDto> OrderComplete(Guid id)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);

            order.ShippingStatus = ShippingStatus.Completed;
            order.CompletedBy = CurrentUser.Name;
            order.CompletionTime = DateTime.Now;

            await _orderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        private async Task SendEmailAsync(Guid id, OrderStatus? orderStatus = null)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await _groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);

            string status = orderStatus == null ? _l[order.ShippingStatus.ToString()] : _l[orderStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

            string body = File.ReadAllText("wwwroot/EmailTemplates/email.html");
            DateTime creationTime = order.CreationTime;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
            DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(creationTime, tz);
            string formattedTime = creationTimeInTimeZone.ToString("yyyy-MM-dd HH:mm:ss");

            body = body.Replace("{{Greetings}}", "");
            body = body.Replace("{{Footer}}", "");
            if (order.ShippingStatus == ShippingStatus.WaitingForPayment)
            {
                body = body.Replace("{{NotifyMessage}}", groupbuy.NotifyMessage);
            }
            else {
                body = body.Replace("{{NotifyMessage}}", "");
            }
            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNo}}", order.OrderNo);
            body = body.Replace("{{OrderDate}}", formattedTime);
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            if (!groupbuy.IsEnterprise)
            {
                body = body.Replace("{{PaymentMethod}}", _l[order.PaymentMethod.ToString()]);
            }
            body = body.Replace("{{PaymentStatus}}", _l[order.OrderStatus.ToString()]);
            body = body.Replace("{{ShippingMethod}}", $"{_l[order.DeliveryMethod.ToString()]} {order.ShippingNumber}");
            body = body.Replace("{{DeliveryFee}}", "0");
            body = body.Replace("{{RecipientAddress}}", order.AddressDetails);
            body = body.Replace("{{ShippingStatus}}", _l[order.ShippingStatus.ToString()]);
            body = body.Replace("{{RecipientComments}}", order.Remarks);

            if (order.OrderItems != null)
            {
                StringBuilder sb = new();
                foreach (var item in order.OrderItems)
                {
                    string itemName = "";
                    if (item.ItemType == ItemType.Item)
                    {
                        itemName = item.Item?.ItemName;
                    }
                    else if (item.ItemType == ItemType.SetItem)
                    {
                        itemName = item.SetItem?.SetItemName;
                    }
                    else
                    {
                        itemName = item.Freebie?.ItemName;
                    }

                    itemName += $" {item.ItemPrice:N0} x {item.Quantity}";
                    sb.Append(
                        $@"
                    <tr>
                        <td>{itemName}</td>
                        <td>${item.ItemPrice:N0}</td>
                        <td>{item.Quantity}</td>
                        <td>${item.TotalAmount:N0}</td>
                    </tr>"
                    );
                }

                body = body.Replace("{{OrderItems}}", sb.ToString());
            }

            body = body.Replace("{{DeliveryFee}}", "$0");
            body = body.Replace("{{TotalAmount}}", $"${order.TotalAmount:N0}");

            await _emailSender.SendAsync(order.CustomerEmail, subject, body);
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
                Order order = new Order();
                using (_dataFilter.Disable<IMultiTenant>())
                {
                    if (paymentResult.OrderId == null)
                    {
                        order = await _orderRepository
                                       .FirstOrDefaultAsync(o => o.OrderNo == paymentResult.MerchantTradeNo)
                                       ?? throw new EntityNotFoundException();

                        order = await _orderRepository.GetWithDetailsAsync(order.Id);

                        if (paymentResult.CustomField1 != order.CheckMacValue)
                        {
                            throw new Exception();
                        }

                        if (paymentResult.TradeAmt != order.TotalAmount)
                        {
                            throw new Exception();
                        }
                    }
                    else {
                        order = await _orderRepository
                                          .FirstOrDefaultAsync(o => o.Id == paymentResult.OrderId)
                                          ?? throw new EntityNotFoundException();
                        order = await _orderRepository.GetWithDetailsAsync(order.Id);
                    }
                    order.ShippingStatus = ShippingStatus.PrepareShipment;
                    order.PrepareShipmentBy = CurrentUser.Name ?? "System";
                    _ = DateTime.TryParse(paymentResult.PaymentDate, out DateTime parsedDate);
                    order.PaymentDate =paymentResult.OrderId==null? parsedDate:DateTime.Now;
                    if (order.OrderItems.Any(x => x.Item?.IsFreeShipping==true))
                    {
                        if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal&&(x.Item!=null|| x.Item?.IsFreeShipping == true)).Any())
                        {
                            var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                            OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                            order.UpdateOrderItem(order.OrderItems.Where(x => (x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping==true)).ToList(), OrderDelivery.Id);
                        }
                        if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).Any())
                        {
                            var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                            OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                            order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                        }
                        if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == true).Any())
                        {
                            var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                            OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                            order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x?.Item.IsFreeShipping == true).ToList(), OrderDelivery.Id);
                        }
                    }
                    if (order.OrderItems.Any(x => x.Item?.IsFreeShipping == false))
                    {
                        if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).Any())
                        {
                            var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                            OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                            order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Normal && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                        }
                        if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).Any())
                        {
                            var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                            OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                            order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Freeze && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                        }
                        if (order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).Any())
                        {
                            var OrderDelivery = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                            OrderDelivery = await _orderDeliveryRepository.InsertAsync(OrderDelivery);
                            order.UpdateOrderItem(order.OrderItems.Where(x => x.DeliveryTemperature == ItemStorageTemperature.Frozen && x.Item?.IsFreeShipping == false).ToList(), OrderDelivery.Id);
                        }

                    }
                    //if (order.OrderItems.Any(x => x.FreebieId != null))
                    //{
                    //    var OrderDelivery1 = new OrderDelivery(Guid.NewGuid(), order.DeliveryMethod.Value, DeliveryStatus.Proccesing, null, "", order.Id);
                    //    OrderDelivery1 = await _orderDeliveryRepository.InsertAsync(OrderDelivery1);
                    //    order.UpdateOrderItem(order.OrderItems.Where(x => x.FreebieId != null).ToList(), OrderDelivery1.Id);
                    //}
            await _orderRepository.UpdateAsync(order);
                    await UnitOfWorkManager.Current.SaveChangesAsync();
                    await SendEmailAsync(order.Id);
                }
            }
        }

        [AllowAnonymous]
        public async Task<PaymentGatewayDto> GetPaymentGatewayConfigurationsAsync(Guid id)
        {
            GroupBuy groupBuy = new();
            using (_dataFilter.Disable<IMultiTenant>())
            {
                groupBuy = await _groupBuyRepository.GetAsync(id);
            }
            using (CurrentTenant.Change(groupBuy.TenantId))
            {
                var paymentGateway = await _paymentGatewayRepository.FirstOrDefaultAsync(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);
                var paymentGatewayDto = ObjectMapper.Map<PaymentGateway, PaymentGatewayDto>(paymentGateway);

                var properties = typeof(PaymentGatewayDto).GetProperties();
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(string))
                    {
                        var value = (string?)property.GetValue(paymentGatewayDto);
                        if (!string.IsNullOrEmpty(value))
                        {
                            var decryptedValue = _stringEncryptionService.Decrypt(value);
                            property.SetValue(paymentGatewayDto, decryptedValue);
                        }
                    }
                }

                return paymentGatewayDto;
            }
        }

        public async Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input)
        {
            var items = await _orderRepository.GetListAsync(0, int.MaxValue, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);
            var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

            // Create a dictionary for localized headers
            var headers = new Dictionary<string, string>
            {
                { "OrderNumber", _l["OrderNo"] },
                { "OrderDate", _l["OrderDate"] },
                { "CustomerName", _l["CustomerName"] },
                { "Email", _l["Email"] },
                { "RecipientInformation", _l["RecipientInformation"] },
                { "ShippingMethod", _l["ShippingMethod"] },
                { "Address", _l["Address"] },
                { "Notes", _l["Notes"] },
                { "MerchantNotes", _l["MerchantNotes"] },
                { "OrderedItems", _l["OrderedItems"] },
                { "InvoiceStatus", _l["InvoiceStatus"] },
                { "ShippingStatus", _l["ShippingStatus"] },
                { "PaymentMethod", _l["PaymentMethod"] },
                { "CheckoutAmount", _l["CheckoutAmount"] }
            };

            var excelContent = Results.Select(x => new Dictionary<string, object>
            {
                { headers["OrderNumber"], x.OrderNo },
                { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
                { headers["CustomerName"], x.CustomerName },
                { headers["Email"], x.CustomerEmail },
                { headers["RecipientInformation"], x.RecipientName + "/" + x.RecipientPhone },
                { headers["ShippingMethod"], _l[x.DeliveryMethod.ToString()] },
                { headers["Address"], x.AddressDetails },
                { headers["Notes"], x.Remarks },
                { headers["MerchantNotes"], x.Remarks },
                { headers["OrderedItems"], string.Join(", ", x.OrderItems.Select(item =>
                    (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
                    (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
                    (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
                )},
                { headers["InvoiceStatus"], _l[x.InvoiceStatus.ToString()] },
                { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
                { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
                { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") }
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(excelContent);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public async Task<IRemoteStreamContent> GetReconciliationListAsExcelFileAsync(GetOrderListDto input)
        {
            var items = await _orderRepository.GetReconciliationListAsync(0, int.MaxValue, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);
            var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);

            // Create a dictionary for localized headers
            var headers = new Dictionary<string, string>
            {
                { "OrderNumber", _l["OrderNo"] },
                { "OrderDate", _l["OrderDate"] },
                { "CustomerName", _l["CustomerName"] },
                { "Email", _l["Email"] },
                { "RecipientInformation", _l["RecipientInformation"] },
                { "ShippingMethod", _l["ShippingMethod"] },
                { "Address", _l["Address"] },
                { "Notes", _l["Notes"] },
                { "MerchantNotes", _l["MerchantNotes"] },
                { "OrderedItems", _l["OrderedItems"] },
                { "InvoiceStatus", _l["InvoiceStatus"] },
                { "ShippingStatus", _l["ShippingStatus"] },
                { "PaymentMethod", _l["PaymentMethod"] },
                { "CheckoutAmount", _l["CheckoutAmount"] },
                { "DeliveryMethod", _l["DeliveryMethod"] }
            };

            var excelContent = Results.Select(x => new Dictionary<string, object>
            {
                { headers["OrderNumber"], x.OrderNo },
                { headers["OrderDate"], x.CreationTime.ToString("MM/d/yyyy h:mm:ss tt") },
                { headers["CustomerName"], x.CustomerName },
                { headers["Email"], x.CustomerEmail },
                { headers["RecipientInformation"], x.RecipientName + "/" + x.RecipientPhone },
                { headers["ShippingMethod"], _l[x.DeliveryMethod.ToString()] },
                { headers["Address"], x.AddressDetails },
                { headers["Notes"], x.Remarks },
                { headers["MerchantNotes"], x.Remarks },
                { headers["OrderedItems"], string.Join(", ", x.OrderItems.Select(item =>
                    (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
                    (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
                    (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
                )},
                { headers["InvoiceStatus"], _l[x.InvoiceStatus.ToString()] },
                { headers["ShippingStatus"], _l[x.ShippingStatus.ToString()] },
                { headers["PaymentMethod"], _l[x.PaymentMethod.ToString()] },
                { headers["CheckoutAmount"], "$ " + x.TotalAmount.ToString("N2") },
                { headers["DeliveryMethod"], _l[x.DeliveryMethod.ToString()] }
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(excelContent);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new RemoteStreamContent(memoryStream, "Reconciliation Report.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}

