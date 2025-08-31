using Kooco.Pikachu.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.TenantPayouts;

public class EfCoreTenantPayoutRepository : EfCoreRepository<PikachuDbContext, TenantPayoutRecord, Guid>, ITenantPayoutRepository
{
    public EfCoreTenantPayoutRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
