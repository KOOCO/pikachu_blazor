using Kooco.Pikachu.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Kooco.Pikachu.EdmManagement;

public class EfCoreEdmRepository : EfCoreRepository<PikachuDbContext, Edm, Guid>, IEdmRepository
{
    public EfCoreEdmRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : base(dbContextProvider) { }

    public async Task<long> CountAsync(string? filter = null, EdmTemplateType? templateType = null, Guid? campaignId = null,
        bool? applyToAllMembers = null, IEnumerable<string>? memberTags = null, Guid? groupBuyId = null,
        DateTime? startDate = null, DateTime? endDate = null, DateTime? minSendTime = null, DateTime? maxSendTime = null,
        EdmSendFrequency? sendFrequency = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, templateType, campaignId, applyToAllMembers,
            memberTags, groupBuyId, startDate, endDate, minSendTime, maxSendTime, sendFrequency);
        return await queryable.LongCountAsync();
    }

    public async Task<List<Edm>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null,
        string? filter = null, EdmTemplateType? templateType = null, Guid? campaignId = null,
        bool? applyToAllMembers = null, IEnumerable<string>? memberTags = null, Guid? groupBuyId = null,
        DateTime? startDate = null, DateTime? endDate = null, DateTime? minSendTime = null, DateTime? maxSendTime = null,
        EdmSendFrequency? sendFrequency = null)
    {
        var queryable = await GetFilteredQueryableAsync(filter, templateType, campaignId, applyToAllMembers,
            memberTags, groupBuyId, startDate, endDate, minSendTime, maxSendTime, sendFrequency);

        var edms = await queryable
            .OrderBy(EdmConsts.GetSortingOrDefault(sorting))
            .PageBy(skipCount, maxResultCount)
            .ToListAsync();

        return edms;
    }

    public async Task<IQueryable<Edm>> GetFilteredQueryableAsync(string? filter = null, EdmTemplateType? templateType = null,
        Guid? campaignId = null, bool? applyToAllMembers = null, IEnumerable<string>? memberTags = null,
        Guid? groupBuyId = null, DateTime? startDate = null, DateTime? endDate = null,
        DateTime? minSendTime = null, DateTime? maxSendTime = null, EdmSendFrequency? sendFrequency = null)
    {
        var queryable = (await GetQueryableAsync())
            .Include(x => x.GroupBuy)
            .Include(x => x.Campaign);

        return queryable
            .WhereIf(templateType.HasValue, x => x.TemplateType == templateType)
            .WhereIf(campaignId.HasValue, x => x.CampaignId == campaignId)
            .WhereIf(applyToAllMembers.HasValue, x => x.ApplyToAllMembers == applyToAllMembers)
            .WhereIf(memberTags != null && memberTags.Any(), x => x.MemberTagsJson != null && memberTags.Any(mt => x.MemberTagsJson.Contains(mt)))
            .WhereIf(groupBuyId.HasValue, x => x.GroupBuyId == groupBuyId)
            .WhereIf(startDate.HasValue, x => x.StartDate >= startDate)
            .WhereIf(endDate.HasValue, x => x.EndDate <= endDate)
            .WhereIf(minSendTime.HasValue, x => x.SendTime.TimeOfDay >= minSendTime!.Value.TimeOfDay)
            .WhereIf(maxSendTime.HasValue, x => x.SendTime.TimeOfDay <= maxSendTime!.Value.TimeOfDay)
            .WhereIf(sendFrequency.HasValue, x => x.SendFrequency == sendFrequency)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.Id.ToString() == filter
            || x.Subject.Contains(filter) || x.Message.Contains(filter)
            || (x.MemberTagsJson != null && x.MemberTagsJson.Contains(filter))
            || (x.CampaignId != null && x.CampaignId.ToString() == filter)
            || (x.Campaign != null && x.Campaign.Name.Contains(filter))
            || x.GroupBuyId.ToString() == filter || (x.GroupBuy != null && x.GroupBuy.GroupBuyName.Contains(filter)));
    }

    public async Task<List<string>> GetGroupBuyNamesAsync(List<Guid> groupBuyIds)
    {
        var dbContext = await GetDbContextAsync();

        return await dbContext.GroupBuys
            .Where(gb => groupBuyIds.Contains(gb.Id))
            .Select(gb => gb.GroupBuyName)
            .ToListAsync();
    }
}
