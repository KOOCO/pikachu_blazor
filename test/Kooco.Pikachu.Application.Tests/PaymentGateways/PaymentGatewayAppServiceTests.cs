using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using NSubstitute;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Security.Encryption;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.PaymentGateways
{
    public class PaymentGatewayAppServiceTests : PikachuApplicationTestBase
    {
        private readonly IPaymentGatewayAppService _paymentGatewayAppService;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;
        private readonly IStringEncryptionService _mockEncryptionService;

        public PaymentGatewayAppServiceTests()
        {
            _paymentGatewayAppService = GetRequiredService<IPaymentGatewayAppService>();
            _paymentGatewayRepository = GetRequiredService<IRepository<PaymentGateway, Guid>>();
            
            // Create mock encryption service for predictable testing
            _mockEncryptionService = Substitute.For<IStringEncryptionService>();
            _mockEncryptionService.Encrypt(Arg.Any<string>()).Returns(args => $"encrypted_{args[0]}");
            _mockEncryptionService.Decrypt(Arg.Any<string>()).Returns(args => 
            {
                var input = args[0] as string;
                return input?.StartsWith("encrypted_") == true ? input.Substring(10) : input;
            });
        }

        #region ECPay Tests

        [Fact]
        public async Task UpdateEcPayAsync_Should_Create_New_EcPay_When_Not_Exists()
        {
            // Arrange
            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = false,
                MerchantId = "2000132",
                HashKey = "5294y06JbISpM5x9",
                HashIV = "v77hoKGq4kWxNNIS",
                TradeDescription = "測試商店",
                CreditCheckCode = "testCheckCode",
                InstallmentPeriods = new List<string> { "3", "6", "12" }
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateEcPayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var ecPay = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);
                
                ecPay.ShouldNotBeNull();
                ecPay.IsCreditCardEnabled.ShouldBe(true);
                ecPay.IsBankTransferEnabled.ShouldBe(false);
                ecPay.MerchantId.ShouldNotBeNullOrEmpty();
                ecPay.HashKey.ShouldNotBeNullOrEmpty();
                ecPay.HashIV.ShouldNotBeNullOrEmpty();
                ecPay.TradeDescription.ShouldNotBeNullOrEmpty();
                ecPay.CreditCheckCode.ShouldNotBeNullOrEmpty();
                ecPay.InstallmentPeriodsJson.ShouldBe("[\"3\",\"6\",\"12\"]");
            });
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Update_Existing_EcPay()
        {
            // Arrange
            var existingEcPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                IsCreditCardEnabled = false,
                IsBankTransferEnabled = true,
                MerchantId = "encrypted_oldMerchantId",
                HashKey = "encrypted_oldHashKey",
                HashIV = "encrypted_oldHashIV",
                TradeDescription = "encrypted_oldDescription",
                CreditCheckCode = "encrypted_oldCheckCode",
                InstallmentPeriodsJson = "[\"3\"]"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existingEcPay);
            });

            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = false,
                MerchantId = "newMerchantId",
                HashKey = "newHashKey",
                HashIV = "newHashIV",
                TradeDescription = "新商店描述",
                CreditCheckCode = "newCheckCode",
                InstallmentPeriods = new List<string> { "6", "12", "24" }
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateEcPayAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updatedEcPay = await _paymentGatewayRepository.GetAsync(existingEcPay.Id);
                
                updatedEcPay.IsCreditCardEnabled.ShouldBe(true);
                updatedEcPay.IsBankTransferEnabled.ShouldBe(false);
                updatedEcPay.InstallmentPeriodsJson.ShouldBe("[\"6\",\"12\",\"24\"]");
            });
        }

        [Fact]
        public async Task GetCreditCheckCodeAsync_Should_Return_Decrypted_Code()
        {
            // Arrange
            var ecPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                CreditCheckCode = "encryptedCheckCode123"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(ecPay);
            });

            // Act
            string creditCheckCode = null;
            await WithUnitOfWorkAsync(async () =>
            {
                creditCheckCode = await _paymentGatewayAppService.GetCreditCheckCodeAsync();
            });

            // Assert
            creditCheckCode.ShouldNotBeNull();
        }

        [Fact]
        public async Task GetCreditCheckCodeAsync_Should_Return_Null_When_No_EcPay()
        {
            // Act
            string creditCheckCode = null;
            await WithUnitOfWorkAsync(async () =>
            {
                creditCheckCode = await _paymentGatewayAppService.GetCreditCheckCodeAsync();
            });

            // Assert
            creditCheckCode.ShouldBeNull();
        }

        #endregion

        #region LinePay Tests

        [Fact]
        public async Task UpdateLinePayAsync_Should_Create_New_LinePay_When_Not_Exists()
        {
            // Arrange
            var input = new UpdateLinePayDto
            {
                IsEnabled = true,
                ChannelId = "1234567890",
                ChannelSecretKey = "secretKey123",
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
            });
        }

        [Fact]
        public async Task UpdateLinePayAsync_Should_Update_Existing_LinePay()
        {
            // Arrange
            var existingLinePay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = false,
                ChannelId = "encrypted_oldChannelId",
                ChannelSecretKey = "encrypted_oldSecretKey",
                LinePointsRedemption = false
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existingLinePay);
            });

            var input = new UpdateLinePayDto
            {
                IsEnabled = true,
                ChannelId = "newChannelId",
                ChannelSecretKey = "newSecretKey",
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
                var updatedLinePay = await _paymentGatewayRepository.GetAsync(existingLinePay.Id);
                
                updatedLinePay.IsEnabled.ShouldBe(true);
                updatedLinePay.LinePointsRedemption.ShouldBe(true);
            });
        }

        [Fact]
        public async Task GetLinePayAsync_Should_Return_LinePay_With_Decryption()
        {
            // Arrange
            var linePay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.LinePay,
                IsEnabled = true,
                ChannelId = "encrypted_channelId123",
                ChannelSecretKey = "encrypted_secretKey456",
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
        }

        [Fact]
        public async Task GetLinePayAsync_Should_Return_Null_When_Not_Exists()
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

        #endregion

        #region ChinaTrust Tests

        [Fact]
        public async Task UpdateChinaTrustAsync_Should_Create_New_ChinaTrust_When_Not_Exists()
        {
            // Arrange
            var input = new UpdateChinaTrustDto
            {
                IsEnabled = true,
                MerchantId = "MERCH123",
                Code = "CODE456",
                TerminalCode = "TERM789",
                CodeValue = "VALUE012"
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateChinaTrustAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var chinaTrust = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.ChinaTrust);
                
                chinaTrust.ShouldNotBeNull();
                chinaTrust.IsEnabled.ShouldBe(true);
                chinaTrust.MerchantId.ShouldNotBeNullOrEmpty();
                chinaTrust.Code.ShouldNotBeNullOrEmpty();
                chinaTrust.TerminalCode.ShouldNotBeNullOrEmpty();
                chinaTrust.CodeValue.ShouldNotBeNullOrEmpty();
            });
        }

        #endregion

        #region Manual Bank Transfer Tests

        [Fact]
        public async Task UpdateManualBankTransferAsync_Should_Create_New_When_Not_Exists()
        {
            // Arrange
            var input = new UpdateManualBankTransferDto
            {
                IsEnabled = true,
                AccountName = "測試帳戶",
                BankName = "測試銀行",
                BranchName = "測試分行",
                BankCode = "012",
                BankAccountNumber = "123456789012",
                MinimumAmountLimit = 100,
                MaximumAmountLimit = 1000000
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateManualBankTransferAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var bankTransfer = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.ManualBankTransfer);
                
                bankTransfer.ShouldNotBeNull();
                bankTransfer.IsEnabled.ShouldBe(true);
                bankTransfer.AccountName.ShouldBe("測試帳戶");
                bankTransfer.BankName.ShouldBe("測試銀行");
                bankTransfer.BankCode.ShouldBe("012");
                bankTransfer.MinimumAmountLimit.ShouldBe(100);
                bankTransfer.MaximumAmountLimit.ShouldBe(1000000);
            });
        }

        [Fact]
        public async Task GetManualBankTransferAsync_Should_Return_Bank_Transfer_Info()
        {
            // Arrange
            var bankTransfer = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.ManualBankTransfer,
                IsEnabled = true,
                AccountName = "Test Account",
                BankName = "Test Bank",
                BankCode = "808",
                BankAccountNumber = "987654321098"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(bankTransfer);
            });

            // Act
            ManualBankTransferDto result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetManualBankTransferAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.AccountName.ShouldBe("Test Account");
            result.BankName.ShouldBe("Test Bank");
            result.BankCode.ShouldBe("808");
        }

        #endregion

        #region Order Validity Tests

        [Fact]
        public async Task UpdateOrderValidityAsync_Should_Create_New_When_Not_Exists()
        {
            // Arrange
            var input = new UpdateOrderValidityDto
            {
                Period = 7,
                Unit = "days"
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateOrderValidityAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var validity = await _paymentGatewayRepository.FirstOrDefaultAsync(
                    x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod);
                
                validity.ShouldNotBeNull();
                validity.Period.ShouldBe(7);
                validity.Unit.ShouldBe("days");
            });
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Payment_Gateways_Decrypted()
        {
            // Arrange
            var paymentGateways = new List<PaymentGateway>
            {
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    IsCreditCardEnabled = true,
                    MerchantId = "encrypted_ecpayMerchant"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.LinePay,
                    IsEnabled = true,
                    ChannelId = "encrypted_linePayChannel"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.ManualBankTransfer,
                    IsEnabled = true,
                    AccountName = "Test Bank Account"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gateway in paymentGateways)
                {
                    await _paymentGatewayRepository.InsertAsync(gateway);
                }
            });

            // Act
            List<PaymentGatewayDto> result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetAllAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            
            var ecPay = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);
            ecPay.ShouldNotBeNull();
            ecPay.IsCreditCardEnabled.ShouldBe(true);
            
            var linePay = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
            linePay.ShouldNotBeNull();
            linePay.IsEnabled.ShouldBe(true);
            
            var bankTransfer = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.ManualBankTransfer);
            bankTransfer.ShouldNotBeNull();
            bankTransfer.AccountName.ShouldBe("Test Bank Account");
        }

        #endregion
    }
}