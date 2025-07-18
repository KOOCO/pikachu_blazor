using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooco.Pikachu.TestHelpers;

/// <summary>
/// Helper class for building test order data
/// </summary>
public class TestOrderDataBuilder
{
    private readonly IOrderAppService _orderAppService;
    
    public TestOrderDataBuilder(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }
    
    /// <summary>
    /// Creates a basic test order with default values
    /// </summary>
    public async Task<OrderDto> CreateBasicOrderAsync(
        PaymentMethods paymentMethod = PaymentMethods.CashOnDelivery,
        DeliveryMethod deliveryMethod = DeliveryMethod.HomeDelivery)
    {
        var input = new CreateUpdateOrderDto
        {
            GroupBuyId = TestData.GroupBuy1Id,
            CustomerName = "Test Customer",
            CustomerPhone = "0912345678",
            CustomerEmail = "test@example.com",
            PaymentMethod = paymentMethod,
            InvoiceType = InvoiceType.PersonalInvoice,
            CarrierId = "TestCarrier",
            TaxTitle = "Test Customer",
            IsAsSameBuyer = true,
            RecipientName = "Test Customer",
            RecipientPhone = "0912345678",
            RecipientEmail = "test@example.com",
            DeliveryMethod = deliveryMethod,
            PostalCode = "10001",
            City = "Taipei",
            AddressDetails = "Test Address",
            Remarks = "Test Order",
            ReceivingTime = ReceivingTime.Weekday9To13,
            TotalQuantity = 1,
            TotalAmount = 100,
            DeliveryCost = 60,
            OrderItems = new List<CreateUpdateOrderItemDto>
            {
                new CreateUpdateOrderItemDto
                {
                    ItemId = TestData.Item1Id,
                    ItemType = ItemType.Item,
                    Spec = "Test Item",
                    Quantity = 1,
                    ItemPrice = 100,
                    TotalAmount = 100,
                    SKU = "TEST-SKU",
                    DeliveryTemperature = ItemStorageTemperature.Normal
                }
            }
        };

        return await _orderAppService.CreateAsync(input);
    }
    
    /// <summary>
    /// Creates an order and simulates payment confirmation
    /// </summary>
    public async Task<OrderDto> CreatePaidOrderAsync(
        PaymentMethods paymentMethod = PaymentMethods.CreditCard)
    {
        // Create order with specified payment method
        var order = await CreateBasicOrderAsync(paymentMethod);
        
        // Generate merchant trade number
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo);
        
        // Update merchant trade number
        await _orderAppService.UpdateMerchantTradeNoAsync(new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            MerchantTradeNo = merchantTradeNo
        });
        
        // Simulate successful payment
        var paymentResult = new PaymentResult
        {
            MerchantID = "TestMerchant",
            MerchantTradeNo = merchantTradeNo,
            PaymentDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            PaymentType = paymentMethod == PaymentMethods.CreditCard ? "Credit_CreditCard" : "ATM_TAISHIN",
            RtnCode = 1, // Success
            RtnMsg = "Success",
            SimulatePaid = 0,
            StoreID = "TestStore",
            TradeAmt = (int)order.TotalAmount,
            TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            TradeNo = "TEST" + Guid.NewGuid().ToString("N")[..12],
            CheckMacValue = "TestCheckMacValue"
        };
        
        await _orderAppService.HandlePaymentAsync(paymentResult);
        
        // Return updated order
        return await _orderAppService.GetAsync(order.Id);
    }
    
    /// <summary>
    /// Creates an order with multiple items
    /// </summary>
    public async Task<OrderDto> CreateOrderWithMultipleItemsAsync(int itemCount = 2)
    {
        var orderItems = new List<CreateUpdateOrderItemDto>();
        var totalAmount = 0m;
        var totalQuantity = 0;
        
        for (int i = 0; i < itemCount; i++)
        {
            var quantity = i + 1;
            var price = 100 * (i + 1);
            var amount = quantity * price;
            
            orderItems.Add(new CreateUpdateOrderItemDto
            {
                ItemId = TestData.Item1Id,
                ItemType = ItemType.Item,
                Spec = $"Test Item {i + 1}",
                Quantity = quantity,
                ItemPrice = price,
                TotalAmount = amount,
                SKU = $"TEST-SKU-{i + 1}",
                DeliveryTemperature = ItemStorageTemperature.Normal
            });
            
            totalAmount += amount;
            totalQuantity += quantity;
        }
        
        var input = new CreateUpdateOrderDto
        {
            GroupBuyId = TestData.GroupBuy1Id,
            CustomerName = "Test Customer",
            CustomerPhone = "0912345678",
            CustomerEmail = "test@example.com",
            PaymentMethod = PaymentMethods.CashOnDelivery,
            InvoiceType = InvoiceType.PersonalInvoice,
            CarrierId = "TestCarrier",
            TaxTitle = "Test Customer",
            IsAsSameBuyer = true,
            RecipientName = "Test Customer",
            RecipientPhone = "0912345678",
            RecipientEmail = "test@example.com",
            DeliveryMethod = DeliveryMethod.HomeDelivery,
            PostalCode = "10001",
            City = "Taipei",
            AddressDetails = "Test Address",
            Remarks = "Test Order with Multiple Items",
            ReceivingTime = ReceivingTime.Weekday9To13,
            TotalQuantity = totalQuantity,
            TotalAmount = totalAmount,
            DeliveryCost = 60,
            OrderItems = orderItems
        };

        return await _orderAppService.CreateAsync(input);
    }
    
    /// <summary>
    /// Creates an order with set items
    /// </summary>
    public async Task<OrderDto> CreateOrderWithSetItemAsync()
    {
        var input = new CreateUpdateOrderDto
        {
            GroupBuyId = TestData.GroupBuy1Id,
            CustomerName = "Test Customer",
            CustomerPhone = "0912345678",
            CustomerEmail = "test@example.com",
            PaymentMethod = PaymentMethods.CashOnDelivery,
            InvoiceType = InvoiceType.PersonalInvoice,
            CarrierId = "TestCarrier",
            TaxTitle = "Test Customer",
            IsAsSameBuyer = true,
            RecipientName = "Test Customer",
            RecipientPhone = "0912345678",
            RecipientEmail = "test@example.com",
            DeliveryMethod = DeliveryMethod.PostOffice,
            PostalCode = "10001",
            City = "Taipei",
            AddressDetails = "Test Address",
            Remarks = "Test Order with Set Item",
            ReceivingTime = ReceivingTime.Weekday9To13,
            TotalQuantity = 2,
            TotalAmount = 200,
            DeliveryCost = 60,
            OrderItems = new List<CreateUpdateOrderItemDto>
            {
                new CreateUpdateOrderItemDto
                {
                    SetItemId = TestData.SetItem1Id,
                    ItemType = ItemType.SetItem,
                    Spec = "Test Set Item",
                    Quantity = 2,
                    ItemPrice = 100,
                    TotalAmount = 200,
                    SKU = "TEST-SET-SKU",
                    DeliveryTemperature = ItemStorageTemperature.Normal
                }
            }
        };

        return await _orderAppService.CreateAsync(input);
    }
}