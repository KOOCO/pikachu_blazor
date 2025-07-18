using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.PaymentGateways
{
    public class PaymentGatewayAppServiceQueryTests : PikachuApplicationTestBase
    {
        private readonly IPaymentGatewayAppService _paymentGatewayAppService;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;

        public PaymentGatewayAppServiceQueryTests()
        {
            _paymentGatewayAppService = GetRequiredService<IPaymentGatewayAppService>();
            _paymentGatewayRepository = GetRequiredService<IRepository<PaymentGateway, Guid>>();
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Empty_List_When_No_Gateways()
        {
            // Act
            List<PaymentGatewayDto> result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetAllAsync();
            });

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Gateway_Types()
        {
            // Arrange
            var gateways = new List<PaymentGateway>
            {
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    IsCreditCardEnabled = true,
                    IsBankTransferEnabled = false,
                    MerchantId = "encrypted_ecpay123",
                    HashKey = "encrypted_key",
                    HashIV = "encrypted_iv",
                    TradeDescription = "encrypted_desc",
                    CreditCheckCode = "encrypted_code",
                    InstallmentPeriodsJson = "[\"3\",\"6\"]"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.LinePay,
                    IsEnabled = true,
                    ChannelId = "encrypted_channel",
                    ChannelSecretKey = "encrypted_secret",
                    LinePointsRedemption = true
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.ChinaTrust,
                    IsEnabled = false,
                    MerchantId = "encrypted_china",
                    Code = "encrypted_code",
                    TerminalCode = "encrypted_terminal",
                    CodeValue = "encrypted_value"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.ManualBankTransfer,
                    IsEnabled = true,
                    AccountName = "Test Company Ltd.",
                    BankName = "Test Bank",
                    BranchName = "Main Branch",
                    BankCode = "012",
                    BankAccountNumber = "123456789012",
                    MinimumAmountLimit = 100,
                    MaximumAmountLimit = 1000000
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.OrderValidatePeriod,
                    Period = 7,
                    Unit = "days"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gateway in gateways)
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
            result.Count.ShouldBe(5);
            
            // Verify all types are present
            result.Any(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).ShouldBeTrue();
            result.Any(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay).ShouldBeTrue();
            result.Any(x => x.PaymentIntegrationType == PaymentIntegrationType.ChinaTrust).ShouldBeTrue();
            result.Any(x => x.PaymentIntegrationType == PaymentIntegrationType.ManualBankTransfer).ShouldBeTrue();
            result.Any(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod).ShouldBeTrue();
        }

        [Fact]
        public async Task GetAllAsync_Should_Decrypt_All_Except_ManualBankTransfer()
        {
            // Arrange
            var gateways = new List<PaymentGateway>
            {
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    MerchantId = "encrypted_ecpay_merchant",
                    HashKey = "encrypted_ecpay_key"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.LinePay,
                    ChannelId = "encrypted_line_channel",
                    ChannelSecretKey = "encrypted_line_secret"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.ManualBankTransfer,
                    AccountName = "Plain Account Name",
                    BankCode = "808"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gateway in gateways)
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
            
            var ecPay = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);
            ecPay.ShouldNotBeNull();
            ecPay.MerchantId.ShouldNotStartWith("encrypted_");
            ecPay.HashKey.ShouldNotStartWith("encrypted_");
            
            var linePay = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
            linePay.ShouldNotBeNull();
            linePay.ChannelId.ShouldNotStartWith("encrypted_");
            linePay.ChannelSecretKey.ShouldNotStartWith("encrypted_");
            
            var bankTransfer = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.ManualBankTransfer);
            bankTransfer.ShouldNotBeNull();
            bankTransfer.AccountName.ShouldBe("Plain Account Name"); // Not encrypted
            bankTransfer.BankCode.ShouldBe("808"); // Not encrypted
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Correct_EcPay_InstallmentPeriods()
        {
            // Arrange
            var ecPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                InstallmentPeriodsJson = "[\"3\",\"6\",\"12\",\"24\"]"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(ecPay);
            });

            // Act
            List<PaymentGatewayDto> result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetAllAsync();
            });

            // Assert
            var ecPayDto = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);
            ecPayDto.ShouldNotBeNull();
            ecPayDto.InstallmentPeriods.ShouldNotBeNull();
            ecPayDto.InstallmentPeriods.Count.ShouldBe(4);
            ecPayDto.InstallmentPeriods.ShouldContain("3");
            ecPayDto.InstallmentPeriods.ShouldContain("6");
            ecPayDto.InstallmentPeriods.ShouldContain("12");
            ecPayDto.InstallmentPeriods.ShouldContain("24");
        }

        [Fact]
        public async Task GetAllAsync_Should_Handle_Multiple_Same_Type_Gateways()
        {
            // Arrange - Edge case: multiple gateways of same type
            var gateways = new List<PaymentGateway>
            {
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    MerchantId = "encrypted_merchant1"
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    MerchantId = "encrypted_merchant2"
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gateway in gateways)
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
            result.Count.ShouldBe(2);
            result.All(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay).ShouldBeTrue();
        }

        [Fact]
        public async Task GetAllAsync_Should_Handle_Null_And_Empty_Fields()
        {
            // Arrange
            var gateways = new List<PaymentGateway>
            {
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    MerchantId = null,
                    HashKey = "",
                    HashIV = "encrypted_value",
                    TradeDescription = null,
                    CreditCheckCode = ""
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.LinePay,
                    ChannelId = null,
                    ChannelSecretKey = ""
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gateway in gateways)
                {
                    await _paymentGatewayRepository.InsertAsync(gateway);
                }
            });

            // Act & Assert - Should not throw exception
            List<PaymentGatewayDto> result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetAllAsync();
            });

            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetAllAsync_Should_Preserve_Unit_Field_Without_Decryption()
        {
            // Arrange
            var validity = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.OrderValidatePeriod,
                Period = 30,
                Unit = "days" // Should not be decrypted
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(validity);
            });

            // Act
            List<PaymentGatewayDto> result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetAllAsync();
            });

            // Assert
            var validityDto = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod);
            validityDto.ShouldNotBeNull();
            validityDto.Unit.ShouldBe("days"); // Should remain unchanged
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Correct_Flags_And_Settings()
        {
            // Arrange
            var gateways = new List<PaymentGateway>
            {
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.EcPay,
                    IsCreditCardEnabled = true,
                    IsBankTransferEnabled = false
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.LinePay,
                    IsEnabled = false,
                    LinePointsRedemption = true
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.ChinaTrust,
                    IsEnabled = true
                },
                new PaymentGateway
                {
                    PaymentIntegrationType = PaymentIntegrationType.ManualBankTransfer,
                    IsEnabled = false,
                    MinimumAmountLimit = 500,
                    MaximumAmountLimit = 50000
                }
            };

            await WithUnitOfWorkAsync(async () =>
            {
                foreach (var gateway in gateways)
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
            var ecPay = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.EcPay);
            ecPay.IsCreditCardEnabled.ShouldBe(true);
            ecPay.IsBankTransferEnabled.ShouldBe(false);
            
            var linePay = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.LinePay);
            linePay.IsEnabled.ShouldBe(false);
            linePay.LinePointsRedemption.ShouldBe(true);
            
            var chinaTrust = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.ChinaTrust);
            chinaTrust.IsEnabled.ShouldBe(true);
            
            var bankTransfer = result.FirstOrDefault(x => x.PaymentIntegrationType == PaymentIntegrationType.ManualBankTransfer);
            bankTransfer.IsEnabled.ShouldBe(false);
            bankTransfer.MinimumAmountLimit.ShouldBe(500);
            bankTransfer.MaximumAmountLimit.ShouldBe(50000);
        }
    }
}