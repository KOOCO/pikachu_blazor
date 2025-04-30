using Kooco.Pikachu.Common;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.EnumValues;
using Kooco.Pikachu.Members;
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

        ShopCart? shopCart = await IncludeDetails(queryable, includeDetails).FirstOrDefaultAsync();

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
                    .ThenInclude(x => x.Item)
                .Include(x => x.CartItems)
                    .ThenInclude(x => x.SetItem);
        }
        return queryable;
    }

    public async Task<PagedResultModel<ShopCartListWithDetailsModel>> GetListWithDetailsAsync(int skipCount, int maxResultCount, string? sorting,
        string? filter, Guid? userId, Guid? groupBuyId, int? minItems, int? maxItems, int? minAmount, int? maxAmount,
        string? vipTier, string? memberStatus)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.ShopCarts
            .AsNoTracking()
            .Include(sc => sc.CartItems)
            .Include(sc => sc.User)
            .WhereIf(userId.HasValue, sc => sc.UserId == userId)
            .WhereIf(groupBuyId.HasValue, sc => sc.GroupBuyId == groupBuyId)
            .WhereIf(!string.IsNullOrWhiteSpace(filter), sc =>
                sc.Id.ToString().Contains(filter!) ||
                sc.UserId.ToString().Contains(filter!) ||
                (sc.User != null && (sc.User.UserName.Contains(filter!) || sc.User.Email.Contains(filter))));

        var projectedQuery = query.Select(sc => new ShopCartListWithDetailsModel
        {
            Id = sc.Id,
            UserId = sc.UserId,
            User = sc.User,
            GroupBuyId = sc.GroupBuyId,
            TotalItems = sc.CartItems.Sum(ci => ci.Quantity),
            TotalAmount = sc.CartItems.Sum(ci => (ci.Quantity * ci.UnitPrice))
        });

        var results = await projectedQuery
            .WhereIf(minItems.HasValue, sc => sc.TotalItems >= minItems)
            .WhereIf(maxItems.HasValue, sc => sc.TotalAmount <= maxItems)
            .WhereIf(minAmount.HasValue, sc => sc.TotalAmount >= minAmount)
            .WhereIf(maxAmount.HasValue, sc => sc.TotalAmount <= maxAmount)
            .ToListAsync();

        await EnrichShopCartsWithTagsAndGroupBuyInfoAsync(dbContext, results);

        results = [.. results
            .WhereIf(!string.IsNullOrWhiteSpace(vipTier), sc => sc.VipTier == vipTier)
            .WhereIf(!string.IsNullOrWhiteSpace(memberStatus), sc => sc.MemberStatus == memberStatus)];

        results = [.. results
            .AsQueryable()
            .OrderBy(string.IsNullOrWhiteSpace(sorting) ? nameof(ShopCartListWithDetailsModel.UserName) : sorting)];

        var totalCount = results.Count;

        return new PagedResultModel<ShopCartListWithDetailsModel>
        {
            TotalCount = totalCount,
            Items = results.Skip(skipCount).Take(maxResultCount).ToList()
        };
    }

    private async static Task EnrichShopCartsWithTagsAndGroupBuyInfoAsync(PikachuDbContext dbContext, List<ShopCartListWithDetailsModel> carts)
    {
        var userIds = carts.Select(c => c.UserId).Distinct().ToList();
        var groupBuyIds = carts.Select(c => c.GroupBuyId).Distinct().ToList();

        var tags = await dbContext.MemberTags
            .Where(mt => userIds.Contains(mt.UserId))
            .Where(mt => mt.VipTierId.HasValue || mt.IsSystemAssigned)
            .ToListAsync();

        var groupBuyMap = await dbContext.GroupBuys
            .Where(gb => groupBuyIds.Contains(gb.Id))
            .ToDictionaryAsync(gb => gb.Id, gb => gb.GroupBuyName);

        foreach (var cart in carts)
        {
            var userTags = tags.Where(t => t.UserId == cart.UserId).ToList();
            cart.VipTier = userTags.FirstOrDefault(t => t.VipTierId.HasValue)?.Name;
            cart.MemberStatus = userTags.FirstOrDefault(t => MemberConsts.MemberTags.Names.Contains(t.Name))?.Name;
            cart.GroupBuyName = groupBuyMap.TryGetValue(cart.GroupBuyId, out var name) ? name : null;
        }
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

    public async Task<List<CartItemWithDetailsModel>> GetCartItemsListAsync(Guid shopCartId)
    {
        var dbContext = await GetDbContextAsync();

        var cartItems = await dbContext.ShopCarts
            .Where(sc => sc.Id == shopCartId)
            .SelectMany(sc => sc.CartItems)
            .Select(ci => new CartItemWithDetailsModel
            {
                Id = ci.Id,
                ShopCartId = ci.ShopCartId,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                ItemId = ci.ItemId,
                SetItemId = ci.SetItemId,
                ItemType = ci.ItemId.HasValue
                    ? ItemType.Item
                    : (ci.SetItemId.HasValue ? ItemType.SetItem : ItemType.Freebie),

            }).ToListAsync();

        var itemIds = cartItems.Where(ci => ci.ItemId.HasValue).Select(ci => ci.ItemId).Distinct().ToList();
        var setItemIds = cartItems.Where(ci => ci.SetItemId.HasValue).Select(ci => ci.SetItemId).Distinct().ToList();

        var items = await dbContext.Items
            .Where(i => itemIds.Contains(i.Id))
            .Include(i => i.Images)
            .Select(i => new
            {
                i.Id,
                i.ItemName,
                Image = i.Images
                .OrderBy(image => image.SortNo)
                .Where(image => !string.IsNullOrWhiteSpace(image.ImageUrl))
                .Select(image => image.ImageUrl)
                .FirstOrDefault()
            }).ToListAsync();

        var setItems = await dbContext.SetItems
            .Where(si => setItemIds.Contains(si.Id))
            .Include(si => si.Images)
            .Select(si => new
            {
                si.Id,
                si.SetItemName,
                Image = si.Images
                .OrderBy(image => image.SortNo)
                .Where(image => !string.IsNullOrWhiteSpace(image.ImageUrl))
                .Select(image => image.ImageUrl)
                .FirstOrDefault()
            }).ToListAsync();

        cartItems.ForEach(cartItem =>
        {
            var item = cartItem.ItemId.HasValue
                ? items.Where(i => i.Id == cartItem.ItemId)
                       .Select(i => new { i.ItemName, i.Image })
                       .FirstOrDefault()
                : setItems.Where(si => si.Id == cartItem.SetItemId)
                          .Select(si => new { ItemName = si.SetItemName, si.Image })
                          .FirstOrDefault();

            if (item != null)
            {
                cartItem.ItemName = item.ItemName;
                cartItem.Image = item.Image;
            }
        });

        return cartItems;
    }

    public async Task<List<ShopCart>> GetListWithCartItemsAsync(List<Guid> ids = null!)
    {
        ids ??= [];
        var queryable = await GetQueryableAsync();
        return await queryable
            .WhereIf(ids.Count > 0, q => ids!.Contains(q.Id))
            .Include(q => q.CartItems)
            .ToListAsync();
    }
}
