using AutoMapper;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using Kooco.Pikachu.Tenants.TenantWallet;
using Kooco.Pikachu.Tenants.TenantWallet.Request;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.Tenants;

[Authorize]
public class TenantWalletAppService : PikachuAppService, ITenantWalletAppService
{
    [RemoteService(false)]
    public async Task AddRechargeTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import)
    {
        var transaction = ObjectMapper.Map<CreateWalletTransactionDto, TenantWalletTransaction>(import);
        await TenantWalletRepository.InsertAsync(transaction);

        var wallet = await TenantWalletRepository.GetAsync(walletId);
        wallet.WalletBalance += amount;

        await TenantWalletRepository.UpdateAsync(wallet);
    }

    [RemoteService(false)]
    public async Task AddDeductionTransactionAsync(Guid walletId, decimal amount, CreateWalletTransactionDto import)
    {
        var transaction = ObjectMapper.Map<CreateWalletTransactionDto, TenantWalletTransaction>(import);
        await TenantWalletRepository.InsertAsync(transaction);

        var wallet = await TenantWalletRepository.GetAsync(walletId);
        wallet.WalletBalance -= amount;

        await TenantWalletRepository.UpdateAsync(wallet);
    }

    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<CreateWalletTransactionDto, TenantWalletTransaction>();
        }
    }

    public required ITenantWalletRepository TenantWalletRepository { get; init; }
}