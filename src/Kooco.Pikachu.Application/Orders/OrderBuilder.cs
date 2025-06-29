using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Services;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Orders;

public class OrderBuilder(OrderManager orderManager) : ITransientDependency
{
    protected OrderManager OrderManager { get; } = orderManager;

    public async Task<Order> CloneAsync(Order order)
    {
        var clone = await OrderManager.CreateAsync(
            order.GroupBuyId,
            order.IsIndividual,
            order.CustomerName,
            order.CustomerPhone,
            order.CustomerEmail,
            order.PaymentMethod,
            order.InvoiceType,
            order.InvoiceNumber,
            order.CarrierId,
            order.UniformNumber,
            order.TaxTitle,
            order.IsAsSameBuyer,
            order.RecipientName,
            order.RecipientPhone,
            order.RecipientEmail,
            order.DeliveryMethod,
            order.PostalCode,
            order.City,
            order.District,
            order.Road,
            order.AddressDetails,
            order.Remarks,
            order.ReceivingTime,
            order.TotalQuantity,
            order.TotalAmount,
            order.ReturnStatus,
            null,
            order.Id
        );

        clone.ShippingStatus = order.ShippingStatus;
        clone.MerchantTradeNo = order.MerchantTradeNo;
        clone.StoreId = order.StoreId;
        clone.StoreAddress = order.StoreAddress;
        clone.CVSStoreOutSide = order.CVSStoreOutSide;
        clone.DeliveryCostForNormal = order.DeliveryCostForNormal;
        clone.DeliveryCostForFreeze = order.DeliveryCostForFreeze;
        clone.DeliveryCostForFrozen = order.DeliveryCostForFrozen;
        clone.DeliveryCost = order.DeliveryCost;
        clone.GWSR = order.GWSR;
        clone.TradeNo = order.TradeNo;
        clone.OrderRefundType = order.OrderRefundType;
        
        if (order.ExtraProperties != null)
        {
            foreach (var prop in order.ExtraProperties)
            {
                clone.SetProperty(prop.Key, prop.Value);
            }
        }

        return clone;
    }
}