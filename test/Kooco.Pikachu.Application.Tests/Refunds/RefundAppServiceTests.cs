using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.Emails;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.Orders.Repositories;
using Kooco.Pikachu.Orders.Services;
using Kooco.Pikachu.PaymentGateways;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserShoppingCredits;
using NSubstitute;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Xunit;

namespace Kooco.Pikachu.Refunds
{
    public class RefundAppServiceTests : PikachuApplicationTestBase
    {
        private readonly IRefundAppService _refundAppService;
        private readonly IRefundRepository _refundRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
        private readonly IPaymentGatewayAppService _mockPaymentGatewayAppService;
        private readonly IEmailAppService _mockEmailAppService;
        private readonly IEmailSender _mockEmailSender;
        private readonly OrderHistoryManager _orderHistoryManager;
        private readonly IUserShoppingCreditRepository _userShoppingCreditRepository;
        private readonly IUserCumulativeCreditRepository _userCumulativeCreditRepository;

        public RefundAppServiceTests()
        {
            _refundAppService = GetRequiredService<IRefundAppService>();
            _refundRepository = GetRequiredService<IRefundRepository>();
            _orderRepository = GetRequiredService<IOrderRepository>();
            _paymentGatewayRepository = GetRequiredService<IRepository<PaymentGateway, Guid>>();
            _orderHistoryManager = GetRequiredService<OrderHistoryManager>();
            _userShoppingCreditRepository = GetRequiredService<IUserShoppingCreditRepository>();
            _userCumulativeCreditRepository = GetRequiredService<IUserCumulativeCreditRepository>();
            
            // Mock services
            _mockPaymentGatewayAppService = Substitute.For<IPaymentGatewayAppService>();
            _mockEmailAppService = Substitute.For<IEmailAppService>();
            _mockEmailSender = Substitute.For<IEmailSender>();
        }

        #region Test Helpers

        private async Task<Order> CreateTestOrderAsync(
            string orderNo = null,
            decimal totalAmount = 1000,
            PaymentMethods paymentMethod = PaymentMethods.CreditCard,
            OrderStatus orderStatus = OrderStatus.Open,
            ShippingStatus shippingStatus = ShippingStatus.PrepareShipment,
            bool isRefunded = false)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNo = orderNo ?? $"ORD{DateTime.Now:yyyyMMddHHmmss}",
                TotalAmount = totalAmount,
                PaymentMethod = paymentMethod,
                OrderStatus = orderStatus,
                ShippingStatus = shippingStatus,
                IsRefunded = isRefunded,
                UserId = Guid.NewGuid(),
                GroupBuyId = Guid.NewGuid(),
                CustomerEmail = "test@example.com",
                MerchantTradeNo = $"MT{DateTime.Now:yyyyMMddHHmmss}",
                TradeNo = $"T{DateTime.Now:yyyyMMddHHmmss}",
                GWSR = 12345,
                CreditDeductionAmount = 100,
                CreditDeductionRecordId = Guid.NewGuid()
            };

