using Kooco.Pikachu.EntityFrameworkCore;
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
    public class EfCoreShoppingCreditEarnSettingRepository : EfCoreRepository<PikachuDbContext, ShoppingCreditEarnSetting, Guid>, IShoppingCreditEarnSettingRepository
    {
        public EfCoreShoppingCreditEarnSettingRepository(IDbContextProvider<PikachuDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<ShoppingCreditEarnSetting> GetWithDetailsAsync(Guid id)
        {
            return await (await GetDbSetAsync())
                .Include(x => x.SpecificProducts)
                //.Include(x => x.SpecificShipping)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}