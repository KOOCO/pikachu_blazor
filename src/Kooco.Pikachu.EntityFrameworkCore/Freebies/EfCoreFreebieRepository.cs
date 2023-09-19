using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Groupbuys;
using Kooco.Pikachu.GroupBuys;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    }
}
