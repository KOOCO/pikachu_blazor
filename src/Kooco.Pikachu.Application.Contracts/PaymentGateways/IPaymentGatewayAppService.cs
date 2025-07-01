using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.PaymentGateways
{
    public interface IPaymentGatewayAppService : IApplicationService
    {
        Task UpdateLinePayAsync(UpdateLinePayDto input);
        Task UpdateChinaTrustAsync(UpdateChinaTrustDto input);
        Task UpdateEcPayAsync(UpdateEcPayDto input);
        Task<List<PaymentGatewayDto>> GetAllAsync();
        Task UpdateOrderValidityAsync(UpdateOrderValidityDto input);
        Task<PaymentGatewayDto?> GetLinePayAsync(bool decrypt = false);
        Task<string?> GetCreditCheckCodeAsync();
        Task UpdateManualBankTransferAsync(UpdateManualBankTransferDto input);
        Task<ManualBankTransferDto?> GetManualBankTransferAsync();
    }
}
