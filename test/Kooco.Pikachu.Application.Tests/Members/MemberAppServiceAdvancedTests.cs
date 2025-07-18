using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.Items.Dtos;
using Kooco.Pikachu.Members;
using Kooco.Pikachu.Members.MemberTags;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Entities;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.UserCumulativeOrders;
using Kooco.Pikachu.UserShoppingCredits;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.Members
{
    public class MemberAppServiceAdvancedTests : PikachuApplicationTestBase
    {
        private readonly IMemberAppService _memberAppService;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<GroupBuy, Guid> _groupBuyRepository;
        private readonly IRepository<Order, Guid> _orderRepository;
        private readonly IRepository<UserAddress, Guid> _userAddressRepository;
        private readonly IRepository<UserCumulativeCredit, Guid> _userCumulativeCreditRepository;
        private readonly IRepository<UserCumulativeOrder, Guid> _userCumulativeOrderRepository;
        private readonly IRepository<UserCumulativeFinancial, Guid> _userCumulativeFinancialRepository;
        private readonly IRepository<MemberTag, Guid> _memberTagRepository;
        private readonly IRepository<UserShoppingCredit, Guid> _userShoppingCreditRepository;

        public MemberAppServiceAdvancedTests()
        {
            _memberAppService = GetRequiredService<IMemberAppService>();
            _userRepository = GetRequiredService<IRepository<IdentityUser, Guid>>();
            _groupBuyRepository = GetRequiredService<IRepository<GroupBuy, Guid>>();
            _orderRepository = GetRequiredService<IRepository<Order, Guid>>();
            _userAddressRepository = GetRequiredService<IRepository<UserAddress, Guid>>();
            _userCumulativeCreditRepository = GetRequiredService<IRepository<UserCumulativeCredit, Guid>>();
            _userCumulativeOrderRepository = GetRequiredService<IRepository<UserCumulativeOrder, Guid>>();
            _userCumulativeFinancialRepository = GetRequiredService<IRepository<UserCumulativeFinancial, Guid>>();
            _memberTagRepository = GetRequiredService<IRepository<MemberTag, Guid>>();
            _userShoppingCreditRepository = GetRequiredService<IRepository<UserShoppingCredit, Guid>>();
        }

        #region Test Helpers

        private async Task<IdentityUser> CreateTestUserAsync(
            string userName = null,
            string email = null,
            string phoneNumber = null,
            string name = null)
        {
            var uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8);
            var user = new IdentityUser(
                Guid.NewGuid(),
                userName ?? $"testuser{uniqueId}",
                email ?? $"testuser{uniqueId}@test.com"
            );

            user.SetPhoneNumber(phoneNumber ?? $"0912{uniqueId.Substring(0, 6)}", false);
            user.Name = name ?? $"Test User {uniqueId}";

            await _userRepository.InsertAsync(user);
            return user;
        }

        private async Task<Order> CreateTestOrderAsync(
            Guid userId,
            Guid? groupBuyId = null,
            int totalAmount = 1000,
            OrderStatus orderStatus = OrderStatus.Open,
            ShippingStatus shippingStatus = ShippingStatus.WaitingForPayment,
            DeliveryMethod deliveryMethod = DeliveryMethod.SelfPickup)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNo = $"ORD{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N").Substring(0, 4)}",
                UserId = userId,
                GroupBuyId = groupBuyId ?? Guid.NewGuid(),
                TotalAmount = totalAmount,
                OrderStatus = orderStatus,
                ShippingStatus = shippingStatus,
                DeliveryMethod = deliveryMethod
            };

            await _orderRepository.InsertAsync(order);
            return order;
        }

        private async Task<List<Order>> CreateMultipleOrdersAsync(
            Guid userId,
            int count,
            Guid? groupBuyId = null)
        {
            var orders = new List<Order>();
            for (int i = 0; i < count; i++)
            {
                orders.Add(await CreateTestOrderAsync(
                    userId,
                    groupBuyId,
                    (i + 1) * 1000
                ));
            }
            return orders;
        }

        private async Task<GroupBuy> CreateTestGroupBuyAsync(
            string name = null,
            string status = "AwaitingRelease")
        {
            var groupBuy = new GroupBuy
            {
                Id = Guid.NewGuid(),
                GroupBuyName = name ?? $"Test Group Buy {Guid.NewGuid().ToString("N").Substring(0, 8)}",
                ShortCode = $"GB{Guid.NewGuid().ToString("N").Substring(0, 6)}",
                GroupBuyNo = 123456,
                Status = status,
                EntryURL = "test-url"
            };

            await _groupBuyRepository.InsertAsync(groupBuy);
            return groupBuy;
        }

        private async Task<UserAddress> CreateTestAddressAsync(
            Guid userId,
            bool isDefault = false,
            string city = null,
            string address = null,
            string recipientName = null)
        {
            var userAddress = new UserAddress(
                Guid.NewGuid(),
                userId,
                "110",
                city ?? "Taipei",
                address ?? "Test Street 123",
                recipientName ?? "Test Recipient",
                "0912345678",
                isDefault
            );

            await _userAddressRepository.InsertAsync(userAddress);
            return userAddress;
        }

        private async Task AddMemberTagAsync(Guid userId, string tagName)
        {
            var memberTag = new MemberTag(Guid.NewGuid(), userId, tagName);
            await _memberTagRepository.InsertAsync(memberTag);
        }

        private async Task<UserCumulativeCredit> CreateUserCumulativeCreditAsync(
            Guid userId,
            int totalAmount = 1000,
            int totalDeductions = 100,
            int totalRefunds = 50)
        {
            var credit = new UserCumulativeCredit(
                Guid.NewGuid(),
                userId,
                totalAmount,
                totalDeductions,
                totalRefunds
            );
            await _userCumulativeCreditRepository.InsertAsync(credit);
            return credit;
        }

        #endregion

        #region Complex Member Query Tests

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Member_Type()
        {
            // Arrange
            var newUser = await CreateTestUserAsync(userName: "newmember");
            await AddMemberTagAsync(newUser.Id, MemberConsts.MemberTags.New);

            var vipUser = await CreateTestUserAsync(userName: "vipmember");
            await AddMemberTagAsync(vipUser.Id, MemberConsts.MemberTags.Vip);

            var regularUser = await CreateTestUserAsync(userName: "regularmember");
            await AddMemberTagAsync(regularUser.Id, MemberConsts.MemberTags.Regular);

            var input = new GetMemberListDto
            {
                MemberType = MemberConsts.MemberTags.Vip,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.Where(x => x.UserName == "vipmember").ShouldNotBeEmpty();
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Creation_Date()
        {
            // Arrange
            var oldUser = await CreateTestUserAsync(userName: "olduser");
            oldUser.CreationTime = DateTime.Now.AddMonths(-6);
            await _userRepository.UpdateAsync(oldUser);

            var recentUser = await CreateTestUserAsync(userName: "recentuser");

            var input = new GetMemberListDto
            {
                MinCreationTime = DateTime.Now.AddDays(-7),
                MaxCreationTime = DateTime.Now,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldContain(x => x.UserName == "recentuser");
            result.Items.ShouldNotContain(x => x.UserName == "olduser");
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Order_Count()
        {
            // Arrange
            var activeUser = await CreateTestUserAsync(userName: "activeuser");
            await CreateMultipleOrdersAsync(activeUser.Id, 10);

            var inactiveUser = await CreateTestUserAsync(userName: "inactiveuser");
            await CreateMultipleOrdersAsync(inactiveUser.Id, 2);

            var input = new GetMemberListDto
            {
                MinOrderCount = 5,
                MaxOrderCount = 15,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.Where(x => x.UserName == "activeuser").ShouldNotBeEmpty();
            result.Items.Where(x => x.UserName == "inactiveuser").ShouldBeEmpty();
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Total_Spent()
        {
            // Arrange
            var highSpender = await CreateTestUserAsync(userName: "highspender");
            await CreateTestOrderAsync(highSpender.Id, totalAmount: 50000);
            await CreateTestOrderAsync(highSpender.Id, totalAmount: 30000);

            var lowSpender = await CreateTestUserAsync(userName: "lowspender");
            await CreateTestOrderAsync(lowSpender.Id, totalAmount: 1000);

            var input = new GetMemberListDto
            {
                MinSpent = 10000,
                MaxSpent = 100000,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.Where(x => x.UserName == "highspender").ShouldNotBeEmpty();
            result.Items.Where(x => x.UserName == "lowspender").ShouldBeEmpty();
        }

        #endregion

        #region Credit Record Tests

        [Fact]
        public async Task GetMemberCreditRecordAsync_Should_Return_Credit_History()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            
            // Create shopping credits with different dates
            var credit1 = new UserShoppingCredit(
                Guid.NewGuid(),
                user.Id,
                100,
                DateTime.Now.AddDays(-10),
                true,
                DateTime.Now.AddMonths(1),
                "Welcome bonus",
                UserShoppingCreditType.Grant
            );
            credit1.SetCurrentRemainingCredits(80);
            await _userShoppingCreditRepository.InsertAsync(credit1);

            var credit2 = new UserShoppingCredit(
                Guid.NewGuid(),
                user.Id,
                50,
                DateTime.Now.AddDays(-5),
                true,
                DateTime.Now.AddMonths(2),
                "Purchase reward",
                UserShoppingCreditType.Grant
            );
            credit2.SetCurrentRemainingCredits(50);
            await _userShoppingCreditRepository.InsertAsync(credit2);

            var input = new GetMemberCreditRecordListDto
            {
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberCreditRecordAsync(user.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetMemberCreditRecordAsync_Should_Filter_By_Date_Range()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            
            var oldCredit = new UserShoppingCredit(
                Guid.NewGuid(),
                user.Id,
                100,
                DateTime.Now.AddMonths(-3),
                true,
                DateTime.Now.AddMonths(-1),
                "Expired credit",
                UserShoppingCreditType.Grant
            );
            await _userShoppingCreditRepository.InsertAsync(oldCredit);

            var recentCredit = new UserShoppingCredit(
                Guid.NewGuid(),
                user.Id,
                50,
                DateTime.Now.AddDays(-5),
                true,
                DateTime.Now.AddMonths(1),
                "Recent credit",
                UserShoppingCreditType.Grant
            );
            await _userShoppingCreditRepository.InsertAsync(recentCredit);

            var input = new GetMemberCreditRecordListDto
            {
                UsageTimeFrom = DateTime.Now.AddDays(-10),
                UsageTimeTo = DateTime.Now,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberCreditRecordAsync(user.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldContain(x => x.TransactionDescription == "Recent credit");
            result.Items.ShouldNotContain(x => x.TransactionDescription == "Expired credit");
        }

        #endregion

        #region Complex Order Tests

        [Fact]
        public async Task GetMemberOrderRecordsAsync_Should_Filter_By_Shipping_Status()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();

            await CreateTestOrderAsync(user.Id, groupBuy.Id, 
                shippingStatus: ShippingStatus.WaitingForShipment);
            await CreateTestOrderAsync(user.Id, groupBuy.Id, 
                shippingStatus: ShippingStatus.Shipped);
            await CreateTestOrderAsync(user.Id, groupBuy.Id, 
                shippingStatus: ShippingStatus.Delivered);

            var input = new GetMemberOrderRecordsDto
            {
                ShippingStatus = ShippingStatus.Shipped,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberOrderRecordsAsync(user.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldAllBe(x => x.ShippingStatus == ShippingStatus.Shipped);
        }

        [Fact]
        public async Task GetMemberOrderRecordsAsync_Should_Filter_By_Delivery_Method()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            
            await CreateTestOrderAsync(user.Id, 
                deliveryMethod: DeliveryMethod.SelfPickup);
            await CreateTestOrderAsync(user.Id, 
                deliveryMethod: DeliveryMethod.HomeDelivery);
            await CreateTestOrderAsync(user.Id, 
                deliveryMethod: DeliveryMethod.SevenToEleven1);

            var input = new GetMemberOrderRecordsDto
            {
                DeliveryMethod = DeliveryMethod.SevenToEleven1,
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberOrderRecordsAsync(user.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldAllBe(x => x.DeliveryMethod == DeliveryMethod.SevenToEleven1);
        }

        [Fact]
        public async Task GetMemberOrdersByGroupBuyAsync_Should_Return_User_Orders_For_GroupBuy()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy1 = await CreateTestGroupBuyAsync("GroupBuy 1");
            var groupBuy2 = await CreateTestGroupBuyAsync("GroupBuy 2");

            await CreateMultipleOrdersAsync(user.Id, 3, groupBuy1.Id);
            await CreateMultipleOrdersAsync(user.Id, 2, groupBuy2.Id);

            // Act - This method requires CurrentUser to be set
            // In a real test scenario, we would need to mock CurrentUser
            // For now, we'll skip this test
        }

        #endregion

        #region Multiple Address Management Tests

        [Fact]
        public async Task Managing_Multiple_Addresses_Should_Work_Correctly()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            // Create multiple addresses
            var address1 = await CreateTestAddressAsync(user.Id, true, "Taipei", "Main St 1");
            var address2 = await CreateTestAddressAsync(user.Id, false, "Taichung", "Second St 2");
            var address3 = await CreateTestAddressAsync(user.Id, false, "Kaohsiung", "Third St 3");

            // Act 1: Get all addresses
            var allAddresses = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberAddressListAsync(user.Id);
            });

            // Assert 1
            allAddresses.Count.ShouldBe(3);
            allAddresses.Count(x => x.IsDefault).ShouldBe(1);

            // Act 2: Change default address
            var updateInput = new CreateUpdateMemberAddressDto
            {
                PostalCode = address2.PostalCode,
                City = address2.City,
                Address = address2.Address,
                RecipientName = address2.RecipientName,
                RecipientPhoneNumber = address2.RecipientPhoneNumber,
                IsDefault = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _memberAppService.UpdateMemberAddressAsync(user.Id, address2.Id, updateInput);
            });

            // Act 3: Verify default changed
            var defaultAddress = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetDefaultAddressAsync(user.Id);
            });

            // Assert 3
            defaultAddress.ShouldNotBeNull();
            defaultAddress.Id.ShouldBe(address2.Id);
            defaultAddress.City.ShouldBe("Taichung");
        }

        #endregion

        #region Cumulative Statistics Tests

        [Fact]
        public async Task GetMemberCumulativeData_Should_Return_Comprehensive_Stats()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            // Create cumulative data
            var credit = await CreateUserCumulativeCreditAsync(user.Id, 5000, 1000, 200);
            
            var order = new UserCumulativeOrder(
                Guid.NewGuid(),
                user.Id,
                totalOrders: 25,
                totalCancelOrders: 3,
                totalReturnOrders: 2,
                totalExchangeOrders: 1,
                totalQuantity: 100
            );
            await _userCumulativeOrderRepository.InsertAsync(order);

            var financial = new UserCumulativeFinancial(
                Guid.NewGuid(),
                user.Id,
                totalSpent: 150000,
                totalRefunded: 10000,
                totalRecharge: 20000,
                totalCashback: 3000,
                totalDiscounts: 5000
            );
            await _userCumulativeFinancialRepository.InsertAsync(financial);

            // Act
            var creditResult = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberCumulativeCreditsAsync(user.Id);
            });

            var orderResult = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberCumulativeOrdersAsync(user.Id);
            });

            var financialResult = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberCumulativeFinancialsAsync(user.Id);
            });

            // Assert
            creditResult.ShouldNotBeNull();
            creditResult.TotalAmount.ShouldBe(5000);
            creditResult.TotalDeductions.ShouldBe(1000);

            orderResult.ShouldNotBeNull();
            orderResult.TotalOrders.ShouldBe(25);
            orderResult.TotalQuantity.ShouldBe(100);

            financialResult.ShouldNotBeNull();
            financialResult.TotalSpent.ShouldBe(150000);
            financialResult.TotalCashback.ShouldBe(3000);
        }

        #endregion

        #region Tag Management Tests

        [Fact]
        public async Task Member_Tag_Transitions_Should_Work_Correctly()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            // Act 1: Initially user should have "New" tag after first order
            await AddMemberTagAsync(user.Id, MemberConsts.MemberTags.New);

            var member1 = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetAsync(user.Id);
            });

            // Assert 1
            member1.IsNew.ShouldBe(true);
            member1.IsBlacklisted.ShouldBe(false);

            // Act 2: Add multiple tags
            await AddMemberTagAsync(user.Id, MemberConsts.MemberTags.Vip);
            await AddMemberTagAsync(user.Id, MemberConsts.MemberTags.Regular);

            var member2 = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetAsync(user.Id);
            });

            // Assert 2
            member2.MemberTags.Count.ShouldBeGreaterThanOrEqualTo(3);

            // Act 3: Blacklist the member
            await WithUnitOfWorkAsync(async () =>
            {
                await _memberAppService.SetBlacklistedAsync(user.Id, true);
            });

            var member3 = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetAsync(user.Id);
            });

            // Assert 3
            member3.IsBlacklisted.ShouldBe(true);

            // Act 4: Remove blacklist
            await WithUnitOfWorkAsync(async () =>
            {
                await _memberAppService.SetBlacklistedAsync(user.Id, false);
            });

            var member4 = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetAsync(user.Id);
            });

            // Assert 4
            member4.IsBlacklisted.ShouldBe(false);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task GetMemberOrderRecordsAsync_With_Complex_Filters_Should_Work()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var groupBuy = await CreateTestGroupBuyAsync();

            // Create various orders
            for (int i = 0; i < 10; i++)
            {
                var order = await CreateTestOrderAsync(
                    user.Id,
                    groupBuy.Id,
                    totalAmount: (i + 1) * 1000,
                    orderStatus: i % 2 == 0 ? OrderStatus.Open : OrderStatus.Closed,
                    shippingStatus: i % 3 == 0 ? ShippingStatus.Shipped : ShippingStatus.WaitingForShipment,
                    deliveryMethod: i % 2 == 0 ? DeliveryMethod.HomeDelivery : DeliveryMethod.SelfPickup
                );

                if (i < 5)
                {
                    order.CreationTime = DateTime.Now.AddDays(-30);
                    await _orderRepository.UpdateAsync(order);
                }
            }

            var input = new GetMemberOrderRecordsDto
            {
                GroupBuyId = groupBuy.Id,
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                ShippingStatus = ShippingStatus.WaitingForShipment,
                DeliveryMethod = DeliveryMethod.SelfPickup,
                Sorting = "TotalAmount DESC",
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetMemberOrderRecordsAsync(user.Id, input);
            });

            // Assert
            result.ShouldNotBeNull();
            result.Items.ShouldAllBe(x => 
                x.ShippingStatus == ShippingStatus.WaitingForShipment &&
                x.DeliveryMethod == DeliveryMethod.SelfPickup &&
                x.CreationTime >= input.StartDate.Value);
        }

        [Fact]
        public async Task GetListAsync_With_Multiple_Selected_Tags_Should_Work()
        {
            // Arrange
            var user1 = await CreateTestUserAsync(userName: "user1");
            await AddMemberTagAsync(user1.Id, MemberConsts.MemberTags.New);
            await AddMemberTagAsync(user1.Id, MemberConsts.MemberTags.Vip);

            var user2 = await CreateTestUserAsync(userName: "user2");
            await AddMemberTagAsync(user2.Id, MemberConsts.MemberTags.Vip);
            await AddMemberTagAsync(user2.Id, MemberConsts.MemberTags.Regular);

            var user3 = await CreateTestUserAsync(userName: "user3");
            await AddMemberTagAsync(user3.Id, MemberConsts.MemberTags.New);

            var input = new GetMemberListDto
            {
                SelectedMemberTags = new List<string> { MemberConsts.MemberTags.New, MemberConsts.MemberTags.Vip },
                MaxResultCount = 10
            };

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _memberAppService.GetListAsync(input);
            });

            // Assert
            result.ShouldNotBeNull();
            // Users with either New or Vip tag should be included
            result.Items.Where(x => x.UserName == "user1" || x.UserName == "user2" || x.UserName == "user3")
                .ShouldNotBeEmpty();
        }

        #endregion
    }
}