using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public static class SetItemEfCoreQueryableExtensions
{
    public static IQueryable<SetItem> IncludeDetails(this IQueryable<SetItem> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }

        return queryable
            // .Include(x => x.xxx) // TODO: AbpHelper generated
            ;
    }
}
