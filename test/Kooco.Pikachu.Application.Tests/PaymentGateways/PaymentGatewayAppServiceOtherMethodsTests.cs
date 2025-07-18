using System;
using System.Threading.Tasks;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.PaymentGateways;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Kooco.Pikachu.Application.Tests.PaymentGateways
{
    public class PaymentGatewayAppServiceOtherMethodsTests : PikachuApplicationTestBase
    {
        private readonly IPaymentGatewayAppService _paymentGatewayAppService;
        private readonly IRepository<PaymentGateway, Guid> _paymentGatewayRepository;

        public PaymentGatewayAppServiceOtherMethodsTests()
        {
            _paymentGatewayAppService = GetRequiredService<IPaymentGatewayAppService>();
            _paymentGatewayRepository = GetRequiredService<IRepository<PaymentGateway, Guid>>();
        }

        #region ChinaTrust Tests

        [Fact]
        public async Task UpdateChinaTrustAsync_Should_Create_New_ChinaTrust()
        {
            // Arrange
            var input = new UpdateChinaTrustDto
            {
                IsEnabled = true,
                MerchantId = "CHINA001",
                Code = "CODE123",
                TerminalCode = "TERM456",
                CodeValue = "VALUE789"
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
                
                // Verify encryption
                chinaTrust.MerchantId.ShouldNotBe("CHINA001");
                chinaTrust.Code.ShouldNotBe("CODE123");
            });
        }

        [Fact]
        public async Task UpdateChinaTrustAsync_Should_Update_Existing_ChinaTrust()
        {
            // Arrange
            var existing = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.ChinaTrust,
                IsEnabled = false,
                MerchantId = "encrypted_old",
                Code = "encrypted_old",
                TerminalCode = "encrypted_old",
                CodeValue = "encrypted_old"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existing);
            });

            var input = new UpdateChinaTrustDto
            {
                IsEnabled = true,
                MerchantId = "NEW_MERCHANT",
                Code = "NEW_CODE",
                TerminalCode = "NEW_TERMINAL",
                CodeValue = "NEW_VALUE"
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateChinaTrustAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _paymentGatewayRepository.GetAsync(existing.Id);
                
                updated.IsEnabled.ShouldBe(true);
                updated.MerchantId.ShouldNotBe("encrypted_old");
                updated.Code.ShouldNotBe("encrypted_old");
                updated.TerminalCode.ShouldNotBe("encrypted_old");
                updated.CodeValue.ShouldNotBe("encrypted_old");
            });
        }

        [Fact]
        public async Task UpdateChinaTrustAsync_Should_Disable_ChinaTrust()
        {
            // Arrange
            var input = new UpdateChinaTrustDto
            {
                IsEnabled = false,
                MerchantId = "DISABLED",
                Code = "DISABLED",
                TerminalCode = "DISABLED",
                CodeValue = "DISABLED"
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
                chinaTrust.IsEnabled.ShouldBe(false);
            });
        }

        #endregion

        #region Manual Bank Transfer Tests

        [Fact]
        public async Task UpdateManualBankTransferAsync_Should_Create_New_Bank_Transfer()
        {
            // Arrange
            var input = new UpdateManualBankTransferDto
            {
                IsEnabled = true,
                AccountName = "皮卡丘電商有限公司",
                BankName = "台灣銀行",
                BranchName = "台北分行",
                BankCode = "004",
                BankAccountNumber = "123456789012345",
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
                bankTransfer.AccountName.ShouldBe("皮卡丘電商有限公司");
                bankTransfer.BankName.ShouldBe("台灣銀行");
                bankTransfer.BranchName.ShouldBe("台北分行");
                bankTransfer.BankCode.ShouldBe("004");
                bankTransfer.BankAccountNumber.ShouldBe("123456789012345");
                bankTransfer.MinimumAmountLimit.ShouldBe(100);
                bankTransfer.MaximumAmountLimit.ShouldBe(1000000);
            });
        }

        [Fact]
        public async Task UpdateManualBankTransferAsync_Should_Update_Existing()
        {
            // Arrange
            var existing = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.ManualBankTransfer,
                IsEnabled = true,
                AccountName = "舊公司名稱",
                BankName = "舊銀行",
                BranchName = "舊分行",
                BankCode = "012",
                BankAccountNumber = "987654321098765",
                MinimumAmountLimit = 500,
                MaximumAmountLimit = 500000
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existing);
            });

            var input = new UpdateManualBankTransferDto
            {
                IsEnabled = false,
                AccountName = "新公司名稱",
                BankName = "新銀行",
                BranchName = "新分行",
                BankCode = "808",
                BankAccountNumber = "111222333444555",
                MinimumAmountLimit = 1000,
                MaximumAmountLimit = 2000000
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateManualBankTransferAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _paymentGatewayRepository.GetAsync(existing.Id);
                
                updated.IsEnabled.ShouldBe(false);
                updated.AccountName.ShouldBe("新公司名稱");
                updated.BankName.ShouldBe("新銀行");
                updated.BranchName.ShouldBe("新分行");
                updated.BankCode.ShouldBe("808");
                updated.BankAccountNumber.ShouldBe("111222333444555");
                updated.MinimumAmountLimit.ShouldBe(1000);
                updated.MaximumAmountLimit.ShouldBe(2000000);
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
                AccountName = "測試公司",
                BankName = "測試銀行",
                BranchName = "測試分行",
                BankCode = "999",
                BankAccountNumber = "999888777666555",
                MinimumAmountLimit = 50,
                MaximumAmountLimit = 100000
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
            result.PaymentIntegrationType.ShouldBe(PaymentIntegrationType.ManualBankTransfer);
            result.IsEnabled.ShouldBe(true);
            result.AccountName.ShouldBe("測試公司");
            result.BankName.ShouldBe("測試銀行");
            result.BranchName.ShouldBe("測試分行");
            result.BankCode.ShouldBe("999");
            result.BankAccountNumber.ShouldBe("999888777666555");
            result.MinimumAmountLimit.ShouldBe(50);
            result.MaximumAmountLimit.ShouldBe(100000);
        }

        [Fact]
        public async Task GetManualBankTransferAsync_Should_Return_Null_When_Not_Exists()
        {
            // Act
            ManualBankTransferDto result = null;
            await WithUnitOfWorkAsync(async () =>
            {
                result = await _paymentGatewayAppService.GetManualBankTransferAsync();
            });

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateManualBankTransferAsync_Should_Handle_Zero_Limits()
        {
            // Arrange
            var input = new UpdateManualBankTransferDto
            {
                IsEnabled = true,
                AccountName = "零限制測試",
                BankName = "測試銀行",
                BranchName = "測試分行",
                BankCode = "001",
                BankAccountNumber = "000111222333444",
                MinimumAmountLimit = 0,
                MaximumAmountLimit = 0
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
                bankTransfer.MinimumAmountLimit.ShouldBe(0);
                bankTransfer.MaximumAmountLimit.ShouldBe(0);
            });
        }

        #endregion

        #region Order Validity Tests

        [Fact]
        public async Task UpdateOrderValidityAsync_Should_Create_New_Validity_Period()
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

        [Fact]
        public async Task UpdateOrderValidityAsync_Should_Update_Existing_Validity()
        {
            // Arrange
            var existing = new PaymentGateway
            {
                PaymentIntegrationType = PaymentIntegrationType.OrderValidatePeriod,
                Period = 30,
                Unit = "days"
            };

            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayRepository.InsertAsync(existing);
            });

            var input = new UpdateOrderValidityDto
            {
                Period = 24,
                Unit = "hours"
            };

            // Act
            await WithUnitOfWorkAsync(async () =>
            {
                await _paymentGatewayAppService.UpdateOrderValidityAsync(input);
            });

            // Assert
            await WithUnitOfWorkAsync(async () =>
            {
                var updated = await _paymentGatewayRepository.GetAsync(existing.Id);
                
                updated.Period.ShouldBe(24);
                updated.Unit.ShouldBe("hours");
            });
        }

        [Fact]
        public async Task UpdateOrderValidityAsync_Should_Handle_Different_Units()
        {
            // Arrange
            var testCases = new[]
            {
                new { Period = 5, Unit = "minutes" },
                new { Period = 12, Unit = "hours" },
                new { Period = 14, Unit = "days" },
                new { Period = 2, Unit = "weeks" },
                new { Period = 1, Unit = "months" }
            };

            foreach (var testCase in testCases)
            {
                var input = new UpdateOrderValidityDto
                {
                    Period = testCase.Period,
                    Unit = testCase.Unit
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
                    validity.Period.ShouldBe(testCase.Period);
                    validity.Unit.ShouldBe(testCase.Unit);
                });

                // Clean up for next iteration
                await WithUnitOfWorkAsync(async () =>
                {
                    var toDelete = await _paymentGatewayRepository.FirstOrDefaultAsync(
                        x => x.PaymentIntegrationType == PaymentIntegrationType.OrderValidatePeriod);
                    if (toDelete != null)
                    {
                        await _paymentGatewayRepository.DeleteAsync(toDelete);
                    }
                });
            }
        }

        [Fact]
        public async Task UpdateOrderValidityAsync_Should_Handle_Zero_Period()
        {
            // Arrange
            var input = new UpdateOrderValidityDto
            {
                Period = 0,
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
                validity.Period.ShouldBe(0);
                validity.Unit.ShouldBe("days");
            });
        }

        #endregion
    }
}