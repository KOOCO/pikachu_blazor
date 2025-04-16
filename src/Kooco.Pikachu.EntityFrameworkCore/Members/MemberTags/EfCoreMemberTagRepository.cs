using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Members.MemberTags;

public class EfCoreMemberTagRepository : EfCoreRepository<PikachuDbContext, MemberTag, Guid>, IMemberTagRepository
{
    public EfCoreMemberTagRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<long> CountAsync(string? filter = null, bool? isSystemGenerated = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, isSystemGenerated);
        return await queryable.LongCountAsync();
    }

    public async Task<List<MemberTag>> GetListAsync(int skipCount = 0, int maxResultCount = 0, string? sorting = null, string? filter = null, bool? isSystemGenerated = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, isSystemGenerated);
        return await queryable
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? MemberTagConsts.DefaultSorting : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<IQueryable<MemberTag>> GetFilteredQueryableAsync(string? filter = null, bool? isSystemGenerated = null)
    {
        var queryable = await GetQueryableAsync();

        queryable = queryable
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.Name.Contains(filter))
            .WhereIf(isSystemGenerated.HasValue, x => x.IsSystemAssigned == isSystemGenerated);

        var minIdPerName = queryable.GroupBy(tag => tag.Name).Select(tag => tag.Min(t => t.Id));

        return queryable
            .Where(q => minIdPerName.Contains(q.Id));
    }

    public async Task SetIsEnabledAsync(string name, bool isEnabled)
    {
        var queryable = await GetQueryableAsync();

        await queryable
            .Where(tag => tag.Name == name)
            .ExecuteUpdateAsync(tag => tag.SetProperty(p => p.IsEnabled, isEnabled));
    }
}
