using Kooco.Pikachu.Campaigns;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Freebies;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Repositories.Orders;
public class OrderRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, Order, Guid>(dbContextProvider), IOrderRepository
{
    #region Methods
    public async Task<long> CountAsync(string? filter,
                                       Guid? groupBuyId,
                                       DateTime? startDate,
                                       DateTime? endDate,
                                       OrderStatus? orderStatus = null,
                                       ShippingStatus? shippingStatus = null,
                                       DeliveryMethod? deliveryMethod = null)
    {
        return await ApplyFilters(
            (await GetQueryableAsync()).Include(o => o.GroupBuy),
            filter,
            groupBuyId,
            null,
            startDate,
            endDate,
            orderStatus,
            shippingStatus,
            deliveryMethod
        ).CountAsync();
    }

    public async Task<long> CountAllAsync(string? filter,
                                          Guid? groupBuyId,
                                          DateTime? startDate,
                                          DateTime? endDate,
                                          OrderStatus? orderStatus = null)
    {
        return await ApplyFiltersNew((await GetQueryableAsync()).Include(o => o.GroupBuy), filter, groupBuyId, null, startDate, endDate, orderStatus).CountAsync();
    }

    public async Task<List<Order>> GetListAsync(int skipCount,
                                                int maxResultCount,
                                                string? sorting,
                                                string? filter,
                                                Guid? groupBuyId,
                                                List<Guid> orderId,
                                                DateTime? startDate = null,
                                                DateTime? endDate = null,
                                                OrderStatus? orderStatus = null,
                                                ShippingStatus? shippingStatus = null,
                                                DeliveryMethod? deliveryMethod = null)
    {
        PikachuDbContext dbContext = await GetDbContextAsync();

        var result = ApplyFilters(await GetQueryableAsync(),
                                       filter,
                                       groupBuyId,
                                       orderId,
                                       startDate,
                                       endDate,
                                       orderStatus,
                                       shippingStatus,
                                       deliveryMethod);
        result = result.OrderBy(sorting)
         .PageBy(skipCount, maxResultCount)
         .Include(o => o.OrderItems)
         .ThenInclude(oi => oi.Freebie);

        return await result.Select(o => new Order
        {
            OrderId = o.Id,
            OrderNo = o.OrderNo,                      // Order No
            LastModificationTime = o.LastModificationTime, // Last Updated Date
            IssueStatus = o.IssueStatus,       // Invoice Issue Status
            InvoiceNumber = o.InvoiceNumber,                // Invoice Number
            TotalAmount = o.TotalAmount, // Checkout Amount
            CreationTime = o.CreationTime,                        // Editor (depends on your model)
            CustomerName = o.CustomerName,
            CustomerEmail = o.CustomerEmail,
            CustomerPhone = o.CustomerPhone,
            DeliveryMethod = o.DeliveryMethod,               // Shipping Method
            OrderStatus = o.OrderStatus,
            DeliveryCost = o.DeliveryCost,
            ShippingStatus = o.ShippingStatus,
            PaymentMethod = o.PaymentMethod,
            City = o.City,
            PostalCode = o.PostalCode,
            AddressDetails = o.AddressDetails, // Address
            Remarks = o.Remarks,                      // Remarks
            VoidUser = o.VoidUser,                                       // Merchant Remarks
            StoreComments = o.StoreComments.ToList(),
            OrderType = o.OrderType,
            RecipientName = o.RecipientName,
            RecipientPhone = o.RecipientPhone,
            RecipientNameDbsNormal = o.RecipientNameDbsNormal,
            RecipientPhoneDbsNormal = o.RecipientPhoneDbsNormal,
            RecipientNameDbsFreeze = o.RecipientNameDbsFreeze,
            RecipientPhoneDbsFreeze = o.RecipientPhoneDbsFreeze,
            RecipientNameDbsFrozen = o.RecipientNameDbsFrozen,
            RecipientPhoneDbsFrozen = o.RecipientPhoneDbsFrozen,
            OrderItems = o.OrderItems.Select(oi => new OrderItem
            {
                OrderId = oi.OrderId,
                Spec = oi.Spec,
                Quantity = oi.Quantity,
                ItemType = oi.ItemType,
                Item = oi.Item != null ? new Item { ItemName = oi.Item.ItemName } : new Item(),
                SetItem = oi.SetItem != null ? new SetItem { SetItemName = oi.SetItem.SetItemName } : new SetItem(),
                Freebie = oi.Freebie != null ? new Freebie { ItemName = oi.Freebie.ItemName } : new Freebie(),
            }).ToList()
        }).ToListAsync();
    }
    public async Task<List<Order>> GetAllListAsync(
        int skipCount, 
        int maxResultCount, 
        string? sorting, 
        string? filter, 
        Guid? groupBuyId, 
        List<Guid> orderId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        OrderStatus? orderStatus = null,
        DateTime? completionTimeFrom = null,
        DateTime? completionTimeTo = null,
        ShippingStatus? shippingStatus = null
        )
    {
        return await ApplyFiltersNew(
            await GetQueryableAsync(), 
            filter, 
            groupBuyId, 
            orderId, 
            startDate, 
            endDate, 
            orderStatus,
            shippingStatus,
            completionTimeFrom,
            completionTimeTo
            )
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .Include(o => o.GroupBuy)
            .Include(o => o.StoreComments)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.SetItem)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Freebie)
            .ToListAsync();
    }

    public async Task<GroupBuyReportModelWithCount> GetReportListAsync(
        int skipCount, 
        int maxResultCount, 
        string? sorting, 
        string? filter, 
        Guid? groupBuyId, 
        List<Guid> orderId, 
        DateTime? startDate = null, 
        DateTime? endDate = null, 
        OrderStatus? orderStatus = null,
        ShippingStatus? shippingStatus = null,
        DateTime? completionTimeFrom = null,
        DateTime? completionTimeTo = null
        )
    {
        var query = ApplyFiltersNew(
            await GetQueryableAsync(),
            filter,
            groupBuyId,
            orderId,
            startDate,
            endDate,
            orderStatus,
            shippingStatus,
            completionTimeFrom,
            completionTimeTo
            )
            .Include(o => o.GroupBuy)
            .Include(o => o.StoreComments)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.SetItem)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Freebie)
            .Select(o => new GroupBuyReportOrderModel
            {
                Id = o.Id,
                OrderNo = o.OrderNo,
                CreationTime = o.CreationTime,
                CustomerName = o.CustomerName,
                CustomerEmail = o.CustomerEmail,
                OrderStatus = o.OrderStatus,
                ShippingStatus = o.ShippingStatus,
                PaymentMethod = o.PaymentMethod,
                ShippingFee = o.DeliveryCost,
                TotalAmount = o.TotalAmount,
                GroupBuyId = o.GroupBuyId,
                OrderItems = o.OrderItems
                    .Select(oi => new GroupBuyReportOrderItemsModel
                    {
                        Id = oi.Id,
                        SKU = oi.SKU,
                        Name = oi.ItemType == ItemType.Item ? oi.Item != null ? oi.Item.ItemName : null
                            : oi.ItemType == ItemType.SetItem ? oi.SetItem != null ? oi.SetItem.SetItemName : null : oi.Freebie != null ? oi.Freebie.ItemName : null,
                        Spec = oi.Spec,
                        Quantity = oi.Quantity,
                        ItemType = oi.ItemType
                    }).ToList()
            });

        var totalCount = await query.LongCountAsync();
        var items = await query
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();

        return new GroupBuyReportModelWithCount
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<long> CountReconciliationAsync(string? filter, Guid? groupBuyId, DateTime? startDate, DateTime? endDate)
    {
        return await ApplyReconciliationFilters((await GetQueryableAsync()).Include(o => o.GroupBuy), filter, groupBuyId, null, startDate, endDate).CountAsync();
    }
    public async Task<List<Order>> GetReconciliationListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId, List<Guid> orderId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = ApplyReconciliationFilters(await GetQueryableAsync(), filter, groupBuyId, orderId, startDate, endDate);
        query = query.OrderBy(sorting)
           .PageBy(skipCount, maxResultCount)

           .Include(o => o.OrderItems)

           .ThenInclude(oi => oi.Freebie);
        return await query.Select(o => new Order
        {
            OrderId = o.Id,
            OrderNo = o.OrderNo,                      // Order No
            LastModificationTime = o.LastModificationTime, // Last Updated Date
            IssueStatus = o.IssueStatus,       // Invoice Issue Status
            InvoiceNumber = o.InvoiceNumber,                // Invoice Number
            TotalAmount = o.TotalAmount, // Checkout Amount
                                         // Editor (depends on your model)
            CustomerName = o.CustomerName + "/" + o.CustomerPhone,
            DeliveryMethod = o.DeliveryMethod,               // Shipping Method
            AddressDetails = o.AddressDetails, // Address
            Remarks = o.Remarks,                      // Remarks
            VoidUser = o.VoidUser,                                       // Merchant Remarks
            OrderItems = o.OrderItems.Select(oi => new OrderItem
            {
                OrderId = oi.OrderId,
                Spec = oi.Spec,
                Quantity = oi.Quantity,
                ItemType = oi.ItemType,
                Item = oi.Item != null ? new Item { ItemName = oi.Item.ItemName } : new Item(),
                SetItem = oi.SetItem != null ? new SetItem { SetItemName = oi.SetItem.SetItemName } : new SetItem(),
                Freebie = oi.Freebie != null ? new Freebie { ItemName = oi.Freebie.ItemName } : new Freebie(),
            }).ToList()
        })
    .ToListAsync();
    }
    public async Task<long> CountVoidAsync(string? filter, Guid? groupBuyId, DateTime? startDate, DateTime? endDate)
    {
        return await ApplyVoidFilters((await GetQueryableAsync()).Include(o => o.GroupBuy), filter, groupBuyId, null, startDate, endDate).CountAsync();
    }
    public async Task<List<Order>> GetVoidListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId, List<Guid> orderId, DateTime? startDate = null, DateTime? endDate = null)
    {
        return await ApplyVoidFilters(await GetQueryableAsync(), filter, groupBuyId, orderId, startDate, endDate)
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .Include(o => o.GroupBuy)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.SetItem)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Freebie)
            .ToListAsync();
    }

    public async Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync()
    {
        IQueryable<Order> queryable = await GetQueryableAsync();

        int normalCount = await queryable.Where(order => order.ShippingStatus == ShippingStatus.ToBeShipped)
                                         .SelectMany(order => order.OrderItems)
                                         .CountAsync(orderItem => orderItem.DeliveryTemperature == ItemStorageTemperature.Normal);

        int freezeCount = await queryable.Where(order => order.ShippingStatus == ShippingStatus.ToBeShipped)
                                         .SelectMany(order => order.OrderItems)
                                         .CountAsync(orderItem => orderItem.DeliveryTemperature == ItemStorageTemperature.Freeze);

        int frozenCount = await queryable.Where(order => order.ShippingStatus == ShippingStatus.ToBeShipped)
                                         .SelectMany(order => order.OrderItems)
                                         .CountAsync(orderItem => orderItem.DeliveryTemperature == ItemStorageTemperature.Frozen);

        return (normalCount, freezeCount, frozenCount);
    }
    #endregion

    #region Private Methods
    private static IQueryable<Order> ApplyFiltersNew(
        IQueryable<Order> queryable,
        string? filter,
        Guid? groupBuyId,
        List<Guid> orderIds,
        DateTime? startDate = null,
        DateTime? endDate = null,
        OrderStatus? orderStatus = null,
        ShippingStatus? shippingStatus = null,
        DateTime? completionTimeFrom = null,
        DateTime? completionTimeTo = null
        )
    {
        return queryable
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .WhereIf(!filter.IsNullOrWhiteSpace(),
            x => x.OrderNo.Contains(filter)
            || x.CustomerName != null && x.CustomerName.Contains(filter)
            || x.CustomerEmail != null && x.CustomerEmail.Contains(filter)
            ).WhereIf(orderIds != null && orderIds.Any(), x => orderIds.Contains(x.Id))
            .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
            .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
            .WhereIf(orderStatus.HasValue, x => x.OrderStatus == orderStatus)
            .WhereIf(shippingStatus.HasValue, x => x.ShippingStatus == shippingStatus)
            .WhereIf(completionTimeFrom.HasValue, x => x.CompletionTime.HasValue && x.CompletionTime >= completionTimeFrom!.Value.Date)
            .WhereIf(completionTimeTo.HasValue, x => x.CompletionTime.HasValue && x.CompletionTime.Value.Date <= completionTimeTo!.Value.Date);
        //.Where(x => x.IsRefunded == false);
        //.Where(x => x.OrderType != OrderType.MargeToNew);
    }

    private static IQueryable<Order> ApplyFilters(IQueryable<Order> queryable,
                                                  string? filter,
                                                  Guid? groupBuyId,
                                                  List<Guid> orderIds,
                                                  DateTime? startDate = null,
                                                  DateTime? endDate = null,
                                                  OrderStatus? orderStatus = null,
                                                  ShippingStatus? shippingStatus = null,
                                                  DeliveryMethod? deliveyMethod = null)
    {
        queryable = queryable
            .WhereIf(deliveyMethod.HasValue, w => w.DeliveryMethod == deliveyMethod)
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .WhereIf(!filter.IsNullOrWhiteSpace(),
                x => x.OrderNo.Contains(filter)
                || x.CustomerName != null && x.CustomerName.Contains(filter)
                || x.CustomerEmail != null && x.CustomerEmail.Contains(filter)
                )
            .WhereIf(orderIds != null && orderIds.Any(), x => orderIds.Contains(x.Id))
            .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
            .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
            .WhereIf(orderStatus.HasValue, x => x.OrderStatus == orderStatus)
            .WhereIf(shippingStatus.HasValue, w => w.ShippingStatus == shippingStatus);
            //.Where(x => x.IsRefunded == false);
        //.Where(x => x.OrderType != OrderType.MargeToNew);

        return queryable;
    }
    private static IQueryable<Order> ApplyReturnFilters(IQueryable<Order> queryable,
                                                        string? filter,
                                                        Guid? groupBuyId,
                                                        List<Guid> orderIds,
                                                        DateTime? startDate = null,
                                                        DateTime? endDate = null,
                                                        OrderStatus? orderStatus = null)
    {
        return queryable
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .WhereIf(!filter.IsNullOrWhiteSpace(),
            x => x.OrderNo.Contains(filter)
            || x.CustomerName != null && x.CustomerName.Contains(filter)
            || x.CustomerEmail != null && x.CustomerEmail.Contains(filter)
            ).WhereIf(orderIds != null && orderIds.Any(), x => orderIds.Contains(x.Id))
            .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
            .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
            .WhereIf(orderStatus.HasValue, x => x.OrderStatus == orderStatus)
            .Where(x => x.OrderStatus == OrderStatus.Returned || x.OrderStatus == OrderStatus.Exchange || x.IsRefunded == true)
            ;
        //.Where(x => x.OrderType != OrderType.MargeToNew);
    }
    private static IQueryable<Order> ApplyReconciliationFilters(IQueryable<Order> queryable,
                                                                string? filter,
                                                                Guid? groupBuyId,
                                                                List<Guid> orderIds,
                                                                DateTime? startDate = null,
                                                                DateTime? endDate = null)
    {
        return queryable
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .WhereIf(!filter.IsNullOrWhiteSpace(),
            x => x.OrderNo.Contains(filter)
            || x.CustomerName != null && x.CustomerName.Contains(filter)
            || x.CustomerEmail != null && x.CustomerEmail.Contains(filter)
            ).WhereIf(orderIds != null && orderIds.Any(), x => orderIds.Contains(x.Id))
            .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
            .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
            //.Where(x => x.ShippingStatus == ShippingStatus.Shipped)
            .Where(x => x.IsVoidInvoice == false && x.IssueStatus != null);
        //.Where(x => x.OrderType != OrderType.MargeToNew);
    }
    private static IQueryable<Order> ApplyVoidFilters(IQueryable<Order> queryable,
                                                      string? filter,
                                                      Guid? groupBuyId,
                                                      List<Guid> orderIds,
                                                      DateTime? startDate = null,
                                                      DateTime? endDate = null)
    {
        return queryable
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .WhereIf(!filter.IsNullOrWhiteSpace(),
            x => x.OrderNo.Contains(filter)
            || x.CustomerName != null && x.CustomerName.Contains(filter)
            || x.CustomerEmail != null && x.CustomerEmail.Contains(filter)
            ).WhereIf(orderIds != null && orderIds.Any(), x => orderIds.Contains(x.Id))
            .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
            .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
            //.Where(x => x.ShippingStatus == ShippingStatus.Shipped)
            //.Where(x => x.OrderType != OrderType.MargeToNew)
            .Where(x => x.IsVoidInvoice == true);
    }
    #endregion

    public async Task<Order> MaxByOrderNumberAsync()
    {
        var orders = await (await GetQueryableAsync()).ToListAsync();
        return orders.OrderByDescending(x => long.Parse(x.OrderNo[^9..])).FirstOrDefault();
    }
    public async Task<Order> GetWithDetailsAsync(Guid id)
    {
        return await (await GetQueryableAsync())
            .Where(o => o.Id == id)
            .Where(x => x.OrderType != OrderType.MargeToNew || x.OrderType != OrderType.SplitToNew)
            .Include(o => o.GroupBuy)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .ThenInclude(i => i.SetItemDetails)
                .ThenInclude(i => i.Item)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems.OrderBy(oi => oi.ItemType))
                .ThenInclude(oi => oi.Freebie)
                .ThenInclude(i => i.Images)
            .Include(o => o.StoreComments.OrderByDescending(c => c.CreationTime))
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync();
    }

    public async Task<Order> GetOrderByMerchantTradeNoAsync(string merchantTradeNo)
    {
        return await (await GetQueryableAsync())
                        .FirstOrDefaultAsync(w => w.OrderNo == merchantTradeNo ||
                                                  w.MerchantTradeNo == merchantTradeNo);
    }

    public async Task<Order?> MatchOrderExtraPropertiesByMerchantTradeNoAsync(string merchantTradeNo)
    {
        var query = await GetQueryableAsync();
        var orders = await query
            .OrderByDescending(o => o.CreationTime)
            .ToListAsync();

        foreach (var order in orders)
        {
            var json = order.GetProperty("Logistics_MerchantTradeNos")?.ToString();
            if (string.IsNullOrWhiteSpace(json))
            {
                continue; 
            }

            Dictionary<string, string>? dictionary = null;
            try
            {
                dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch
            {
                continue;
            }

            if (dictionary is not null && dictionary.ContainsValue(merchantTradeNo))
            {
                return order;
            }
        }

        return null;
    }

    public async Task<Order> GetMemberOrderAsync(Guid orderId, Guid memberId)
    {
        return await (await GetQueryableAsync())
            .Where(w => w.Id == orderId)
            .Where(w => w.UserId == memberId)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .ThenInclude(i => i.SetItemDetails)
                .ThenInclude(i => i.Item)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems.OrderBy(oi => oi.ItemType))
                .ThenInclude(oi => oi.Freebie)
                .ThenInclude(i => i.Images)
            .Include(o => o.StoreComments.OrderByDescending(c => c.CreationTime))
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync();
    }

    public async Task<List<MemberOrderInfoModel>> GetMemberOrdersByGroupBuyAsync(Guid memberId, Guid groupBuyId)
    {
        var orders = await (await GetQueryableAsync())
            .Where(x => x.UserId == memberId && x.GroupBuyId == groupBuyId)
            .OrderByDescending(x => x.CreationTime)
            .Select(x => new MemberOrderInfoModel
            {
                Id = x.Id,
                OrderNo = x.OrderNo,
                CreationTime = x.CreationTime,
                ShippingStatus = x.ShippingStatus,
                OrderStatus = x.OrderStatus,
                TotalAmount = x.TotalAmount,
                PaymentMethod = x.PaymentMethod
            }).ToListAsync();

        return orders;
    }

    public async Task<Order> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo)
    {
        return await (await GetQueryableAsync())
            .Where(w => w.GroupBuyId == groupBuyId)
            .Where(w => w.OrderNo == orderNo)
            .Where(w => w.CustomerEmail == extraInfo ||
                        w.CustomerName == extraInfo ||
                        w.CustomerPhone == extraInfo)
            //.Include(o => o.GroupBuy)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Item)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .ThenInclude(i => i.SetItemDetails)
                .ThenInclude(i => i.Item)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.SetItem)
                .ThenInclude(i => i.Images)
            .Include(o => o.OrderItems.OrderBy(oi => oi.ItemType))
                .ThenInclude(oi => oi.Freebie)
                .ThenInclude(i => i.Images)
            .Include(o => o.StoreComments.OrderByDescending(c => c.CreationTime))
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync();
    }

    public async Task<long> ReturnOrderCountAsync(string? filter, Guid? groupBuyId)
    {
        return await ApplyReturnFilters((await GetQueryableAsync()).Include(o => o.GroupBuy), filter, groupBuyId, null).Where(x => x.OrderStatus == OrderStatus.Returned || x.OrderStatus == OrderStatus.Exchange).CountAsync();
    }
    public async Task<long> ReturnOrderNotificationCountAsync()
    {
        return await ApplyReturnFilters((await GetQueryableAsync()).Include(o => o.GroupBuy), null, null, null).Where(x => (x.OrderStatus == OrderStatus.Returned || x.OrderStatus == OrderStatus.Exchange) && x.ReturnStatus == OrderReturnStatus.Pending).CountAsync();
    }

    public async Task<List<Order>> GetReturnListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId)
    {
        return await ApplyReturnFilters(await GetQueryableAsync(), filter, groupBuyId, null)
            .Where(x => x.OrderStatus == OrderStatus.Returned || x.OrderStatus == OrderStatus.Exchange)
            .OrderBy(sorting)
            .PageBy(skipCount, maxResultCount)
            .Include(o => o.GroupBuy)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.SetItem)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Freebie)
            .ToListAsync();
    }

    public async Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId)
    {
        List<Order> orders = [.. (await GetQueryableAsync()).Where(w => w.GroupBuyId == groupBuyId)];

        foreach (Order item in orders)
        {
            item.ShippingStatus = ShippingStatus.EnterpricePurchase;
        }

        await UpdateManyAsync(orders);
    }

    public async Task<string> GetOrderNoByOrderId(Guid OrderId)
    {
        return (await GetQueryableAsync())
                       .Where(w => w.Id == OrderId)
                       .Select(s => s.OrderNo)
                       .FirstOrDefault();
    }

    public async Task<List<Order>> GetOrdersToCloseAsync()
    {
        var dbContext = await GetDbContextAsync();

        var deliveredMethods = new[]
        {
            DeliveryMethod.HomeDelivery,
            DeliveryMethod.PostOffice,
            DeliveryMethod.BlackCat1,
            DeliveryMethod.BlackCatFreeze,
            DeliveryMethod.BlackCatFrozen,
            DeliveryMethod.TCatDeliveryNormal,
            DeliveryMethod.TCatDeliveryFreeze,
            DeliveryMethod.TCatDeliveryFrozen
        };

        var pickedUpMethods = new[]
        {
            DeliveryMethod.SevenToEleven1,
            DeliveryMethod.SevenToElevenFrozen,
            DeliveryMethod.SevenToElevenC2C,
            DeliveryMethod.FamilyMart1,
            DeliveryMethod.FamilyMartC2C,
            DeliveryMethod.TCatDeliverySevenElevenNormal,
            DeliveryMethod.TCatDeliverySevenElevenFreeze,
            DeliveryMethod.TCatDeliverySevenElevenFrozen
        };

        var ordersToClose = await dbContext.Orders
            .Where(o => o.CreationTime < DateTime.Today.AddDays(-7))
            .Where(o => o.DeliveryMethod != null
            && ((o.ShippingStatus == ShippingStatus.Delivered && deliveredMethods.Contains(o.DeliveryMethod.Value))
            || (o.ShippingStatus == ShippingStatus.PickedUp && pickedUpMethods.Contains(o.DeliveryMethod.Value))))
            .GroupJoin(
                dbContext.OrderHistories,
                order => order.Id,
                history => history.OrderId,
                (order, histories) => new { order, histories }
            )
            .SelectMany(
                x => x.histories.DefaultIfEmpty(),
                (x, history) => new { Order = x.order, History = history }
            )
            .GroupBy(g => g.Order)
            .Where(g => !g.Any(l => l.History != null && l.History.CreationTime.Date >= DateTime.Today.AddDays(-7)))
            .Select(g => g.Key)
            .OrderBy(o => o.CreationTime)
            .ToListAsync();

        return ordersToClose;
    }

    public async Task<List<(int AppliedAmount, Campaign Campaign)>> CheckForAppliedCreditsAsync(Guid id)
    {
        var dbContext = await GetDbContextAsync();

        var appliedCredits = dbContext.Orders
            .Where(order => order.Id == id)
            .SelectMany(order => order.AppliedCampaigns)
            .Join(dbContext.Campaigns.Where(c => c.PromotionModule == PromotionModule.ShoppingCredit).Include(c => c.ShoppingCredit),
            appliedCampaign => appliedCampaign.CampaignId,
            campaign => campaign.Id,
            (appliedCampaign, campaign) => new { AppliedCampaign = appliedCampaign, Campaign = campaign })
            .ToList();

        return appliedCredits.Select(x => (x.AppliedCampaign.Amount, x.Campaign)).ToList();
    }
}