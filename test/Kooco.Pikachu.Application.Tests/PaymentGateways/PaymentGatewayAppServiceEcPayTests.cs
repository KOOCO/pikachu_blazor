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
    public class PaymentGatewayAppServiceEcPayTests : PikachuApplicationTestBase
    {
        private readonly IPaymentGatewayAppService _paymentGatewayAppService;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;

        public PaymentGatewayAppServiceEcPayTests()
        {
            _paymentGatewayAppService = GetRequiredService<IPaymentGatewayAppService>();
            _paymentGatewayRepository = GetRequiredService<IRepository<PaymentGateway, Guid>>();
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Create_New_EcPay_With_All_Fields()
        {
            // Arrange
            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = true,
                MerchantId = "2000132",
                HashKey = "5294y06JbISpM5x9",
                HashIV = "v77hoKGq4kWxNNIS",
                TradeDescription = "Pikachu電商平台",
                CreditCheckCode = "CHECK123",
                InstallmentPeriods = new List<string> { "3", "6", "12", "24" }
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
                ecPay.IsBankTransferEnabled.ShouldBe(true);
                ecPay.MerchantId.ShouldNotBeNullOrEmpty();
                ecPay.HashKey.ShouldNotBeNullOrEmpty();
                ecPay.HashIV.ShouldNotBeNullOrEmpty();
                ecPay.TradeDescription.ShouldNotBeNullOrEmpty();
                ecPay.CreditCheckCode.ShouldNotBeNullOrEmpty();
                
                // Verify InstallmentPeriods are properly serialized
                ecPay.InstallmentPeriodsJson.ShouldBe("[\"3\",\"6\",\"12\",\"24\"]");
                ecPay.InstallmentPeriods.Count.ShouldBe(4);
                ecPay.InstallmentPeriods.ShouldContain("3");
                ecPay.InstallmentPeriods.ShouldContain("24");
            });
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Update_Existing_With_Empty_InstallmentPeriods()
        {
            // Arrange
            var existingEcPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = false,
                MerchantId = "encrypted_old",
                HashKey = "encrypted_old",
                HashIV = "encrypted_old",
                TradeDescription = "encrypted_old",
                CreditCheckCode = "encrypted_old",
                InstallmentPeriodsJson = "[\"3\",\"6\"]"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existingEcPay);
            });

            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = false,
                IsBankTransferEnabled = true,
                MerchantId = "newMerchant",
                HashKey = "newKey",
                HashIV = "newIV",
                TradeDescription = "新描述",
                CreditCheckCode = "newCode",
                InstallmentPeriods = new List<string>() // Empty list
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
                
                updatedEcPay.IsCreditCardEnabled.ShouldBe(false);
                updatedEcPay.IsBankTransferEnabled.ShouldBe(true);
                updatedEcPay.InstallmentPeriodsJson.ShouldBe("[]");
                updatedEcPay.InstallmentPeriods.Count.ShouldBe(0);
            });
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Handle_Null_InstallmentPeriods()
        {
            // Arrange
            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = false,
                MerchantId = "MERCHANT001",
                HashKey = "KEY001",
                HashIV = "IV001",
                TradeDescription = "Test",
                CreditCheckCode = "CHECK001",
                InstallmentPeriods = null // Null list
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
                ecPay.InstallmentPeriodsJson.ShouldBeOneOf("null", "[]");
                ecPay.InstallmentPeriods.Count.ShouldBe(0);
            });
        }

        [Fact]
        public async Task GetCreditCheckCodeAsync_Should_Return_Decrypted_Code_From_EcPay()
        {
            // Arrange
            var ecPay1 = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                CreditCheckCode = "encrypted_checkCode123"
            };
            
            var ecPay2 = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                CreditCheckCode = "encrypted_checkCode456"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(ecPay1);
                await _paymentGatewayRepository.InsertAsync(ecPay2);
            });

            // Act
            string creditCheckCode = null;
            await WithUnitOfWorkAsync(async () =>
            {
                creditCheckCode = await _paymentGatewayAppService.GetCreditCheckCodeAsync();
            });

            // Assert
            creditCheckCode.ShouldNotBeNull();
            // Should return one of the credit check codes (implementation uses FirstOrDefault)
            creditCheckCode.ShouldBeOneOf("checkCode123", "checkCode456");
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Enable_Both_Payment_Methods()
        {
            // Arrange
            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = true,
                MerchantId = "DUAL001",
                HashKey = "DUALKEY",
                HashIV = "DUALIV",
                TradeDescription = "雙重支付",
                CreditCheckCode = "DUALCHECK",
                InstallmentPeriods = new List<string> { "3", "6" }
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
                ecPay.IsBankTransferEnabled.ShouldBe(true);
            });
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Disable_Both_Payment_Methods()
        {
            // Arrange
            var existingEcPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = true,
                MerchantId = "encrypted",
                HashKey = "encrypted",
                HashIV = "encrypted",
                TradeDescription = "encrypted",
                CreditCheckCode = "encrypted"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existingEcPay);
            });

            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = false,
                IsBankTransferEnabled = false,
                MerchantId = "DISABLED",
                HashKey = "DISABLED",
                HashIV = "DISABLED",
                TradeDescription = "已停用",
                CreditCheckCode = "DISABLED",
                InstallmentPeriods = new List<string>()
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
                
                updatedEcPay.IsCreditCardEnabled.ShouldBe(false);
                updatedEcPay.IsBankTransferEnabled.ShouldBe(false);
            });
        }

        [Fact]
        public async Task UpdateEcPayAsync_Should_Handle_Special_Characters_In_TradeDescription()
        {
            // Arrange
            var input = new UpdateEcPayDto
            {
                IsCreditCardEnabled = true,
                IsBankTransferEnabled = false,
                MerchantId = "SPECIAL001",
                HashKey = "KEY",
                HashIV = "IV",
                TradeDescription = "測試商店 & 特殊字元 <>&\"'",
                CreditCheckCode = "CHECK",
                InstallmentPeriods = new List<string> { "3" }
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
                ecPay.TradeDescription.ShouldNotBeNullOrEmpty();
            });
        }

        [Fact]
        public async Task GetCreditCheckCodeAsync_Should_Handle_Null_CreditCheckCode()
        {
            // Arrange
            var ecPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                CreditCheckCode = null
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
            creditCheckCode.ShouldBeNull();
        }

        [Fact]
        public async Task GetCreditCheckCodeAsync_Should_Handle_Empty_CreditCheckCode()
        {
            // Arrange
            var ecPay = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.EcPay,
                CreditCheckCode = ""
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
            creditCheckCode.ShouldBeEmpty();
        }
    }
}