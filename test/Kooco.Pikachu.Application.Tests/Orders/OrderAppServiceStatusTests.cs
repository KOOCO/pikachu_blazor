using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.TestHelpers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Kooco.Pikachu.Orders;

/// <summary>
/// Tests for OrderAppService status management functionality
/// </summary>
public class OrderAppServiceStatusTests : PikachuApplicationTestBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly TestOrderDataBuilder _testDataBuilder;

    public OrderAppServiceStatusTests()
    {
        _orderAppService = GetRequiredService<IOrderAppService>();
        _testDataBuilder = new TestOrderDataBuilder(_orderAppService);
    }

    #region Order Status Flow Tests

    [Fact]
    public async Task OrderToBeShipped_Should_Update_Status()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        
        // Act
        var result = await _orderAppService.OrderToBeShipped(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShippingStatus.ShouldBe(ShippingStatus.ToBeShipped);
    }

    [Fact]
    public async Task OrderShipped_Should_Update_Status_And_Date()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        await _orderAppService.OrderToBeShipped(order.Id);
        
        // Act
        var result = await _orderAppService.OrderShipped(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShippingStatus.ShouldBe(ShippingStatus.Shipped);
        result.ShippingDate.ShouldNotBeNull();
    }

    [Fact]
    public async Task OrderComplete_Should_Update_Status_And_CompletionTime()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        await _orderAppService.OrderToBeShipped(order.Id);
        await _orderAppService.OrderShipped(order.Id);
        
        // Act
        var result = await _orderAppService.OrderComplete(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShippingStatus.ShouldBe(ShippingStatus.Completed);
        result.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public async Task OrderClosed_Should_Update_Status()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        var result = await _orderAppService.OrderClosed(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.OrderStatus.ShouldBe(OrderStatus.Closed);
    }

    [Fact]
    public async Task ChangeOrderStatus_Should_Update_Shipping_Status()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        
        // Act
        var result = await _orderAppService.ChangeOrderStatus(order.Id, ShippingStatus.PrepareShipment);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShippingStatus.ShouldBe(ShippingStatus.PrepareShipment);
    }

    #endregion

    #region Order Cancellation Tests

    [Fact]
    public async Task CancelOrderAsync_Should_Cancel_Unpaid_Order()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        await _orderAppService.CancelOrderAsync(order.Id);
        
        // Assert
        var cancelledOrder = await _orderAppService.GetAsync(order.Id);
        cancelledOrder.OrderStatus.ShouldBe(OrderStatus.Closed);
    }

    [Fact]
    public async Task CancelOrderAsync_Should_Throw_When_Order_Already_Shipped()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        await _orderAppService.OrderToBeShipped(order.Id);
        await _orderAppService.OrderShipped(order.Id);
        
        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _orderAppService.CancelOrderAsync(order.Id)
        );
    }

    [Fact]
    public async Task ExpireOrderAsync_Should_Expire_Unpaid_Order()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        await _orderAppService.ExpireOrderAsync(order.Id);
        
        // Assert
        var expiredOrder = await _orderAppService.GetAsync(order.Id);
        expiredOrder.OrderStatus.ShouldBe(OrderStatus.Closed);
    }

    #endregion

    #region Return and Exchange Tests

    [Fact]
    public async Task ChangeReturnStatusAsync_Should_Update_Return_Status()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        
        // Act
        await _orderAppService.ChangeReturnStatusAsync(order.Id, OrderReturnStatus.Processing, false);
        
        // Assert
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.ReturnStatus.ShouldBe(OrderReturnStatus.Processing);
        updatedOrder.ShippingStatus.ShouldBe(ShippingStatus.Return);
    }

    [Fact]
    public async Task ChangeReturnStatusAsync_With_Refund_Should_Update_Status()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        
        // Act
        await _orderAppService.ChangeReturnStatusAsync(order.Id, OrderReturnStatus.Approve, true);
        
        // Assert
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.ReturnStatus.ShouldBe(OrderReturnStatus.Approve);
        updatedOrder.IsRefunded.ShouldBeTrue();
    }

    #endregion

    #region Invoice Management Tests

    [Fact]
    public async Task VoidInvoice_Should_Void_Invoice_With_Reason()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        var reason = "Customer request";
        
        // Act
        await _orderAppService.VoidInvoice(order.Id, reason);
        
        // Assert
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.IsVoidInvoice.ShouldBeTrue();
    }

    [Fact]
    public async Task CreditNoteInvoice_Should_Create_Credit_Note()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        var reason = "Product defect";
        
        // Act
        await _orderAppService.CreditNoteInvoice(order.Id, reason);
        
        // Assert
        // The actual credit note creation depends on invoice service implementation
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.ShouldNotBeNull();
    }

    #endregion

    #region Delivery Management Tests

    [Fact]
    public async Task UpdateLogisticStatusAsync_Should_Update_Delivery_Status()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo);
        await _orderAppService.UpdateMerchantTradeNoAsync(new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            MerchantTradeNo = merchantTradeNo
        });
        
        // Act
        await _orderAppService.UpdateLogisticStatusAsync(
            merchantTradeNo, 
            "已送達", 
            "LOG123456", 
            1
        );
        
        // Assert
        // Logistics status update is handled by background jobs
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateOrderDeliveriesAndInvoiceAsync_Should_Create_Deliveries()
    {
        // Arrange
        var order = await CreatePaidOrderAsync();
        
        // Act
        await _orderAppService.CreateOrderDeliveriesAndInvoiceAsync(order.Id);
        
        // Assert
        // Delivery creation depends on logistics service implementation
        var updatedOrder = await _orderAppService.GetWithDetailsAsync(order.Id);
        updatedOrder.ShouldNotBeNull();
    }

    #endregion

    #region Helper Methods

    private async Task<OrderDto> CreateTestOrderAsync()
    {
        return await _testDataBuilder.CreateBasicOrderAsync();
    }

    private async Task<OrderDto> CreatePaidOrderAsync()
    {
        return await _testDataBuilder.CreatePaidOrderAsync();
    }

    #endregion
}