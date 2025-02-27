﻿using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Freebies
{
    public class EfCoreFreebieRepository : EfCoreRepository<PikachuDbContext, Freebie, Guid>, IFreebieRepository
    {
        public EfCoreFreebieRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
        public async Task<Freebie> FindByNameAsync(string itemName)
        {
            return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.ItemName == itemName);
        }
        public async Task<List<Freebie>> GetFreebieStoreAsync(Guid groupBuyId)
        {
            return [.. (await GetQueryableAsync())
                .Where(w => DateTime.Now.Date >= w.ActivityStartDate && DateTime.Now.Date <= w.ActivityEndDate)
                .Where(x => x.IsFreebieAvaliable)
                .Include(x => x.FreebieGroupBuys)
                .Include(i => i.Images)
                .Where(x => x.ApplyToAllGroupBuy || x.FreebieGroupBuys.Select(x => x.GroupBuyId).Contains(groupBuyId))
            ];
        }

    }
}
