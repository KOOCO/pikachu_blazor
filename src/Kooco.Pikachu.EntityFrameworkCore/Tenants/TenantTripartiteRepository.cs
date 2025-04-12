using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Tenants.Entities;
using Kooco.Pikachu.Tenants.Repositories;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Tenants;
public class TenantTripartiteRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) :
    EfCoreRepository<PikachuDbContext, TenantTripartite, Guid>(dbContextProvider), ITenantTripartiteRepository
{

}