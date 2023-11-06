using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Localization;
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
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Encryption;
using Kooco.Pikachu.TenantEmailing;
using System.Net.Mail;
using Volo.Abp.SettingManagement;

namespace Kooco.Pikachu.Orders
{
    //[Authorize(PikachuPermissions.Orders.Default)]
    public class OrderAppService : ApplicationService, IOrderAppService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderManager _orderManager;
        private readonly IDataFilter _dataFilter;
        private readonly IGroupBuyRepository _groupBuyRepository;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
        private readonly IStringEncryptionService _stringEncryptionService;
        private readonly IRepository<TenantEmailSettings, Guid> _tenantEmailSettingsRepository;
        private readonly ISettingManager _settingManager;
        private readonly IStringLocalizer<PikachuResource> _l;
        public OrderAppService(
            IOrderRepository orderRepository,
            OrderManager orderManager,
            IDataFilter dataFilter,
            IGroupBuyRepository groupBuyRepository,
            IEmailSender emailSender,
            IRepository<PaymentGateway, Guid> paymentGatewayRepository,
            IStringEncryptionService stringEncryptionService,
            IRepository<TenantEmailSettings, Guid> tenantEmailSettingsRepository,
            ISettingManager settingManager,
            IStringLocalizer<PikachuResource> l
            )
        {
            _orderRepository = orderRepository;
            _orderManager = orderManager;
            _dataFilter = dataFilter;
            _groupBuyRepository = groupBuyRepository;
            _emailSender = emailSender;
            _paymentGatewayRepository = paymentGatewayRepository;
            _stringEncryptionService = stringEncryptionService;
            _tenantEmailSettingsRepository = tenantEmailSettingsRepository;
            _settingManager = settingManager;
            _l = l;
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
                        input.TotalAmount,
                        input.ReturnStatus
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
                            item.SKU
                            );
                    }
                }
                await _orderRepository.InsertAsync(order);
                await UnitOfWorkManager.Current.SaveChangesAsync();
                await SendEmailAsync(order.Id);

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

            var totalCount = await _orderRepository.CountAsync(input.Filter, input.GroupBuyId);

            var items = await _orderRepository.GetListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, input.Filter, input.GroupBuyId, input.OrderIds);

            return new PagedResultDto<OrderDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Order>, List<OrderDto>>(items)
            };
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

        public async Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            order.DeliveryMethod = input.DeliveryMethod;
            order.ShippingNumber = input.ShippingNumber;
            order.ShippingStatus = ShippingStatus.Shipped;
            order.ShippingDate = DateTime.Now;

            await _orderRepository.UpdateAsync(order);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            await SendEmailAsync(order.Id);
            return ObjectMapper.Map<Order, OrderDto>(order);
        }
        private async Task SendEmailAsync(Guid id, OrderStatus? orderStatus = null)
        {
            var order = await _orderRepository.GetWithDetailsAsync(id);
            var groupbuy = await _groupBuyRepository.GetAsync(g => g.Id == order.GroupBuyId);
            var emailSettings = await _tenantEmailSettingsRepository.FirstOrDefaultAsync();

            string status = orderStatus == null ? _l[order.ShippingStatus.ToString()] : _l[orderStatus.ToString()];

            string subject = $"{groupbuy.GroupBuyName} 訂單#{order.OrderNo} {status}";

            if (emailSettings != null && !string.IsNullOrEmpty(emailSettings.Subject))
            {
                subject = emailSettings.Subject;
            }

            string body = File.ReadAllText("wwwroot/EmailTemplates/email.html");
            DateTime creationTime = order.CreationTime;
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"); // UTC+8
            DateTimeOffset creationTimeInTimeZone = TimeZoneInfo.ConvertTime(creationTime, tz);
            string formattedTime = creationTimeInTimeZone.ToString("yyyy-MM-dd HH:mm:ss");
            
            if(emailSettings != null)
            {
                if (!string.IsNullOrEmpty(emailSettings.Greetings))
                {
                    body = body.Replace("{{Greetings}}", emailSettings.Greetings);
                }
                else
                {
                    body = body.Replace("{{Greetings}}", "");
                }

                if (!string.IsNullOrEmpty(emailSettings.Footer))
                {
                    body = body.Replace("{{Footer}}", emailSettings.Footer);
                }
            }

            body = body.Replace("{{NotifyMessage}}", groupbuy.NotifyMessage);
            body = body.Replace("{{GroupBuyName}}", groupbuy.GroupBuyName);
            body = body.Replace("{{OrderNo}}", order.OrderNo);
            body = body.Replace("{{OrderDate}}", formattedTime);
            body = body.Replace("{{CustomerName}}", order.CustomerName);
            body = body.Replace("{{CustomerEmail}}", order.CustomerEmail);
            body = body.Replace("{{CustomerPhone}}", order.CustomerPhone);
            body = body.Replace("{{RecipientName}}", order.RecipientName);
            body = body.Replace("{{RecipientPhone}}", order.RecipientPhone);
            body = body.Replace("{{PaymentMethod}}", _l[order.PaymentMethod.ToString()]);
            body = body.Replace("{{PaymentStatus}}", _l[order.OrderStatus.ToString()]);
            body = body.Replace("{{ShippingMethod}}", _l[order.DeliveryMethod.ToString()]);
            body = body.Replace("{{DeliveryFee}}", "0");
            body = body.Replace("{{RecipientAddress}}", order.AddressDetails);
            body = body.Replace("{{ShippingStatus}}", _l[order.OrderStatus.ToString()]);
            body = body.Replace("{{RecipientComments}}", order.Remarks);

            if (order.OrderItems != null)
            {
                StringBuilder sb = new StringBuilder();
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

            var defaultFromEmail = await _settingManager.GetOrNullGlobalAsync("Abp.Mailing.DefaultFromAddress");
            var defaultFromName = await _settingManager.GetOrNullGlobalAsync("Abp.Mailing.DefaultFromDisplayName");
            MailAddress from = new(defaultFromEmail, emailSettings?.SenderName ?? defaultFromName);
            MailAddress to = new(order.CustomerEmail);

            MailMessage mailMessage = new(from, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await _emailSender.SendAsync(mailMessage);
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

                    order = await _orderRepository.GetWithDetailsAsync(order.Id);

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

                    await SendEmailAsync(order.Id);
                }
            }
        }

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

            //var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            //if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            //{
            //    throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            //}
            var items = await _orderRepository.GetListAsync(0, int.MaxValue, input.Sorting,input.Filter,input.GroupBuyId,input.OrderIds);
            var Results = ObjectMapper.Map<List<Order>, List<OrderDto>>(items);
            var excelContent = Results.Select(x =>new
            {
                OrderNumber=x.OrderNo,
                OrderDate=x.CreationTime, 
                CustomerName=x.CustomerName, 
                Email=x.CustomerEmail, 
               
                RecipientInformation=x.RecipientName+"/"+x.RecipientPhone, 
                ShippingMethod=x.DeliveryMethod,
                Address=x.AddressDetails,
                Notes=x.Remarks,
                MerchantNotes=x.Remarks,
                OrderedItems = string.Join(", ", x.OrderItems.Select(item =>
        (item.ItemType == ItemType.Item) ? $"{item.Item?.ItemName} x {item.Quantity}" :
        (item.ItemType == ItemType.SetItem) ? $"{item.SetItem?.SetItemName} x {item.Quantity}" :
        (item.ItemType == ItemType.Freebie) ? $"{item.Freebie?.ItemName} x {item.Quantity}" : "")
    ),
                InvoiceStatus=x.InvoiceStatus,
                ShippingStatus=x.ShippingStatus,
                PaymentMethod = x.PaymentMethod,
                CheckoutAmount=x.TotalAmount




            });
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(excelContent);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new RemoteStreamContent(memoryStream, "InventroyReport.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}

