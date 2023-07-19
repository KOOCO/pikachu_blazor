using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public static class ItemEfCoreQueryableExtensions
{
    public static IQueryable<Item> IncludeDetails(this IQueryable<Item> queryable, bool include = true)
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
