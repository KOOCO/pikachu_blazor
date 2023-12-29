using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.AutomaticEmails
{
    public class EfCoreAutomaticEmailRepository : EfCoreRepository<PikachuDbContext, AutomaticEmail, Guid>, IAutomaticEmailRepository
    {
        public EfCoreAutomaticEmailRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<long> GetCountAsync()
        {
            var query = await GetQueryableAsync();
            return await query.LongCountAsync();
        }

        public async Task<List<AutomaticEmail>> GetListAsync(int skipCount, int maxResultCount, string sorting)
        {
            var query = await GetQueryableAsync();
            return await query
                .OrderBy(sorting)
                .PageBy(skipCount, maxResultCount)
                .ToListAsync();
        }
    }
}
