using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.Response;
using Kooco.Pikachu.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Xunit;

namespace Kooco.Pikachu.Orders;

/// <summary>
/// Extended tests for OrderAppService to improve coverage
/// </summary>
public class OrderAppServiceExtendedTests : PikachuApplicationTestBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<GroupBuy, Guid> _groupBuyRepository;
    private readonly IDataFilter _dataFilter;
    private readonly ICurrentTenant _currentTenant;
    private readonly TestOrderDataBuilder _testDataBuilder;

    public OrderAppServiceExtendedTests()
    {
        _orderAppService = GetRequiredService<IOrderAppService>();
        _orderRepository = GetRequiredService<IRepository<Order, Guid>>();
        _groupBuyRepository = GetRequiredService<IRepository<GroupBuy, Guid>>();
        _dataFilter = GetRequiredService<IDataFilter>();
        _currentTenant = GetRequiredService<ICurrentTenant>();
        _testDataBuilder = new TestOrderDataBuilder(_orderAppService);
    }

    #region Order Status Management Tests

    [Fact]
    public async Task OrderShipped_Should_Update_Status()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        var result = await _orderAppService.OrderShipped(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShippingStatus.ShouldBe(ShippingStatus.Shipped);
        result.ShippingDate.ShouldNotBeNull();
    }

    [Fact]
    public async Task OrderComplete_Should_Update_Status()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        await _orderAppService.OrderShipped(order.Id); // Must be shipped first
        
        // Act
        var result = await _orderAppService.OrderComplete(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.ShippingStatus.ShouldBe(ShippingStatus.Completed);
        result.CompletionTime.ShouldNotBeNull();
    }

    [Fact]
    public async Task CancelOrderAsync_Should_Cancel_Order()
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
        var order = await CreateTestOrderAsync();
        await _orderAppService.OrderShipped(order.Id);
        
        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
            await _orderAppService.CancelOrderAsync(order.Id)
        );
    }

    #endregion

    #region Payment Management Tests

    [Fact]
    public async Task UpdateOrderPaymentMethodAsync_Should_Update_Payment_Method()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var request = new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            PaymentMethod = PaymentMethods.CreditCard
        };
        
        // Act
        var result = await _orderAppService.UpdateOrderPaymentMethodAsync(request);
        
        // Assert
        result.ShouldNotBeNull();
        result.PaymentMethod.ShouldBe(PaymentMethods.CreditCard);
    }

    [Fact]
    public async Task HandlePaymentAsync_Should_Process_Successful_Payment()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var paymentResult = new PaymentResult
        {
            MerchantID = "TestMerchant",
            MerchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo),
            PaymentDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            PaymentType = "Credit_CreditCard",
            RtnCode = 1,
            RtnMsg = "Success",
            SimulatePaid = 0,
            StoreID = "TestStore",
            TradeAmt = (int)order.TotalAmount,
            TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            TradeNo = "TestTradeNo" + Guid.NewGuid().ToString("N")[..8],
            CheckMacValue = "TestCheckMacValue"
        };
        
        // Act
        var result = await _orderAppService.HandlePaymentAsync(paymentResult);
        
        // Assert
        result.ShouldBe("1|OK");
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.OrderStatus.ShouldBe(OrderStatus.Open);
    }

    [Fact]
    public async Task HandlePaymentAsync_Should_Handle_Failed_Payment()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var paymentResult = new PaymentResult
        {
            MerchantID = "TestMerchant",
            MerchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo),
            RtnCode = 0, // Failed payment
            RtnMsg = "Payment Failed",
            TradeAmt = (int)order.TotalAmount
        };
        
        // Act
        var result = await _orderAppService.HandlePaymentAsync(paymentResult);
        
        // Assert
        result.ShouldBe("0|Error");
    }

    #endregion

    #region Order Query Tests

    [Fact]
    public async Task GetAsync_Should_Return_Order()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        var result = await _orderAppService.GetAsync(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
        result.OrderNo.ShouldBe(order.OrderNo);
    }

    [Fact]
    public async Task GetWithDetailsAsync_Should_Return_Order_With_All_Details()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        var result = await _orderAppService.GetWithDetailsAsync(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
        result.OrderItems.ShouldNotBeEmpty();
        result.GroupBuy.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Orders()
    {
        // Arrange
        await CreateTestOrderAsync();
        await CreateTestOrderAsync();
        
        var input = new GetOrderListDto
        {
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetOrderIdAsync_Should_Return_Order_Id_By_OrderNo()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        var orderId = await _orderAppService.GetOrderIdAsync(order.OrderNo);
        
        // Assert
        orderId.ShouldBe(order.Id);
    }

    #endregion

    #region Refund and Return Tests

    [Fact]
    public async Task RefundOrderItems_Should_Process_Refund()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var orderItemIds = order.OrderItems.Select(x => x.Id).ToList();
        
        // Act
        var result = await _orderAppService.RefundOrderItems(orderItemIds, order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        var refundedOrder = await _orderAppService.GetWithDetailsAsync(order.Id);
        // Verify order is refunded
        refundedOrder.IsRefunded.ShouldBeTrue();
    }

    [Fact]
    public async Task RefundAmountAsync_Should_Process_Partial_Refund()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var refundAmount = order.TotalAmount / 2;
        
        // Act
        await _orderAppService.RefundAmountAsync((double)refundAmount, order.Id);
        
        // Assert
        var refundedOrder = await _orderAppService.GetAsync(order.Id);
        refundedOrder.RefundAmount.ShouldBe((int)refundAmount);
    }

    [Fact]
    public async Task ChangeReturnStatusAsync_Should_Update_Return_Status()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        await _orderAppService.ChangeReturnStatusAsync(order.Id, OrderReturnStatus.Processing, false);
        
        // Assert
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.ReturnStatus.ShouldBe(OrderReturnStatus.Processing);
    }

    #endregion

    #region Order Update Tests

    [Fact]
    public async Task UpdateOrderItemsAsync_Should_Update_Order_Items()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var updateItems = order.OrderItems.Select(x => new UpdateOrderItemDto
        {
            Id = x.Id,
            Quantity = x.Quantity + 1,
            ItemPrice = x.ItemPrice
        }).ToList();
        
        // Act
        await _orderAppService.UpdateOrderItemsAsync(order.Id, updateItems);
        
        // Assert
        var updatedOrder = await _orderAppService.GetWithDetailsAsync(order.Id);
        updatedOrder.TotalQuantity.ShouldBe(order.TotalQuantity + order.OrderItems.Count);
    }

    [Fact]
    public async Task AddStoreCommentAsync_Should_Add_Comment()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var comment = "Test store comment";
        
        // Act
        await _orderAppService.AddStoreCommentAsync(order.Id, comment);
        
        // Assert
        var updatedOrder = await _orderAppService.GetWithDetailsAsync(order.Id);
        updatedOrder.StoreComments.ShouldContain(c => c.Comment == comment);
    }

    #endregion

    #region Helper Methods

    private async Task<OrderDto> CreateTestOrderAsync()
    {
        return await _testDataBuilder.CreateOrderWithMultipleItemsAsync(1);
    }

    #endregion
}