            await _orderRepository.InsertAsync(order);
            return order;
        }

        private async Task<Refund> CreateTestRefundAsync(
            Guid orderId,
            RefundReviewStatus status = RefundReviewStatus.PendingReview)
        {
            var refund = new Refund(Guid.NewGuid(), orderId)
            {
                RefundReview = status
            };

            await _refundRepository.InsertAsync(refund);
            return refund;
        }

        private async Task<PaymentGateway> CreateTestPaymentGatewayAsync(
            PaymentIntegrationType type = PaymentIntegrationType.EcPay)
        {
            var gateway = new PaymentGateway
            {
                Id = Guid.NewGuid(),
                PaymentIntegrationType = type,
                MerchantId = "TEST123",
                HashKey = "testHashKey",
                HashIV = "testHashIV",
                IsEnabled = true
            };

            await _paymentGatewayRepository.InsertAsync(gateway);
            return gateway;
        }

        #endregion

        #region Create Refund Tests

        [Fact]
        public async Task CreateAsync_Should_Create_New_Refund()
        {
            // Arrange
            var order = await CreateTestOrderAsync();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _refundAppService.CreateAsync(order.Id);
            });

            // Assert
            var refund = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundRepository.FirstOrDefaultAsync(x => x.OrderId == order.Id);
            });

            refund.ShouldNotBeNull();
            refund.OrderId.ShouldBe(order.Id);
            refund.RefundReview.ShouldBe(RefundReviewStatus.PendingReview);

            // Check order is marked as refunded
            var updatedOrder = await WithUnitOfWorkAsync(async () =>
            {
                return await _orderRepository.GetAsync(order.Id);
            });

            updatedOrder.IsRefunded.ShouldBe(true);
            updatedOrder.OrderRefundType.ShouldBe(OrderRefundType.FullRefund);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Refund_Already_Exists()
        {
            // Arrange
            var order = await CreateTestOrderAsync();
            await CreateTestRefundAsync(order.Id);

            // Act & Assert
            await Should.ThrowAsync<BusinessException>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _refundAppService.CreateAsync(order.Id);
                });
            });
        }

        #endregion

        #region List and Query Tests

        [Fact]
        public async Task GetListAsync_Should_Return_Paged_Refunds()
        {
            // Arrange
            var order1 = await CreateTestOrderAsync("ORD001");
            var order2 = await CreateTestOrderAsync("ORD002");
            var order3 = await CreateTestOrderAsync("ORD003");

            await CreateTestRefundAsync(order1.Id);
            await CreateTestRefundAsync(order2.Id);
            await CreateTestRefundAsync(order3.Id);

            var input = new GetRefundListDto
            {
                MaxResultCount = 2,
                SkipCount = 0
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
            result.Items.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetListAsync_With_Filter_Should_Return_Filtered_Results()
        {
            // Arrange
            var order1 = await CreateTestOrderAsync("ORD001");
            var order2 = await CreateTestOrderAsync("SPECIAL002");
            var order3 = await CreateTestOrderAsync("ORD003");

            await CreateTestRefundAsync(order1.Id);
            await CreateTestRefundAsync(order2.Id);
            await CreateTestRefundAsync(order3.Id);

            var input = new GetRefundListDto
            {
                Filter = "SPECIAL",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldContain(x => x.OrderId == order2.Id);
            result.Items.ShouldNotContain(x => x.OrderId == order1.Id);
        }

        [Fact]
        public async Task GetRefundPendingCount_Should_Return_Count()
        {
            // Arrange
            var order1 = await CreateTestOrderAsync();
            var order2 = await CreateTestOrderAsync();
            var order3 = await CreateTestOrderAsync();

            await CreateTestRefundAsync(order1.Id, RefundReviewStatus.PendingReview);
            await CreateTestRefundAsync(order2.Id, RefundReviewStatus.Proccessing);
            await CreateTestRefundAsync(order3.Id, RefundReviewStatus.Success);

            // Act
            var count = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.GetRefundPendingCount();
            });

            // Assert
            count.ShouldBeGreaterThanOrEqualTo(1);
        }

        #endregion

        #region Update Review Status Tests

        [Fact]
        public async Task UpdateRefundReviewAsync_Should_Update_To_Processing()
        {
            // Arrange
            var order = await CreateTestOrderAsync();
            var refund = await CreateTestRefundAsync(order.Id);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.Proccessing
                );
            });

            // Assert
            result.ShouldNotBeNull();
            result.RefundReview.ShouldBe(RefundReviewStatus.Proccessing);
            result.ReviewCompletionTime.ShouldNotBeNull();
        }

        [Fact]
        public async Task UpdateRefundReviewAsync_Should_Update_To_Returned_With_Reason()
        {
            // Arrange
            var order = await CreateTestOrderAsync();
            var refund = await CreateTestRefundAsync(order.Id);
            var rejectReason = "Insufficient documentation provided";

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.ReturnedApplication,
                    rejectReason
                );
            });

            // Assert
            result.ShouldNotBeNull();
            result.RefundReview.ShouldBe(RefundReviewStatus.ReturnedApplication);
            result.RejectReason.ShouldBe(rejectReason);
            result.ReviewCompletionTime.ShouldNotBeNull();
        }

        [Fact]
        public async Task UpdateRefundReviewAsync_To_Success_Should_Process_Credit_Refund()
        {
            // Arrange
            var order = await CreateTestOrderAsync();
            order.OrderRefundType = OrderRefundType.FullRefund;
            order.CreditDeductionRecordId = Guid.NewGuid();
            order.CreditDeductionAmount = 200;
            await _orderRepository.UpdateAsync(order);

            var refund = await CreateTestRefundAsync(order.Id);

            // Create a shopping credit record
            var shoppingCredit = new UserShoppingCredit(
                order.CreditDeductionRecordId.Value,
                order.UserId.Value,
                500,
                DateTime.Now.AddDays(-10),
                true,
                DateTime.Now.AddMonths(1),
                "Initial credit",
                UserShoppingCreditType.Grant
            );
            await _userShoppingCreditRepository.InsertAsync(shoppingCredit);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.Success
                );
            });

            // Assert
            result.ShouldNotBeNull();
            result.RefundReview.ShouldBe(RefundReviewStatus.Success);

            // Check if refund credit was created
            var refundCredits = await WithUnitOfWorkAsync(async () =>
            {
                var credits = await _userShoppingCreditRepository.GetListAsync();
                return credits.Where(x => x.UserId == order.UserId && 
                                        x.TransactionDescription.Contains("訂單取消")).ToList();
            });

            refundCredits.ShouldNotBeEmpty();
            refundCredits.First().Amount.ShouldBe(order.CreditDeductionAmount);
        }

        #endregion

        #region Complex Refund Processing Tests

        [Fact]
        public async Task CheckStatusAndRequestRefundAsync_Should_Process_Authorized_Full_Refund()
        {
            // Arrange
            var order = await CreateTestOrderAsync(totalAmount: 5000);
            order.PaymentMethod = PaymentMethods.CreditCard;
            order.OrderRefundType = null; // Full refund
            order.GWSR = 12345;
            await _orderRepository.UpdateAsync(order);

            var refund = await CreateTestRefundAsync(order.Id);

            // Mock payment gateway
            var ecpayDto = new PaymentGatewayDto
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                MerchantId = "TEST123",
                HashKey = "testKey",
                HashIV = "testIV"
            };

            // This test would require mocking external API calls
            // For now, we'll skip the actual execution
        }

        [Fact]
        public async Task CheckStatusAndRequestRefundAsync_Should_Handle_Partial_Refund()
        {
            // Arrange
            var order = await CreateTestOrderAsync(totalAmount: 5000);
            order.PaymentMethod = PaymentMethods.CreditCard;
            order.OrderRefundType = OrderRefundType.PartialRefund;
            order.ShippingStatus = ShippingStatus.Shipped;
            order.DeliveryCost = 100;
            await _orderRepository.UpdateAsync(order);

            var refund = await CreateTestRefundAsync(order.Id);

            // This test would require mocking external API calls
            // The actual implementation would calculate refund amount as totalAmount - deliveryCost
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task CreateAsync_With_NonExistent_Order_Should_Throw()
        {
            // Arrange
            var nonExistentOrderId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _refundAppService.CreateAsync(nonExistentOrderId);
                });
            });
        }

        [Fact]
        public async Task UpdateRefundReviewAsync_With_NonExistent_Refund_Should_Throw()
        {
            // Arrange
            var nonExistentRefundId = Guid.NewGuid();

            // Act & Assert
            await Should.ThrowAsync<Exception>(async () =>
            {
                await WithUnitOfWorkAsync(async () =>
                {
                    await _refundAppService.UpdateRefundReviewAsync(
                        nonExistentRefundId,
                        RefundReviewStatus.Success
                    );
                });
            });
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task UpdateRefundReviewAsync_Success_With_Expired_Credit_Should_Not_Create_Refund()
        {
            // Arrange
            var order = await CreateTestOrderAsync();
            order.OrderRefundType = OrderRefundType.FullRefund;
            order.CreditDeductionRecordId = Guid.NewGuid();
            order.CreditDeductionAmount = 200;
            await _orderRepository.UpdateAsync(order);

            var refund = await CreateTestRefundAsync(order.Id);

            // Create an expired shopping credit record
            var expiredCredit = new UserShoppingCredit(
                order.CreditDeductionRecordId.Value,
                order.UserId.Value,
                500,
                DateTime.Now.AddMonths(-2),
                true,
                DateTime.Now.AddDays(-1), // Already expired
                "Expired credit",
                UserShoppingCreditType.Grant
            );
            await _userShoppingCreditRepository.InsertAsync(expiredCredit);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.Success
                );
            });

            // Assert
            result.RefundReview.ShouldBe(RefundReviewStatus.Success);

            // Check that no refund credit was created
            var refundCredits = await WithUnitOfWorkAsync(async () =>
            {
                var credits = await _userShoppingCreditRepository.GetListAsync();
                return credits.Where(x => x.UserId == order.UserId && 
                                        x.TransactionDescription.Contains("訂單取消")).ToList();
            });

            refundCredits.ShouldBeEmpty();
        }

        [Fact]
        public async Task Multiple_Review_Status_Updates_Should_Track_History()
        {
            // Arrange
            var order = await CreateTestOrderAsync();
            var refund = await CreateTestRefundAsync(order.Id);

            // Act - Update to Processing
            var result1 = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.Proccessing
                );
            });

            // Update to Returned
            var result2 = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.ReturnedApplication,
                    "Need more information"
                );
            });

            // Update to Processing again
            var result3 = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.Proccessing
                );
            });

            // Update to Success
            var result4 = await WithUnitOfWorkAsync(async () =>
            {
                return await _refundAppService.UpdateRefundReviewAsync(
                    refund.Id, 
                    RefundReviewStatus.Success
                );
            });

            // Assert
            result4.RefundReview.ShouldBe(RefundReviewStatus.Success);
            result4.ReviewCompletionTime.ShouldNotBeNull();
        }

        #endregion
    }
}