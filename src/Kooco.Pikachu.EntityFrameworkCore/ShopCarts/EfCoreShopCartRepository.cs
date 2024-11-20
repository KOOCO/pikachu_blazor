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

namespace Kooco.Pikachu.ShopCarts;

public class EfCoreShopCartRepository(IDbContextProvider<PikachuDbContext> dbContextProvider) : EfCoreRepository<PikachuDbContext, ShopCart, Guid>(dbContextProvider), IShopCartRepository
{
    public async Task<ShopCart> FindByUserIdAsync(Guid userId, bool includeDetails = false, bool exception = false)
    {
        var queryable = (await GetQueryableAsync()).Where(x => x.UserId == userId);
        var shopCart = await IncludeDetails(queryable, includeDetails).FirstOrDefaultAsync();
        if (exception && shopCart == null)
        {
            throw new EntityNotFoundException(typeof(ShopCart), userId);
        }
        return shopCart;
    }

    public async Task<ShopCart> FindByUserIdAndGroupBuyIdAsync(Guid userId, Guid groupBuyId, bool includeDetails = false)
    {
        IQueryable<ShopCart> queryable = (await GetQueryableAsync()).Where(w => w.UserId == userId && w.GroupBuyId == groupBuyId);

        ShopCart? shopCart = await IncludeDetails(queryable, includeDetails).FirstOrDefaultAsync() ?? 
                             throw new EntityNotFoundException(typeof(ShopCart), groupBuyId);

        return shopCart;
    }

    public async Task<long> GetCountAsync(string? filter, Guid? userId, Guid? groupBuyId, bool includeDetails = false)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, groupBuyId, includeDetails);
        return await queryable.LongCountAsync();
    }

    public async Task<List<ShopCart>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter, Guid? userId, Guid? groupBuyId, bool includeDetails = false)
    {
        var queryable = await GetFilteredQueryableAsync(filter, userId, groupBuyId, includeDetails);
        return await queryable.OrderBy(sorting).PageBy(skipCount, maxResultCount).ToListAsync();
    }

    public async Task<IQueryable<ShopCart>> GetFilteredQueryableAsync(string? filter, Guid? userId, Guid? groupBuyId, bool includeDetails = false)
    {
        var queryable = await GetQueryableAsync();

        queryable = queryable
            .WhereIf(userId.HasValue, x => x.UserId == userId)
            .WhereIf(groupBuyId.HasValue, w => w.GroupBuyId == groupBuyId)
            .Include(x => x.User)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), x => x.Id.ToString().Contains(filter)
            || x.UserId.ToString().Contains(filter) || (x.User != null && x.User.Name.Contains(filter)));

        return IncludeDetails(queryable, includeDetails);
    }

    private static IQueryable<ShopCart> IncludeDetails(IQueryable<ShopCart> queryable, bool includeDetails)
    {
        if (includeDetails)
        {
            queryable = queryable
                .Include(x => x.User)
                .Include(x => x.CartItems)
                    .ThenInclude(x => x.Item);
        }
        return queryable;
    }

    public async Task<CartItem> FindCartItemAsync(Guid userId, Guid itemId, bool includeDetails = false)
    {
        var queryable = (await GetQueryableAsync())
                        .Where(x => x.UserId == userId)
                        .Include(x => x.CartItems)
                        .SelectMany(x => x.CartItems)
                        .Where(x => x.ItemId == itemId);

        if (includeDetails)
        {
            queryable = queryable.Include(x => x.Item);
        }

        return await queryable.FirstOrDefaultAsync();
    }

    public async Task<ShopCart> FindByCartItemIdAsync(Guid cartItemId, bool includeDetails = false, bool exception = false)
    {
        var queryable = (await GetQueryableAsync())
                        .Include(x => x.CartItems)
                        .Where(x => x.CartItems.Any(ci => ci.Id == cartItemId));

        if (includeDetails)
        {
            queryable = queryable
                        .Include(x => x.User)
                        .Include(x => x.CartItems)
                            .ThenInclude(x => x.Item);
        }

        var shopCart = await queryable.FirstOrDefaultAsync();

        if (exception && shopCart is null)
        {
            throw new EntityNotFoundException(typeof(CartItem), cartItemId);
        }

        return shopCart;
    }
}
