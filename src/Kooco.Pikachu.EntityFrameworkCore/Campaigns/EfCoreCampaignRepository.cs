using Kooco.Pikachu.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Campaigns;

public class EfCoreCampaignRepository : EfCoreRepository<PikachuDbContext, Campaign, Guid>, ICampaignRepository
{
    public EfCoreCampaignRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }
}
