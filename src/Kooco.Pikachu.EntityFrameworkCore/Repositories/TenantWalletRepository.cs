using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.TenantWallets;
using Kooco.Pikachu.TenantWallets.Entities;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Repositories;
public class TenantWalletRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, TenantWallet, Guid>(dbContextProvider), ITenantWalletRepository
{

}