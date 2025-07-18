using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.OrderItems;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.PaymentGateways;
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
/// Tests for OrderAppService payment-related functionality
/// </summary>
public class OrderAppServicePaymentTests : PikachuApplicationTestBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly TestOrderDataBuilder _testDataBuilder;

    public OrderAppServicePaymentTests()
    {
        _orderAppService = GetRequiredService<IOrderAppService>();
        _testDataBuilder = new TestOrderDataBuilder(_orderAppService);
    }

    #region Payment Method Tests

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
    public async Task UpdateMerchantTradeNoAsync_Should_Update_Trade_Number()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo);
        var request = new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            MerchantTradeNo = merchantTradeNo
        };
        
        // Act
        var result = await _orderAppService.UpdateMerchantTradeNoAsync(request);
        
        // Assert
        result.ShouldNotBeNull();
        result.MerchantTradeNo.ShouldBe(merchantTradeNo);
    }

    [Fact]
    public async Task HandlePaymentAsync_Should_Process_Successful_Payment()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo);
        
        // Update merchant trade no first
        await _orderAppService.UpdateMerchantTradeNoAsync(new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            MerchantTradeNo = merchantTradeNo
        });
        
        var paymentResult = new PaymentResult
        {
            MerchantID = "TestMerchant",
            MerchantTradeNo = merchantTradeNo,
            PaymentDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            PaymentType = "Credit_CreditCard",
            RtnCode = 1, // Success
            RtnMsg = "Success",
            SimulatePaid = 0,
            StoreID = "TestStore",
            TradeAmt = (int)order.TotalAmount,
            TradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            TradeNo = "TEST" + Guid.NewGuid().ToString("N")[..12],
            CheckMacValue = "TestCheckMacValue"
        };
        
        // Act
        var result = await _orderAppService.HandlePaymentAsync(paymentResult);
        
        // Assert
        result.ShouldBe("1|OK");
        
        // Verify order status updated
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.OrderStatus.ShouldBe(OrderStatus.Open);
    }

    [Fact]
    public async Task HandlePaymentAsync_Should_Handle_Failed_Payment()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(order.OrderNo);
        
        // Update merchant trade no first
        await _orderAppService.UpdateMerchantTradeNoAsync(new OrderPaymentMethodRequest
        {
            OrderId = order.Id,
            MerchantTradeNo = merchantTradeNo
        });
        
        var paymentResult = new PaymentResult
        {
            MerchantID = "TestMerchant",
            MerchantTradeNo = merchantTradeNo,
            RtnCode = 0, // Failed
            RtnMsg = "Payment Failed",
            TradeAmt = (int)order.TotalAmount
        };
        
        // Act
        var result = await _orderAppService.HandlePaymentAsync(paymentResult);
        
        // Assert
        result.ShouldBe("0|Error");
        
        // Verify order status unchanged
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.OrderStatus.ShouldBe(OrderStatus.Open);
    }

    [Fact]
    public void GenerateMerchantTradeNo_Should_Generate_Valid_Format()
    {
        // Arrange
        var orderNo = "202501170001";
        
        // Act
        var merchantTradeNo = _orderAppService.GenerateMerchantTradeNo(orderNo);
        
        // Assert
        merchantTradeNo.ShouldNotBeNullOrEmpty();
        merchantTradeNo.ShouldContain(orderNo);
        merchantTradeNo.Length.ShouldBeLessThanOrEqualTo(20); // ECPay限制
    }

    [Fact]
    public async Task AddCheckMacValueAsync_Should_Store_Mac_Value()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        var checkMacValue = "TestCheckMacValue123";
        
        // Act
        await _orderAppService.AddCheckMacValueAsync(order.Id, checkMacValue);
        
        // Assert
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.CheckMacValue.ShouldBe(checkMacValue);
    }

    #endregion

    #region Manual Bank Transfer Tests

    [Fact]
    public async Task ConfirmManualBankTransferAsync_Should_Update_Order_Status()
    {
        // Arrange
        var order = await CreateTestOrderAsync(PaymentMethods.BankTransfer);
        
        // Act
        await _orderAppService.ConfirmManualBankTransferAsync(order.Id);
        
        // Assert
        var updatedOrder = await _orderAppService.GetAsync(order.Id);
        updatedOrder.OrderStatus.ShouldBe(OrderStatus.Open);
    }

    [Fact]
    public async Task GetManualBankTransferRecordAsync_Should_Return_Transfer_Details()
    {
        // Arrange
        var order = await CreateTestOrderAsync(PaymentMethods.BankTransfer);
        await _orderAppService.ConfirmManualBankTransferAsync(order.Id);
        
        // Act
        var record = await _orderAppService.GetManualBankTransferRecordAsync(order.Id);
        
        // Assert
        record.ShouldNotBeNull();
        record.OrderId.ShouldBe(order.Id);
    }

    #endregion

    #region Payment Gateway Configuration Tests

    [Fact]
    public async Task GetPaymentGatewayConfigurationsAsync_Should_Return_Gateway_Config()
    {
        // Arrange
        var order = await CreateTestOrderAsync();
        
        // Act
        var config = await _orderAppService.GetPaymentGatewayConfigurationsAsync(order.Id);
        
        // Assert
        config.ShouldNotBeNull();
        // The actual values depend on test data setup
    }

    #endregion

    #region Helper Methods

    private async Task<OrderDto> CreateTestOrderAsync(PaymentMethods paymentMethod = PaymentMethods.CashOnDelivery)
    {
        return await _testDataBuilder.CreateBasicOrderAsync(paymentMethod);
    }

    #endregion
}