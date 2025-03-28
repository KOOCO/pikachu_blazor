using Autofac.Core;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PikachuAccounts;
using Kooco.Pikachu.ShoppingCredits;
using Kooco.Pikachu.UserAddresses;
using Kooco.Pikachu.UserCumulativeCredits;
using Kooco.Pikachu.UserCumulativeFinancials;
using Kooco.Pikachu.UserCumulativeOrders;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Threading;
using Xunit;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace Kooco.Pikachu.Members
{
    public class MemberAppServiceTests : PikachuApplicationTestBase
    {
        private readonly MemberAppService _memberAppService;
        private readonly IMemberAppService memberAppService;
        private readonly IObjectMapper _objectMapper;  // ✅ Inject ObjectMapper
        // ✅ Mock Dependencies
        private readonly Mock<IMemberRepository> _memberRepositoryMock;
        private readonly IdentityUserManager _identityUserManager;
        private readonly UserAddressManager _userAddressManager;
        private readonly UserCumulativeCreditManager _userCumulativeCreditManager;
        private readonly UserCumulativeOrderManager _userCumulativeOrderManager;
        private readonly UserCumulativeFinancialManager _userCumulativeFinancialManager;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IGroupBuyRepository> _groupBuyRepositoryMock;
     
        private readonly Mock<IUserShoppingCreditAppService> _userShoppingCreditAppServiceMock;
        private readonly Mock<IShoppingCreditEarnSettingAppService> _shoppingCreditEarnSettingAppServiceMock;
        private readonly Mock<IPikachuAccountAppService> _pikachuAccountAppServiceMock;
        private readonly Mock<IUserAddressRepository> _userAddressRepositoryMock;
        private readonly Mock<IUserCumulativeCreditAppService> _userCumulativeCreditAppServiceMock;
        private readonly Mock<IUserCumulativeCreditRepository> _userCumulativeCreditRepositoryMock;
    
        private readonly Mock<IUserCumulativeOrderRepository> _userCumulativeOrderRepositoryMock;
        private readonly Mock<IUserCumulativeFinancialRepository> _userCumulativeFinancialRepositoryMock;

        private readonly Mock<MemberTagManager> _memberTagManager;



        public MemberAppServiceTests()
        {
            // ✅ Initialize Mocks
            _memberRepositoryMock = new Mock<IMemberRepository>();
         
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _groupBuyRepositoryMock = new Mock<IGroupBuyRepository>();
          
            _userShoppingCreditAppServiceMock = new Mock<IUserShoppingCreditAppService>();
            _shoppingCreditEarnSettingAppServiceMock = new Mock<IShoppingCreditEarnSettingAppService>();
            _pikachuAccountAppServiceMock = new Mock<IPikachuAccountAppService>();
            _userAddressRepositoryMock = new Mock<IUserAddressRepository>();
            _userCumulativeCreditAppServiceMock = new Mock<IUserCumulativeCreditAppService>();
            _userCumulativeCreditRepositoryMock = new Mock<IUserCumulativeCreditRepository>();
            _userCumulativeOrderRepositoryMock = new Mock<IUserCumulativeOrderRepository>();
            _userCumulativeFinancialRepositoryMock = new Mock<IUserCumulativeFinancialRepository>();
            _userAddressManager = new UserAddressManager(_userAddressRepositoryMock.Object);
            _userCumulativeCreditManager = new UserCumulativeCreditManager(_userCumulativeCreditRepositoryMock.Object);
            _userCumulativeOrderManager = new UserCumulativeOrderManager(_userCumulativeOrderRepositoryMock.Object);
            _userCumulativeFinancialManager = new UserCumulativeFinancialManager(_userCumulativeFinancialRepositoryMock.Object);
            _memberTagManager = new Mock<MemberTagManager>();

            // ✅ Provide Real `IdentityUserManager` Instead of Mocking
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            var roleRepositoryMock = new Mock<IIdentityRoleRepository>();
            var userRepositoryMock = new Mock<IIdentityUserRepository>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<IdentityUser>>();
            var userValidators = new List<IUserValidator<IdentityUser>>();
            var passwordValidators = new List<IPasswordValidator<IdentityUser>>();
            var keyNormalizerMock = new Mock<ILookupNormalizer>();
            var errorsMock = new Mock<IdentityErrorDescriber>();
            var servicesMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<IdentityUserManager>>();
            var cancellationTokenProviderMock = new Mock<ICancellationTokenProvider>();
            var organizationUnitRepositoryMock = new Mock<IOrganizationUnitRepository>();
            var settingProviderMock = new Mock<ISettingProvider>();
            var distributedEventBusMock = new Mock<IDistributedEventBus>();
            var identityLinkUserRepositoryMock = new Mock<IIdentityLinkUserRepository>();
            var dynamicClaimCacheMock = new Mock<IDistributedCache<AbpDynamicClaimCacheItem>>();

            // ✅ Mock IdentityUserStore Dependencies
          
            var guidGeneratorMock = new Mock<IGuidGenerator>();
            var loggerRoleStoreMock = new Mock<ILogger<IdentityRoleStore>>();
            var lookupNormalizerMock = new Mock<ILookupNormalizer>();
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            // ✅ Create `IdentityUserStore`
            var identityUserStore = new IdentityUserStore(
                userRepositoryMock.Object,
                roleRepositoryMock.Object,
                guidGeneratorMock.Object,
                loggerRoleStoreMock.Object,
                lookupNormalizerMock.Object,
                identityErrorDescriberMock.Object
            );

            _identityUserManager = new IdentityUserManager(
               identityUserStore,
                roleRepositoryMock.Object,
                userRepositoryMock.Object,
                optionsMock.Object,
                passwordHasherMock.Object,
                userValidators,
                passwordValidators,
                keyNormalizerMock.Object,
                errorsMock.Object,
                servicesMock.Object,
                loggerMock.Object,
                cancellationTokenProviderMock.Object,
                organizationUnitRepositoryMock.Object,
                settingProviderMock.Object,
                distributedEventBusMock.Object,
                identityLinkUserRepositoryMock.Object,
                dynamicClaimCacheMock.Object
            );
            _objectMapper = GetRequiredService<IObjectMapper>();
            // ✅ Create MemberAppService Using Mocks
            _memberAppService = new MemberAppService(
                _objectMapper,
                _memberRepositoryMock.Object,
                _identityUserManager,  // ✅ Inject real IdentityUserManager
                _userAddressManager,
                _orderRepositoryMock.Object,
                _groupBuyRepositoryMock.Object,
                _userCumulativeCreditManager,
                _userCumulativeOrderManager,
                _userCumulativeFinancialManager,
                _userShoppingCreditAppServiceMock.Object,
                _shoppingCreditEarnSettingAppServiceMock.Object,
                _pikachuAccountAppServiceMock.Object,
                _userAddressRepositoryMock.Object,
                _userCumulativeCreditAppServiceMock.Object,
                _userCumulativeCreditRepositoryMock.Object,
                _memberTagManager.Object
            );
          


        }

        [Fact]
        public async Task RegisterAsync_Should_Register_Member_Successfully()
        {
            // ✅ Arrange
            var input = new CreateMemberDto { Name = "TestUser",isCallFromTest=true };
            var identityUserDto = new IdentityUserDto { Id = Guid.NewGuid(), UserName = "TestUser" };
            
            _pikachuAccountAppServiceMock
                .Setup(x => x.RegisterAsync(It.IsAny<CreateMemberDto>()))
                .ReturnsAsync(identityUserDto);
            // ✅ Mock ShoppingCreditEarnSettingAppService
            var shoppingCredit = new ShoppingCreditEarnSettingDto
            {
                Id = Guid.NewGuid(),
                RegistrationBonusEnabled = true,
                RegistrationEarnedPoints = 100,
                RegistrationUsagePeriodType = "Fixed",
                RegistrationValidDays = 30,
                BirthdayBonusEnabled = false,
                CashbackEnabled = false,
                SpecificProducts = new List<ShoppingCreditEarnSpecificProductDto>(),
                SpecificGroupBuys = new List<ShoppingCreditEarnSpecificGroupbuyDto>()
            };

            //_shoppingCreditEarnSettingAppServiceMock
            //    .Setup(x => x.GetFirstAsync())
            //    .ReturnsAsync(shoppingCredit);

            // ✅ Act
            var result = await _memberAppService.RegisterAsync(input);

            // ✅ Assert
            result.ShouldNotBeNull();
            result.UserName.ShouldBe("TestUser");
        }
    }
}

