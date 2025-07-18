using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.TestHelpers;
using Shouldly;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kooco.Pikachu.Orders;

/// <summary>
/// Tests for OrderAppService query functionality
/// </summary>
public class OrderAppServiceQueryTests : PikachuApplicationTestBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly TestOrderDataBuilder _testDataBuilder;

    public OrderAppServiceQueryTests()
    {
        _orderAppService = GetRequiredService<IOrderAppService>();
        _testDataBuilder = new TestOrderDataBuilder(_orderAppService);
    }

    #region Order Query Tests

    [Fact]
    public async Task GetAsync_Should_Return_Order_By_Id()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        
        // Act
        var result = await _orderAppService.GetAsync(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
        result.OrderNo.ShouldBe(order.OrderNo);
        result.CustomerName.ShouldBe("Test Customer");
        result.TotalAmount.ShouldBe(100);
    }

    [Fact]
    public async Task GetWithDetailsAsync_Should_Return_Order_With_All_Relations()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        
        // Act
        var result = await _orderAppService.GetWithDetailsAsync(order.Id);
        
        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
        result.OrderItems.ShouldNotBeNull();
        result.OrderItems.Count.ShouldBe(1);
        result.OrderItems[0].ItemId.ShouldBe(TestData.Item1Id);
        result.GroupBuy.ShouldNotBeNull();
        result.GroupBuy.Id.ShouldBe(TestData.GroupBuy1Id);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Orders()
    {
        // Arrange
        await _testDataBuilder.CreateBasicOrderAsync();
        await _testDataBuilder.CreateBasicOrderAsync(PaymentMethods.CreditCard);
        await _testDataBuilder.CreateBasicOrderAsync(PaymentMethods.BankTransfer);
        
        var input = new GetOrderListDto
        {
            MaxResultCount = 2,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        result.Items.Count.ShouldBe(2); // Due to MaxResultCount
    }

    [Fact]
    public async Task GetListAsync_Should_Filter_By_OrderStatus()
    {
        // Arrange
        var unpaidOrder = await _testDataBuilder.CreateBasicOrderAsync();
        var paidOrder = await _testDataBuilder.CreatePaidOrderAsync();
        
        var input = new GetOrderListDto
        {
            OrderStatus = OrderStatus.Open,
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldContain(x => x.Id == paidOrder.Id);
        result.Items.ShouldAllBe(x => x.OrderStatus == OrderStatus.Open);
    }

    [Fact]
    public async Task GetListAsync_Should_Filter_By_PaymentMethod()
    {
        // Arrange
        await _testDataBuilder.CreateBasicOrderAsync(PaymentMethods.CashOnDelivery);
        await _testDataBuilder.CreateBasicOrderAsync(PaymentMethods.CreditCard);
        await _testDataBuilder.CreateBasicOrderAsync(PaymentMethods.BankTransfer);
        
        var input = new GetOrderListDto
        {
            Filter = "CreditCard",
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldAllBe(x => x.PaymentMethod == PaymentMethods.CreditCard);
    }

    [Fact]
    public async Task GetOrderIdAsync_Should_Return_OrderId_By_OrderNo()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        
        // Act
        var orderId = await _orderAppService.GetOrderIdAsync(order.OrderNo);
        
        // Assert
        orderId.ShouldBe(order.Id);
    }

    [Fact]
    public async Task GetReturnListAsync_Should_Return_Only_Return_Orders()
    {
        // Arrange
        var normalOrder = await _testDataBuilder.CreateBasicOrderAsync();
        var returnOrder = await _testDataBuilder.CreateBasicOrderAsync();
        
        // Set one order to return status
        await _orderAppService.ChangeReturnStatusAsync(returnOrder.Id, OrderReturnStatus.Processing, false);
        
        var input = new GetOrderListDto
        {
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetReturnListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldContain(x => x.Id == returnOrder.Id);
        result.Items.ShouldNotContain(x => x.Id == normalOrder.Id);
    }

    #endregion

    #region Order Statistics Tests

    [Fact]
    public async Task GetTotalDeliveryTemperatureCountsAsync_Should_Return_Temperature_Counts()
    {
        // Arrange
        await _testDataBuilder.CreateBasicOrderAsync(); // Normal temperature
        await _testDataBuilder.CreateBasicOrderAsync(); // Normal temperature
        
        // Act
        var (normalCount, freezeCount, frozenCount) = await _orderAppService.GetTotalDeliveryTemperatureCountsAsync();
        
        // Assert
        normalCount.ShouldBeGreaterThanOrEqualTo(2);
        freezeCount.ShouldBeGreaterThanOrEqualTo(0);
        frozenCount.ShouldBeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetReturnOrderNotificationCount_Should_Return_Count()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        await _orderAppService.ChangeReturnStatusAsync(order.Id, OrderReturnStatus.Pending, false);
        
        // Act
        var count = await _orderAppService.GetReturnOrderNotificationCount();
        
        // Assert
        count.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Order Export Tests

    [Fact]
    public async Task GetListAsExcelFileAsync_Should_Return_Excel_File()
    {
        // Arrange
        await _testDataBuilder.CreateBasicOrderAsync();
        await _testDataBuilder.CreateBasicOrderAsync();
        
        var input = new GetOrderListDto
        {
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsExcelFileAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.ContentType.ShouldBe("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        result.FileName.ShouldContain(".xlsx");
    }

    [Fact]
    public async Task GetReconciliationListAsExcelFileAsync_Should_Return_Excel_File()
    {
        // Arrange
        await _testDataBuilder.CreatePaidOrderAsync();
        
        var input = new GetOrderListDto
        {
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetReconciliationListAsExcelFileAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.ContentType.ShouldBe("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    #endregion

    #region Order Search Tests

    [Fact]
    public async Task GetListAsync_Should_Filter_By_OrderNo()
    {
        // Arrange
        var order = await _testDataBuilder.CreateBasicOrderAsync();
        
        var input = new GetOrderListDto
        {
            Filter = order.OrderNo,
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBe(1);
        result.Items[0].OrderNo.ShouldBe(order.OrderNo);
    }

    [Fact]
    public async Task GetListAsync_Should_Filter_By_Customer_Name()
    {
        // Arrange
        await _testDataBuilder.CreateBasicOrderAsync();
        
        var input = new GetOrderListDto
        {
            Filter = "Test Customer",
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.ShouldAllBe(x => x.CustomerName.Contains("Test Customer"));
    }

    [Fact]
    public async Task GetListAsync_Should_Sort_By_CreationTime_Desc()
    {
        // Arrange
        var order1 = await _testDataBuilder.CreateBasicOrderAsync();
        await Task.Delay(100); // Ensure different creation times
        var order2 = await _testDataBuilder.CreateBasicOrderAsync();
        
        var input = new GetOrderListDto
        {
            Sorting = "creationTime desc",
            MaxResultCount = 10,
            SkipCount = 0
        };
        
        // Act
        var result = await _orderAppService.GetListAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
        
        // Find the indices of our orders
        var itemsList = result.Items.ToList();
        var index1 = itemsList.FindIndex(x => x.Id == order1.Id);
        var index2 = itemsList.FindIndex(x => x.Id == order2.Id);
        
        // Order2 should come before Order1 when sorted by creation time desc
        if (index1 >= 0 && index2 >= 0)
        {
            index2.ShouldBeLessThan(index1);
        }
    }

    #endregion
}