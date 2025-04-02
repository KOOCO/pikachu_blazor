using Kooco.Pikachu.TenantManagement.Request;
using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TenantManagement;
public interface ITenantWalletAppService : IApplicationService
{
    Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
    Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
}