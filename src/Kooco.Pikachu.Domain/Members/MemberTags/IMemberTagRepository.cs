using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Members.MemberTags;

public interface IMemberTagRepository : IRepository<MemberTag, Guid>
{
    Task<long> CountAsync(string? filter = null, bool? isSystemGenerated = null);
    Task<List<MemberTag>> GetListAsync(int skipCount = 0, int maxResultCount = 0, string? sorting = null, string? filter = null, bool? isSystemGenerated = null);
    Task<IQueryable<MemberTag>> GetFilteredQueryableAsync(string? filter = null, bool? isSystemGenerated = null);
    Task SetIsEnabledAsync(string name, bool isEnabled);
}
