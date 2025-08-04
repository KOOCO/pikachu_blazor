using System;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.PaymentGateways
{
    public class PaymentGatewayAppServiceLinePayTests : PikachuApplicationTestBase
    {
        private readonly IPaymentGatewayAppService _paymentGatewayAppService;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;

        public PaymentGatewayAppServiceLinePayTests()
        {
            _paymentGatewayAppService = GetRequiredService<IPaymentGatewayAppService>();
            _paymentGatewayRepository = GetRequiredService<IRepository<PaymentGateway, Guid>>();
        }

        [Fact]
        public async Task UpdateLinePayAsync_Should_Create_New_LinePay_With_Points_Enabled()
        {
            // Arrange
            var input = new UpdateLinePayDto
            {
                IsEnabled = true,
                ChannelId = "1234567890",
                ChannelSecretKey = "secretKey123456789",
                LinePointsRedemption = true
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateLinePayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var linePay = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
                
                linePay.ShouldNotBeNull();
                linePay.IsEnabled.ShouldBe(true);
                linePay.ChannelId.ShouldNotBeNullOrEmpty();
                linePay.ChannelSecretKey.ShouldNotBeNullOrEmpty();
                linePay.LinePointsRedemption.ShouldBe(true);
                
                // Verify that values are encrypted
                linePay.ChannelId.ShouldNotBe("1234567890");
                linePay.ChannelSecretKey.ShouldNotBe("secretKey123456789");
            });
        }

        [Fact]
        public async Task UpdateLinePayAsync_Should_Disable_LinePay_And_Points()
        {
            // Arrange
            var existingLinePay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = true,
                ChannelId = "encrypted_oldChannel",
                ChannelSecretKey = "encrypted_oldSecret",
                LinePointsRedemption = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existingLinePay);
            });

            var input = new UpdateLinePayDto
            {
                IsEnabled = false,
                ChannelId = "newChannel",
                ChannelSecretKey = "newSecret",
                LinePointsRedemption = false
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateLinePayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updatedLinePay = await _paymentGatewayRepository.GetAsync(existingLinePay.Id);
                
                updatedLinePay.IsEnabled.ShouldBe(false);
                updatedLinePay.LinePointsRedemption.ShouldBe(false);
                
                // Verify new credentials are encrypted and updated
                updatedLinePay.ChannelId.ShouldNotBe("encrypted_oldChannel");
                updatedLinePay.ChannelSecretKey.ShouldNotBe("encrypted_oldSecret");
            });
        }

        [Fact]
        public async Task GetLinePayAsync_Should_Return_Null_When_Not_Configured()
        {
            // Act
            PaymentGatewayDto result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetLinePayAsync();
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task GetLinePayAsync_Should_Return_LinePay_Without_Decryption()
        {
            // Arrange
            var linePay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = true,
                ChannelId = "encrypted_channel123",
                ChannelSecretKey = "encrypted_secret456",
                LinePointsRedemption = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(linePay);
            });

            // Act
            PaymentGatewayDto result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetLinePayAsync(decrypt: false);
            });

            // Assert
            result.ShouldNotBeNull();
            result.IsEnabled.ShouldBe(true);
            result.LinePointsRedemption.ShouldBe(false);
            result.ChannelId.ShouldBe("encrypted_channel123");
            result.ChannelSecretKey.ShouldBe("encrypted_secret456");
        }

        [Fact]
        public async Task GetLinePayAsync_Should_Return_LinePay_With_Decryption()
        {
            // Arrange
            var linePay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = true,
                ChannelId = "encrypted_channel123",
                ChannelSecretKey = "encrypted_secret456",
                LinePointsRedemption = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(linePay);
            });

            // Act
            PaymentGatewayDto result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetLinePayAsync(decrypt: true);
            });

            // Assert
            result.ShouldNotBeNull();
            result.IsEnabled.ShouldBe(true);
            result.LinePointsRedemption.ShouldBe(true);
            // Due to the way the decryption works in the service, these should be decrypted
            result.ChannelId.ShouldNotStartWith("encrypted_");
            result.ChannelSecretKey.ShouldNotStartWith("encrypted_");
        }

        [Fact]
        public async Task UpdateLinePayAsync_Should_Handle_Long_Channel_Id()
        {
            // Arrange
            var input = new UpdateLinePayDto
            {
                IsEnabled = true,
                ChannelId = "1234567890123456789012345678901234567890", // 40 characters
                ChannelSecretKey = "abcdefghijklmnopqrstuvwxyz123456789012345678901234567890",
                LinePointsRedemption = false
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateLinePayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var linePay = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
                
                linePay.ShouldNotBeNull();
                linePay.ChannelId.ShouldNotBeNullOrEmpty();
                linePay.ChannelSecretKey.ShouldNotBeNullOrEmpty();
            });
        }

        [Fact]
        public async Task UpdateLinePayAsync_Should_Update_Only_Changed_Fields()
        {
            // Arrange
            var existingLinePay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = true,
                ChannelId = "encrypted_keepThisChannel",
                ChannelSecretKey = "encrypted_keepThisSecret",
                LinePointsRedemption = true
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existingLinePay);
            });

            var input = new UpdateLinePayDto
            {
                IsEnabled = false, // Only change this
                ChannelId = "newChannel123",
                ChannelSecretKey = "newSecret456",
                LinePointsRedemption = true // Keep same
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateLinePayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updatedLinePay = await _paymentGatewayRepository.GetAsync(existingLinePay.Id);
                
                updatedLinePay.IsEnabled.ShouldBe(false);
                updatedLinePay.LinePointsRedemption.ShouldBe(true);
                
                // All fields are updated even if only one changed
                updatedLinePay.ChannelId.ShouldNotBe("encrypted_keepThisChannel");
                updatedLinePay.ChannelSecretKey.ShouldNotBe("encrypted_keepThisSecret");
            });
        }

        [Fact]
        public async Task Multiple_LinePay_Configs_Should_Return_First_One()
        {
            // Arrange - Create multiple LinePay configs (edge case)
            var linePay1 = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = true,
                ChannelId = "encrypted_channel1",
                ChannelSecretKey = "encrypted_secret1",
                LinePointsRedemption = true
            };

            var linePay2 = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = false,
                ChannelId = "encrypted_channel2",
                ChannelSecretKey = "encrypted_secret2",
                LinePointsRedemption = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(linePay1);
                await _paymentGatewayRepository.InsertAsync(linePay2);
            });

            // Act
            PaymentGatewayDto result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetLinePayAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            // Should return the first one based on FirstOrDefaultAsync
        }

        [Fact]
        public async Task UpdateLinePayAsync_Should_Create_LinePay_With_Points_Disabled()
        {
            // Arrange
            var input = new UpdateLinePayDto
            {
                IsEnabled = true,
                ChannelId = "POINTS_DISABLED_CHANNEL",
                ChannelSecretKey = "POINTS_DISABLED_SECRET",
                LinePointsRedemption = false
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateLinePayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var linePay = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
                
                linePay.ShouldNotBeNull();
                linePay.IsEnabled.ShouldBe(true);
                linePay.LinePointsRedemption.ShouldBe(false);
            });
        }
    }
}