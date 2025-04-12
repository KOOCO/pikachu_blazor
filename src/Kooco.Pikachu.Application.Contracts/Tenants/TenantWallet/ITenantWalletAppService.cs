using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using Kooco.Pikachu.Tenants.TenantWallet.Request;

namespace Kooco.Pikachu.Tenants.TenantWallet;
public interface ITenantWalletAppService : IApplicationService
{
    Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
    Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
}