using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Kooco.Pikachu.Orders
{
    public interface IOrderAppService: IApplicationService
    {
        Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo, PaymentMethods? paymentMethod = null);
        string GenerateMerchantTradeNo(string orderNo);
        Task UpdateLogisticStatusAsync(string merchantTradeNo, string rtnMsg);
        Task AddValuesAsync(Guid id, string checkMacValue, string merchantTradeNo);
        Task<OrderDto> GetOrderAsync(Guid groupBuyId, string orderNo, string extraInfo);
        Task<OrderDto> UpdateOrderPaymentMethodAsync(OrderPaymentMethodRequest request);
        Task<OrderDto> UpdateMerchantTradeNoAsync(OrderPaymentMethodRequest request);
        Task<OrderDto> GetAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input, bool hideCredentials = false);
        Task<OrderDto> CreateAsync(CreateUpdateOrderDto input);
        Task<OrderDto> UpdateAsync(Guid id,CreateOrderDto input);
        Task<OrderDto> UpdateShippingDetails(Guid id, CreateOrderDto input);
        Task<OrderDto> GetWithDetailsAsync(Guid id);
        Task AddStoreCommentAsync(Guid id, string comment);
        Task UpdateStoreCommentAsync(Guid id, Guid commentId, string comment);
        Task HandlePaymentAsync(PaymentResult paymentResult);
        Task AddCheckMacValueAsync(Guid id, string checkMacValue);
        Task UpdateOrderItemsAsync(Guid id, List<UpdateOrderItemDto> orderItems);
        Task CancelOrderAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetReturnListAsync(GetOrderListDto input);
        Task ChangeReturnStatusAsync(Guid id, OrderReturnStatus? orderReturnStatus);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(GetOrderListDto input);
        Task<PaymentGatewayDto> GetPaymentGatewayConfigurationsAsync(Guid id);
        Task<OrderDto> MergeOrdersAsync(List<Guid> Ids);
        Task<OrderDto> SplitOrderAsync(List<Guid> OrderItemIds, Guid OrderId);
        Task<OrderDto> RefundOrderItems(List<Guid> OrderItemIds, Guid OrderId);
        Task ExchangeOrderAsync(Guid id);
        Task<PagedResultDto<OrderDto>> GetTenantOrderListAsync(GetOrderListDto input);
        Task<PagedResultDto<OrderDto>> GetReconciliationListAsync(GetOrderListDto input);
        Task<IRemoteStreamContent> GetReconciliationListAsExcelFileAsync(GetOrderListDto input);
        Task<OrderDto> OrderShipped(Guid id);
        Task<OrderDto> OrderClosed(Guid id);
        Task<OrderDto> OrderComplete(Guid id);
        Task VoidInvoice(Guid id, string reason);
        Task<PagedResultDto<OrderDto>> GetVoidListAsync(GetOrderListDto input);
        Task CreditNoteInvoice(Guid id, string reason);
        Task RefundAmountAsync(double amount, Guid OrderId);
        Task<PagedResultDto<OrderDto>> GetReportListAsync(GetOrderListDto input, bool hideCredentials = false);
        Task ReturnOrderAsync(Guid id);
        Task<OrderDto> ChangeOrderStatus(Guid id, ShippingStatus status);

        Task UpdateOrdersIfIsEnterpricePurchaseAsync(Guid groupBuyId);

        Task<(int normalCount, int freezeCount, int frozenCount)> GetTotalDeliveryTemperatureCountsAsync();
        Task<OrderDto> OrderToBeShipped(Guid id);
        Task<(decimal PaidAmount, decimal UnpaidAmount, decimal RefundedAmount)> GetOrderStatusAmountsAsync(Guid UserId);
        Task<(int Open, int Exchange, int Return)> GetOrderStatusCountsAsync(Guid userId);
    }
}
