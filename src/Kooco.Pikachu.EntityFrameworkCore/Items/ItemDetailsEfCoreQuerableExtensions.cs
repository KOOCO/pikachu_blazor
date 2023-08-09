using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public static class ItemDetailsEfCoreQueryableExtensions
{
    public static IQueryable<ItemDetails> IncludeDetails(this IQueryable<ItemDetails> queryable, bool include = true)
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
