using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.TestHelpers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Kooco.Pikachu.Orders;

/// <summary>
/// Tests for OrderAppService business logic and validation
/// </summary>
public class OrderAppServiceBusinessLogicTests : PikachuApplicationTestBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly TestOrderDataBuilder _testDataBuilder;

    public OrderAppServiceBusinessLogicTests()
    {
        _orderAppService = GetRequiredService<IOrderAppService>();
        _testDataBuilder = new TestOrderDataBuilder(_orderAppService);
    }

    #region Order Creation Validation Tests

    [Fact]
    public async Task CreateAsync_Should_Generate_Unique_OrderNo()
    {
        // Arrange & Act
        var order1 = await _testDataBuilder.CreateBasicOrderAsync();
        var order2 = await _testDataBuilder.CreateBasicOrderAsync();
        
        // Assert
        order1.OrderNo.ShouldNotBeNullOrEmpty();
        order2.OrderNo.ShouldNotBeNullOrEmpty();
        order1.OrderNo.ShouldNotBe(order2.OrderNo);
    }

    [Fact]
    public async Task CreateAsync_Should_Calculate_Total_Amount_Correctly()
    {
        // Arrange
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
            Remarks = "Test Order",
            ReceivingTime = ReceivingTime.Weekday9To13,
            TotalQuantity = 5, // 2 + 3
            TotalAmount = 500, // (2 * 100) + (3 * 100)
            DeliveryCost = 60,
            OrderItems = new List<CreateUpdateOrderItemDto>
            {
                new CreateUpdateOrderItemDto
                {
                    ItemId = TestData.Item1Id,
                    ItemType = ItemType.Item,
                    Spec = "Item 1",
                    Quantity = 2,
                    ItemPrice = 100,
                    TotalAmount = 200,
                    SKU = "SKU-1",
                    DeliveryTemperature = ItemStorageTemperature.Normal
                },
                new CreateUpdateOrderItemDto
                {
                    ItemId = TestData.Item1Id,
                    ItemType = ItemType.Item,
                    Spec = "Item 2",
                    Quantity = 3,
                    ItemPrice = 100,
                    TotalAmount = 300,
                    SKU = "SKU-2",
                    DeliveryTemperature = ItemStorageTemperature.Normal
                }
            }
        };
        
        // Act
        var order = await _orderAppService.CreateAsync(input);
        
        // Assert
        order.TotalQuantity.ShouldBe(5);
        order.TotalAmount.ShouldBe(500);
        order.DeliveryCost.ShouldBe(60);
    }

    [Fact]
    public async Task CreateAsync_Should_Set_Initial_Order_Status()
    {
        // Arrange & Act
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        
        // Assert
        order.OrderStatus.ShouldBe(OrderStatus.Open);
        order.ShippingStatus.ShouldBe(ShippingStatus.WaitingForPayment);
        order.IsRefunded.ShouldBeFalse();
        order.ReturnStatus.ShouldBe(OrderReturnStatus.Pending);
    }

    #endregion

    #region Order State Transition Tests

    [Fact]
    public async Task Order_Status_Should_Follow_Valid_Transitions()
    {
        // Arrange
        var order = await _testDataBuilder.CreatePaidOrderAsync();
        
        // Act & Assert - Valid transition: Paid -> ToBeShipped
        var toBeShippedOrder = await _orderAppService.OrderToBeShipped(order.Id);
        toBeShippedOrder.ShippingStatus.ShouldBe(ShippingStatus.ToBeShipped);
        
        // Valid transition: ToBeShipped -> Shipped
        var shippedOrder = await _orderAppService.OrderShipped(toBeShippedOrder.Id);
        shippedOrder.ShippingStatus.ShouldBe(ShippingStatus.Shipped);
        shippedOrder.ShippingDate.ShouldNotBeNull();
        
        // Valid transition: Shipped -> Completed
        var completedOrder = await _orderAppService.OrderComplete(shippedOrder.Id);
        completedOrder.ShippingStatus.ShouldBe(ShippingStatus.Completed);
        completedOrder.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public async Task CancelOrderAsync_Should_Not_Allow_When_Shipped()
    {
        // Arrange
        var order = await _testDataBuilder.CreatePaidOrderAsync();
        await _orderAppService.OrderToBeShipped(order.Id);
        await _orderAppService.OrderShipped(order.Id);
        
        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _orderAppService.CancelOrderAsync(order.Id)
        );
    }

    [Fact]
    public async Task ExpireOrderAsync_Should_Only_Work_For_Unpaid_Orders()
    {
        // Arrange
        var unpaidOrder = await _testDataBuilder.CreateBasicOrderAsync();
        var paidOrder = await _testDataBuilder.CreatePaidOrderAsync();
        
        // Act
        await _orderAppService.ExpireOrderAsync(unpaidOrder.Id);
        
        // Assert
        var expiredOrder = await _orderAppService.GetAsync(unpaidOrder.Id);
        expiredOrder.OrderStatus.ShouldBe(OrderStatus.Closed);
        
        // Paid order should not be expirable
        await Should.ThrowAsync<Exception>(async () =>
            await _orderAppService.ExpireOrderAsync(paidOrder.Id)
        );
    }

    #endregion

    #region Order Modification Tests

    [Fact]
    public async Task UpdateOrderItemsAsync_Should_Update_Quantities_And_Totals()
    {
        // Arrange
        var order = await _testDataBuilder.CreateOrderWithMultipleItemsAsync(2);
        var originalQuantity = order.TotalQuantity;
        var originalAmount = order.TotalAmount;
        
        var updateItems = order.OrderItems.Select(x => new UpdateOrderItemDto
        {
            Id = x.Id,
            Quantity = x.Quantity + 1, // Increase each item by 1
            ItemPrice = x.ItemPrice
        }).ToList();
        
        // Act
        await _orderAppService.UpdateOrderItemsAsync(order.Id, updateItems);
        
        // Assert
        var updatedOrder = await _orderAppService.GetWithDetailsAsync(order.Id);
        updatedOrder.TotalQuantity.ShouldBeGreaterThan(originalQuantity);
        updatedOrder.OrderItems.Sum(x => x.Quantity).ShouldBe(updatedOrder.TotalQuantity);
    }

    [Fact]
    public async Task AddStoreCommentAsync_Should_Add_Comment()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        var comment1 = "First store comment";
        var comment2 = "Second store comment";
        
        // Act
        await _orderAppService.AddStoreCommentAsync(order.Id, comment1);
        await _orderAppService.AddStoreCommentAsync(order.Id, comment2);
        
        // Assert
        var orderWithComments = await _orderAppService.GetWithDetailsAsync(order.Id);
        orderWithComments.StoreComments.Count.ShouldBe(2);
        orderWithComments.StoreComments.ShouldContain(c => c.Comment == comment1);
        orderWithComments.StoreComments.ShouldContain(c => c.Comment == comment2);
    }

    #endregion

    #region Payment and Refund Tests

    [Fact]
    public async Task HandlePaymentAsync_Should_Only_Accept_Valid_Payment()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo);
        
        await _orderAppService.UpdateMerchantTradeNoAsync(new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            MerchantTradeNo = merchantTradeNo
        });
        
        // Act - Invalid payment (RtnCode = 0)
        var failedPayment = new PaymentResult
        {
            MerchantID = "TestMerchant",
            MerchantTradeNo = merchantTradeNo,
            RtnCode = 0, // Failed
            RtnMsg = "Payment Failed",
            TradeAmt = (int)order.TotalAmount
        };
        
        var failedResult = await _orderAppService.HandlePaymentAsync(failedPayment);
        
        // Assert
        failedResult.ShouldBe("0|Error");
        var unchangedOrder = await _orderAppService.GetAsync(order.Id);
        unchangedOrder.OrderStatus.ShouldBe(OrderStatus.Open); // Status unchanged
        unchangedOrder.PaymentDate.ShouldBeNull();
    }

    [Fact]
    public async Task RefundOrderItems_Should_Mark_Order_As_Refunded()
    {
        // Arrange
        var order = await _testDataBuilder.CreatePaidOrderAsync();
        var orderItemIds = order.OrderItems.Select(x => x.Id).ToList();
        
        // Act
        var refundedOrder = await _orderAppService.RefundOrderItems(orderItemIds, order.Id);
        
        // Assert
        refundedOrder.IsRefunded.ShouldBeTrue();
        refundedOrder.OrderStatus.ShouldBe(OrderStatus.Refund);
    }

    [Fact]
    public async Task RefundAmountAsync_Should_Update_Refund_Amount()
    {
        // Arrange
        var order = await _testDataBuilder.CreatePaidOrderAsync();
        var refundAmount = 50.0; // Partial refund
        
        // Act
        await _orderAppService.RefundAmountAsync(refundAmount, order.Id);
        
        // Assert
        var refundedOrder = await _orderAppService.GetAsync(order.Id);
        refundedOrder.RefundAmount.ShouldBe((decimal)refundAmount);
    }

    #endregion

    #region Invoice Management Tests

    [Fact]
    public async Task VoidInvoice_Should_Mark_Invoice_As_Void()
    {
        // Arrange
        var order = await _testDataBuilder.CreatePaidOrderAsync();
        var voidReason = "Customer request for cancellation";
        
        // Act
        await _orderAppService.VoidInvoice(order.Id, voidReason);
        
        // Assert
        var voidedOrder = await _orderAppService.GetAsync(order.Id);
        voidedOrder.IsVoidInvoice.ShouldBeTrue();
        voidedOrder.VoidReason.ShouldBe(voidReason);
    }

    [Fact]
    public async Task CreditNoteInvoice_Should_Process_Credit_Note()
    {
        // Arrange
        var order = await _testDataBuilder.CreatePaidOrderAsync();
        var creditNoteReason = "Product defect";
        
        // Act
        await _orderAppService.CreditNoteInvoice(order.Id, creditNoteReason);
        
        // Assert
        var creditNoteOrder = await _orderAppService.GetAsync(order.Id);
        creditNoteOrder.CreditNoteReason.ShouldBe(creditNoteReason);
    }

    #endregion

    #region Business Rule Validation Tests

    [Fact]
    public async Task GenerateMerchantTradeNo_Should_Meet_ECPay_Requirements()
    {
        // Arrange
        var orderNo = "202501170001";
        
        // Act
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(orderNo);
        
        // Assert
        merchantTradeNo.ShouldNotBeNullOrEmpty();
        merchantTradeNo.Length.ShouldBeLessThanOrEqualTo(20); // ECPay限制
        merchantTradeNo.ShouldContain(orderNo);
        
        // Should be alphanumeric only
        merchantTradeNo.ShouldSatisfyAllConditions(
            s => s.All(c => char.IsLetterOrDigit(c))
        );
    }

    [Fact]
    public async Task Order_Should_Require_Valid_Contact_Information()
    {
        // Arrange
        var input = new CreateUpdateOrderDto
        {
            GroupBuyId = TestData.GroupBuy1Id,
            CustomerName = "", // Invalid - empty name
            CustomerPhone = "invalid", // Invalid phone format
            CustomerEmail = "invalid-email", // Invalid email format
            PaymentMethod = PaymentMethods.CashOnDelivery,
            InvoiceType = InvoiceType.PersonalInvoice,
            TotalQuantity = 1,
            TotalAmount = 100,
            OrderItems = new List<CreateUpdateOrderItemDto>()
        };
        
        // Act & Assert
        await Should.ThrowAsync<Exception>(async () =>
            await _orderAppService.CreateAsync(input)
        );
    }

    #endregion
}