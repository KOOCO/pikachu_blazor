using Kooco.Pikachu.Tenants.Requests;
using System.Threading.Tasks;
using System;

namespace Kooco.Pikachu.Tenants;
public interface ITenantWalletAppService
{
    Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
    Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import);
}
