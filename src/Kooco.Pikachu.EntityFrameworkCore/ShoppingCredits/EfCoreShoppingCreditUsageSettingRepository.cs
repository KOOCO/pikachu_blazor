﻿using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.ShoppingCredits
{
    public class EfCoreShoppingCreditUsageSettingRepository : EfCoreRepository<PikachuDbContext, ShoppingCreditUsageSetting, Guid>, IShoppingCreditUsageSettingRepository
    {
        public EfCoreShoppingCreditUsageSettingRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<ShoppingCreditUsageSetting> GetWithDetailsAsync(Guid id)
        {
            return await (await GetDbSetAsync())
                .Include(x => x.SpecificProducts)
                .Include(x => x.SpecificGroupbuys)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}