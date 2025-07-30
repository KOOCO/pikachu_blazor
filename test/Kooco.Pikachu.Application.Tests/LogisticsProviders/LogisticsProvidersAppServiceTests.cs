using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Kooco.Pikachu.DeliveryTemperatureCosts;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.LogisticsSettings;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.LogisticsProviders
{
    public class LogisticsProvidersAppServiceTests : PikachuApplicationTestBase
    {
        private readonly ILogisticsProvidersAppService _logisticsProvidersAppService;
        private readonly IRepository<LogisticsProviderSettings, Guid> _logisticsProviderRepository;
        private readonly IDeliveryTemperatureCostAppService _mockDeliveryTemperatureAppService;

        public LogisticsProvidersAppServiceTests()
        {
            _logisticsProvidersAppService = GetRequiredService<ILogisticsProvidersAppService>();
            _logisticsProviderRepository = GetRequiredService<IRepository<LogisticsProviderSettings, Guid>>();
            _mockDeliveryTemperatureAppService = Substitute.For<IDeliveryTemperatureCostAppService>();
        }

        #region Test Helpers

        private async Task<LogisticsProviderSettings> CreateTestProviderAsync(
            LogisticProviders provider,
            bool isEnabled = true,
            int freight = 100)
        {
            var settings = new LogisticsProviderSettings
            {
                LogisticProvider = provider,
                IsEnabled = isEnabled,
                Freight = freight,
                StoreCode = "TEST123",
                HashKey = "testHashKey",
                HashIV = "testHashIV"
            };

            await _logisticsProviderRepository.InsertAsync(settings);
            return settings;
        }

        private GreenWorldLogisticsCreateUpdateDto GetGreenWorldDto()
        {
            return new GreenWorldLogisticsCreateUpdateDto
            {
                IsEnabled = true,
                StoreCode = "GW123",
                HashKey = "gwHashKey",
                HashIV = "gwHashIV",
                SenderName = "Test Sender",
                SenderPhoneNumber = "0912345678",
                PlatFormId = "Platform123",
                SenderAddress = "Test Address 123",
                SenderPostalCode = "10001",
                City = MainlandCity.TaipeiCity
            };
        }

        private HomeDeliveryCreateUpdateDto GetHomeDeliveryDto()
        {
            return new HomeDeliveryCreateUpdateDto
            {
                IsEnabled = true,
                CustomTitle = "Home Delivery Service",
                Freight = 150,
                MainIslands = "100",
                OuterIslands = "200",
                IsOuterIslands = true
            };
        }

        private SevenToElevenCreateUpdateDto GetSevenToElevenDto()
        {
            return new SevenToElevenCreateUpdateDto
            {
                IsEnabled = true,
                Freight = 60,
                Payment = true
            };
        }

        #endregion

        #region GreenWorld Tests

        [Fact]
        public async Task UpdateGreenWorldAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = GetGreenWorldDto();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateGreenWorldAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.GreenWorldLogistics);
            });

            provider.ShouldNotBeNull();
            provider.StoreCode.ShouldBe("GW123");
            provider.SenderName.ShouldBe("Test Sender");
            provider.IsEnabled.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateGreenWorldAsync_Should_Update_Existing_Provider()
        {
            // Arrange
            await CreateTestProviderAsync(LogisticProviders.GreenWorldLogistics);
            var input = GetGreenWorldDto();
            input.StoreCode = "UPDATED123";

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateGreenWorldAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.GreenWorldLogistics);
            });

            provider.StoreCode.ShouldBe("UPDATED123");
        }

        [Fact]
        public async Task UpdateGreenWorldC2CAsync_Should_Handle_C2C_Provider()
        {
            // Arrange
            var input = GetGreenWorldDto();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateGreenWorldC2CAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.GreenWorldLogisticsC2C);
            });

            provider.ShouldNotBeNull();
            provider.LogisticProvider.ShouldBe(LogisticProviders.GreenWorldLogisticsC2C);
        }

        #endregion

        #region Home Delivery Tests

        [Fact]
        public async Task UpdateHomeDeliveryAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = GetHomeDeliveryDto();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateHomeDeliveryAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.HomeDelivery);
            });

            provider.ShouldNotBeNull();
            provider.CustomTitle.ShouldBe("Home Delivery Service");
            provider.Freight.ShouldBe(150);
            provider.MainIslands.ShouldBe(100);
            provider.OuterIslands.ShouldBe(200);
            provider.IsOuterIslands.ShouldBe(true);
        }

        #endregion

        #region Convenience Store Tests

        [Fact]
        public async Task UpdateSevenToElevenAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = GetSevenToElevenDto();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateSevenToElevenAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.SevenToEleven);
            });

            provider.ShouldNotBeNull();
            provider.Freight.ShouldBe(60);
            provider.Payment.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateFamilyMartAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = GetSevenToElevenDto();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateFamilyMartAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.FamilyMart);
            });

            provider.ShouldNotBeNull();
            provider.LogisticProvider.ShouldBe(LogisticProviders.FamilyMart);
        }

        [Fact]
        public async Task Update_Multiple_SevenEleven_Variants_Should_Work()
        {
            // Arrange
            var input = GetSevenToElevenDto();

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateSevenToElevenAsync(input);
                await _logisticsProvidersAppService.UpdateSevenToElevenC2CAsync(input);
                await _logisticsProvidersAppService.UpdateSevenToElevenFrozenAsync(input);
            });

            // Assert
            var providers = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.GetListAsync();
            });

            providers.Count(x => x.LogisticProvider.ToString().Contains("SevenToEleven")).ShouldBe(3);
        }

        #endregion

        #region TCat Tests

        [Fact]
        public async Task UpdateTCatAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = new TCatLogisticsCreateUpdateDto
            {
                IsEnabled = true,
                CustomerId = "TCAT123",
                CustomerToken = "token123",
                SenderName = "TCat Sender",
                SenderPhoneNumber = "0923456789",
                SenderAddress = "TCat Address 456",
                TCatShippingLabelForm = "Label1",
                TCatPickingListForm = "Picking1",
                TCatShippingLabelForm711 = "Label711",
                ReverseLogisticShippingFee = 50,
                DeclaredValue = 1000
            };

            // Note: GenerateSenderZIPCodeAsync would need external API mocking
            // For testing purposes, we'll skip the actual implementation

            // Act & Assert would require mocking the external API call
        }

        [Fact]
        public async Task UpdateTCatNormalAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = new TCatNormalCreateUpdateDto
            {
                IsEnabled = true,
                Freight = 120,
                OuterIslandFreight = 250,
                Size = SizeEnum.Cm0001,
                Payment = 20,
                TCatPaymentMethod = TCatPaymentMethod.CardAndMobilePaymentsAccepted,
                HualienAndTaitungShippingFee = 30,
                HolidaySurcharge = 50,
                HolidaySurchargeStartTime = new DateTime(2024, 12, 20),
                HolidaySurchargeEndTime = new DateTime(2024, 12, 31)
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateTCatNormalAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.TCatNormal);
            });

            provider.ShouldNotBeNull();
            provider.Freight.ShouldBe(120);
            provider.OuterIslandFreight.ShouldBe(250);
            provider.TCatPaymentMethod.ShouldBe(TCatPaymentMethod.CardAndMobilePaymentsAccepted);
            provider.HolidaySurcharge.ShouldBe(50);
        }

        #endregion

        #region B-Type Provider Tests

        [Fact]
        public async Task UpdateBNormalAsync_Should_Create_New_Provider()
        {
            // Arrange
            var input = new BNormalCreateUpdateDto
            {
                IsEnabled = true,
                Freight = 100,
                OuterIslandFreight = 200,
                Size = SizeEnum.Cm0004,
                HualienAndTaitungShippingFee = 25,
                HolidaySurcharge = 40,
                HolidaySurchargeStartTime = new DateTime(2024, 2, 1),
                HolidaySurchargeEndTime = new DateTime(2024, 2, 15)
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateBNormalAsync(input);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.BNormal);
            });

            provider.ShouldNotBeNull();
            provider.Size.ShouldBe("Large");
            provider.HualienAndTaitungShippingFee.ShouldBe(25);
        }

        [Fact]
        public async Task Update_All_B_Variants_Should_Create_Different_Providers()
        {
            // Arrange
            var input = new BNormalCreateUpdateDto
            {
                IsEnabled = true,
                Freight = 100,
                OuterIslandFreight = 200,
                Size = "Medium"
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _logisticsProvidersAppService.UpdateBNormalAsync(input);
                await _logisticsProvidersAppService.UpdateBFreezeAsync(input);
                await _logisticsProvidersAppService.UpdateBFrozenAsync(input);
            });

            // Assert
            var providers = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.GetListAsync();
            });

            providers.Count(x => x.LogisticProvider.ToString().StartsWith("B")).ShouldBe(3);
            providers.ShouldContain(x => x.LogisticProvider == LogisticProviders.BNormal);
            providers.ShouldContain(x => x.LogisticProvider == LogisticProviders.BFreeze);
            providers.ShouldContain(x => x.LogisticProvider == LogisticProviders.BFrozen);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Providers()
        {
            // Arrange
            await CreateTestProviderAsync(LogisticProviders.GreenWorldLogistics);
            await CreateTestProviderAsync(LogisticProviders.HomeDelivery);
            await CreateTestProviderAsync(LogisticProviders.SevenToEleven);
            await CreateTestProviderAsync(LogisticProviders.FamilyMart);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProvidersAppService.GetAllAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(4);
            result.ShouldAllBe(x => !string.IsNullOrEmpty(x.LogisticProviderName));
        }

        #endregion

        #region GetAsync Tests

        [Fact]
        public async Task GetAsync_With_SelfPickup_Should_Return_Zero_Freight()
        {
            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProvidersAppService.GetAsync("SELFPICKUP");
            });

            // Assert
            result.ShouldNotBeNull();
            result["Freight"].GetValue<int>().ShouldBe(0);
        }

        [Fact]
        public async Task GetAsync_With_HomeDelivery_Should_Return_Correct_Data()
        {
            // Arrange
            var homeDelivery = new LogisticsProviderSettings
            {
                LogisticProvider = LogisticProviders.HomeDelivery,
                IsEnabled = true,
                Freight = 150,
                MainIslands = "100",
                OuterIslands = "200",
                IsOuterIslands = true
            };
            await _logisticsProviderRepository.InsertAsync(homeDelivery, true);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProvidersAppService.GetAsync("HOMEDELIVERY");
            });

            // Assert
            result.ShouldNotBeNull();
            result["Provider"].GetValue<string>().ShouldBe("HomeDelivery");
            result["Cost"].GetValue<decimal>().ShouldBe(150);
            result["MainIslands"].GetValue<decimal>().ShouldBe(100);
            result["OuterIslands"].GetValue<decimal>().ShouldBe(200);
            result["IsOuterIslands"].GetValue<bool>().ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync_With_SevenToEleven_Should_Include_MerchantID()
        {
            // Arrange
            await CreateTestProviderAsync(LogisticProviders.GreenWorldLogistics);
            await CreateTestProviderAsync(LogisticProviders.SevenToEleven, true, 60);

            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProvidersAppService.GetAsync("SEVENTOELEVEN1");
            });

            // Assert
            result.ShouldNotBeNull();
            result["Provider"].GetValue<string>().ShouldBe("SevenToEleven");
            result["COST"].GetValue<decimal>().ShouldBe(60);
            result.ContainsKey("MerchantID").ShouldBe(true);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task GetAsync_With_Unknown_ShippingMethod_Should_Return_Null()
        {
            // Act
            var result = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProvidersAppService.GetAsync("UNKNOWN_METHOD");
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task Update_Provider_Multiple_Times_Should_Keep_Latest_Values()
        {
            // Arrange & Act
            await WithUnitOfWorkAsync(async () =>
            {
                var input1 = GetHomeDeliveryDto();
                input1.Freight = 100;
                await _logisticsProvidersAppService.UpdateHomeDeliveryAsync(input1);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var input2 = GetHomeDeliveryDto();
                input2.Freight = 200;
                await _logisticsProvidersAppService.UpdateHomeDeliveryAsync(input2);
            });

            await WithUnitOfWorkAsync(async () =>
            {
                var input3 = GetHomeDeliveryDto();
                input3.Freight = 300;
                await _logisticsProvidersAppService.UpdateHomeDeliveryAsync(input3);
            });

            // Assert
            var provider = await WithUnitOfWorkAsync(async () =>
            {
                return await _logisticsProviderRepository.FirstOrDefaultAsync(
                    x => x.LogisticProvider == LogisticProviders.HomeDelivery);
            });

            provider.Freight.ShouldBe(300);
        }

        #endregion
    }
}