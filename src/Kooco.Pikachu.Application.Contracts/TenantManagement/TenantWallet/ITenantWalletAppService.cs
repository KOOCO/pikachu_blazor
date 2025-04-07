using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using Kooco.Pikachu.TenantManagement.TenantWallet.Request;

namespace Kooco.Pikachu.TenantManagement.TenantWallet;
public interface ITenantWalletAppService : IApplicationService
{
    Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
    Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
}