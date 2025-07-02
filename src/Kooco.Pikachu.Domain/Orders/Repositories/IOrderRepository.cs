using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Orders.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Orders.Repositories;
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<Order> GetMemberOrderAsync(Guid orderId, Guid memberId);
    Task<List<MemberOrderInfoModel>> GetMemberOrdersByGroupBuyAsync(Guid memberId, Guid groupBuyId);
    Task<Order> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo);

    Task<long> CountAsync(string? filter,
                          Guid? groupBuyId,
                          DateTime? startDate = null,
                          DateTime? endDate = null,
                          OrderStatus? orderStatus = null,
                          ShippingStatus? shippingStatus = null,
                          DeliveryMethod? deliveryMethod = null);

    Task<long> CountAllAsync(string? filter,
                             Guid? groupBuyId,
                             DateTime? startDate = null,
                             DateTime? endDate = null,
                             OrderStatus? orderStatus = null);

    Task<List<Order>> GetListAsync(int skipCount,
                                   int maxResultCount,
                                   string? sorting,
                                   string? filter,
                                   Guid? groupBuyId,
                                   List<Guid> orderId,
                                   DateTime? startDate = null,
                                   DateTime? endDate = null,
                                   OrderStatus? orderStatus = null,
                                   ShippingStatus? shippingStatus = null,
                                   DeliveryMethod? deliveryMethod = null);

    Task<List<Order>> GetAllListAsync(
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
        );
    Task<Order> MaxByOrderNumberAsync();
    Task<Order> GetWithDetailsAsync(Guid id);
    Task<long> ReturnOrderCountAsync(string? filter, Guid? groupBuyId);
    Task<List<Order>> GetReturnListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId);
    Task<long> CountReconciliationAsync(string? filter, Guid? groupBuyId, DateTime? startDate, DateTime? endDate);
    Task<List<Order>> GetReconciliationListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId, List<Guid> orderId, DateTime? startDate = null, DateTime? endDate = null);
    Task<long> CountVoidAsync(string? filter, Guid? groupBuyId, DateTime? startDate, DateTime? endDate);
    Task<List<Order>> GetVoidListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, Guid? groupBuyId, List<Guid> orderId, DateTime? startDate = null, DateTime? endDate = null);

    Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId);

    Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync();

    Task<Order> GetOrderByMerchantTradeNoAsync(string merchantTradeNo);

    Task<Order?> MatchOrderExtraPropertiesByMerchantTradeNoAsync(string merchantTradeNo);
    Task<string> GetOrderNoByOrderId(Guid OrderId);
    Task<List<Order>> GetOrdersToCloseAsync();
    Task<long> ReturnOrderNotificationCountAsync();
    Task<GroupBuyReportModelWithCount> GetReportListAsync(
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
        );
}