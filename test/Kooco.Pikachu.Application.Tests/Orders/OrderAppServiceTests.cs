using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Items;
using Kooco.Pikachu.OrderItems;
using Shouldly;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Xunit;

namespace Kooco.Pikachu.Orders;

public class OrderAppServiceTests : PikachuApplicationTestBase
{
    private readonly IOrderAppService _orderAppService;
    private readonly ISetItemAppService _setItemAppService;

    public OrderAppServiceTests()
    {
        _orderAppService = GetRequiredService<IOrderAppService>();
        _setItemAppService = GetRequiredService<ISetItemAppService>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create()
    {
        var input = GetInput();
        var order = await _orderAppService.CreateAsync(input);

        order.Id.ShouldNotBe(Guid.Empty);
        order.TotalAmount.ShouldBe(input.TotalAmount);
        order.TotalQuantity.ShouldBe(input.TotalQuantity);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Item()
    {
        var input = GetInput();
        input.OrderItems.Add(GetOrderItemWithItem());

        var order = await _orderAppService.CreateAsync(input);

        order.Id.ShouldNotBe(Guid.Empty);
        order.TotalAmount.ShouldBe(input.TotalAmount);
        order.TotalQuantity.ShouldBe(input.TotalQuantity);

        order.OrderItems.Count.ShouldBe(1);

        var inputOrderItem = input.OrderItems[0];
        var orderItem = order.OrderItems[0];

        ValidateOrderItem(orderItem, inputOrderItem);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Set_Item()
    {
        var input = GetInput();
        input.OrderItems.Add(GetOrderItemWithSetItem());

        var order = await _orderAppService.CreateAsync(input);

        order.Id.ShouldNotBe(Guid.Empty);
        order.TotalAmount.ShouldBe(input.TotalAmount);
        order.TotalQuantity.ShouldBe(input.TotalQuantity);

        order.OrderItems.Count.ShouldBe(1);

        var inputOrderItem = input.OrderItems[0];
        var orderItem = order.OrderItems[0];

        ValidateOrderItem(orderItem, inputOrderItem);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_With_Both_Item_And_Set_Item()
    {
        var input = GetInput();
        input.OrderItems.Add(GetOrderItemWithItem());
        input.OrderItems.Add(GetOrderItemWithSetItem());

        var order = await _orderAppService.CreateAsync(input);

        order.Id.ShouldNotBe(Guid.Empty);
        order.TotalAmount.ShouldBe(input.TotalAmount);
        order.TotalQuantity.ShouldBe(input.TotalQuantity);

        order.OrderItems.Count.ShouldBe(2);

        for (int i = 0; i < input.OrderItems.Count; i++)
        {
            var inputOrderItem = input.OrderItems[i];
            var orderItem = order.OrderItems[i];
            ValidateOrderItem(orderItem, inputOrderItem);
        }
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_For_Insufficient_Item_Quantity()
    {
        var order = GetInput();
        var item = GetOrderItemWithItem();
        item.Quantity = int.MaxValue;

        order.OrderItems.Add(item);

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _orderAppService.CreateAsync(order);
        });
        exception.Code.ShouldBe("409");
        exception.Message.ShouldContain("Insufficient stock for the following items:");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_For_Insufficient_Set_Item_Quantity()
    {
        var order = GetInput();
        var item = GetOrderItemWithSetItem();
        item.Quantity = int.MaxValue;

        order.OrderItems.Add(item);

        var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _orderAppService.CreateAsync(order);
        });
        exception.Code.ShouldBe("409");
        exception.Message.ShouldContain("Insufficient stock for the following items:");
    }

    private static void ValidateOrderItem(OrderItemDto orderItem, CreateUpdateOrderItemDto inputOrderItem)
    {
        if (orderItem.ItemType == ItemType.Item)
        {
            orderItem.ItemId.ShouldBe(inputOrderItem.ItemId);
        }
        else
        {
            orderItem.SetItemId.ShouldBe(inputOrderItem.SetItemId);
        }
        orderItem.ItemType.ShouldBe(inputOrderItem.ItemType);
        orderItem.Spec.ShouldBe(inputOrderItem.Spec);
        orderItem.Quantity.ShouldBe(inputOrderItem.Quantity);
        orderItem.ItemPrice.ShouldBe(inputOrderItem.ItemPrice);
        orderItem.TotalAmount.ShouldBe(inputOrderItem.TotalAmount);
        orderItem.SKU.ShouldBe(inputOrderItem.SKU);
        orderItem.DeliveryTemperature.ShouldBe(inputOrderItem.DeliveryTemperature);
    }

    #region INPUT
    private static CreateUpdateOrderDto GetInput()
    {
        return new CreateUpdateOrderDto
        {
            GroupBuyId = TestData.GroupBuy1Id,
            CustomerName = "Customer One",
            CustomerPhone = "0900786010",
            CustomerEmail = "customer@one.com",
            PaymentMethod = PaymentMethods.CashOnDelivery,
            InvoiceType = InvoiceType.PersonalInvoice,
            UniformNumber = "Uniform",
            CarrierId = "CarrierId",
            TaxTitle = "Customer One",
            IsAsSameBuyer = true,
            RecipientName = "Customer One",
            RecipientPhone = "0900786010",
            RecipientEmail = "customer@one.com",
            DeliveryMethod = DeliveryMethod.PostOffice,
            PostalCode = "A1A1A1",
            City = "Taipei",
            AddressDetails = "Apt 404, Taipei",
            Remarks = "DELIVER ON TIME",
            ReceivingTime = ReceivingTime.Weekday9To13,
            TotalQuantity = 1,
            TotalAmount = 130,
            DeliveryCost = 30,
            IsTest = true,
            OrderItems = []
        };
    }

    private static CreateUpdateOrderItemDto GetOrderItemWithItem()
    {
        return new CreateUpdateOrderItemDto
        {
            ItemId = TestData.Item1Id,
            ItemType = ItemType.Item,
            Spec = "ITEM",
            Quantity = 1,
            ItemPrice = 100,
            TotalAmount = 100,
            SKU = "ITEM",
            DeliveryTemperature = ItemStorageTemperature.Normal
        };
    }

    private static CreateUpdateOrderItemDto GetOrderItemWithSetItem()
    {
        return new CreateUpdateOrderItemDto
        {
            SetItemId = TestData.SetItem1Id,
            ItemType = ItemType.SetItem,
            Spec = "SET_ITEM",
            Quantity = 2,
            ItemPrice = 50,
            TotalAmount = 100,
            SKU = "SET_ITEM",
            DeliveryTemperature = ItemStorageTemperature.Normal
        };
    }
    #endregion
}
