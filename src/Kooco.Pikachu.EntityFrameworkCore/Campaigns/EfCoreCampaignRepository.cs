using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.Campaigns;

public class EfCoreCampaignRepository : EfCoreRepository<PikachuDbContext, Campaign, Guid>, ICampaignRepository
{
    public EfCoreCampaignRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider)
    {
    }

    public async Task<Campaign> GetWithDetailsAsync(Guid id)
    {
        var dbContext = await GetDbContextAsync();

        var campaign = await dbContext.Campaigns
            .Where(c => c.Id == id)
            .Include(c => c.Discount)
            .Include(c => c.ShoppingCredit)
                .ThenInclude(sc => sc.StageSettings)
            .Include(c => c.AddOnProduct)
            .Include(c => c.GroupBuys)
            .Include(c => c.Products)
            .FirstOrDefaultAsync();

        return campaign ?? throw new EntityNotFoundException(typeof(Campaign), id);
    }

    public async Task<long> CountAsync(string? filter, bool? isEnabled, DateTime? startDate, DateTime? endDate)
    {
        var queryable = await GetFilteredQueryableAsync(filter, isEnabled, startDate, endDate);
        return await queryable.LongCountAsync();
    }

    public async Task<List<Campaign>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter, bool? isEnabled, DateTime? startDate, DateTime? endDate)
    {
        var queryable = await GetFilteredQueryableAsync(filter, isEnabled, startDate, endDate);
        return await queryable
            .AsNoTracking()
            .Include(c => c.Discount)
            .Include(c => c.ShoppingCredit)
                .ThenInclude(sc => sc.StageSettings)
            .Include(c => c.AddOnProduct)
            .Include(c => c.GroupBuys)
            .Include(c => c.Products)
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? CampaignConsts.DefaultSorting : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();
    }

    public async Task<IQueryable<Campaign>> GetFilteredQueryableAsync(string? filter, bool? isEnabled, DateTime? startDate, DateTime? endDate)
    {
        filter ??= "";

        var queryable = await GetQueryableAsync();
        return queryable
            .WhereIf(!filter.IsNullOrWhiteSpace(), x => x.Name.Contains(filter) || x.Id.ToString() == filter
             || (x.TargetAudienceJson != null && x.TargetAudienceJson.Contains(filter)))
            .WhereIf(isEnabled.HasValue, x => x.IsEnabled == isEnabled)
            .WhereIf(startDate.HasValue, x => x.StartDate.Date >= startDate!.Value.Date)
            .WhereIf(endDate.HasValue, x => x.EndDate.Date <= endDate!.Value.Date);
    }

    public async Task<long> GetActiveCampaignsCountAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Campaigns
            .Where(c => c.IsEnabled)
            .LongCountAsync();
    }

    public async Task<List<Campaign>> GetListAsync(List<Guid> campaignIds)
    {
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(c => campaignIds.Contains(c.Id))
            .Include(c => c.Discount)
            .Include(c => c.ShoppingCredit)
                .ThenInclude(sc => sc.StageSettings)
            .Include(c => c.AddOnProduct)
            .ToListAsync();
    }
}
