using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.EdmManagement;

public interface IEdmRepository : IRepository<Edm, Guid>
{
    Task<long> CountAsync(string? filter = null, EdmTemplateType? templateType = null, Guid? campaignId = null,
        bool? applyToAllMembers = null, IEnumerable<string>? memberTags = null, Guid? groupBuyId = null,
        DateTime? startDate = null, DateTime? endDate = null, DateTime? minSendTime = null, DateTime? maxSendTime = null,
        EdmSendFrequency? sendFrequency = null);

    Task<List<Edm>> GetListAsync(int skipCount = 0, int maxResultCount = 10, string? sorting = null,
        string? filter = null, EdmTemplateType? templateType = null, Guid? campaignId = null,
        bool? applyToAllMembers = null, IEnumerable<string>? memberTags = null, Guid? groupBuyId = null,
        DateTime? startDate = null, DateTime? endDate = null, DateTime? minSendTime = null, DateTime? maxSendTime = null,
        EdmSendFrequency? sendFrequency = null);

    Task<IQueryable<Edm>> GetFilteredQueryableAsync(string? filter = null, EdmTemplateType? templateType = null,
        Guid? campaignId = null, bool? applyToAllMembers = null, IEnumerable<string>? memberTags = null, Guid? groupBuyId = null,
        DateTime? startDate = null, DateTime? endDate = null, DateTime? minSendTime = null, DateTime? maxSendTime = null, 
        EdmSendFrequency? sendFrequency = null);

    Task<List<string>> GetGroupBuyNamesAsync(List<Guid> groupBuyIds);
}
