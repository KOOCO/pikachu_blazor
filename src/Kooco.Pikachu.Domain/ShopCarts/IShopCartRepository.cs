using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.ShopCarts;

public interface IShopCartRepository : IRepository<ShopCart, Guid>
{
    Task<ShopCart> FindByUserIdAsync(Guid userId, bool includeDetails = false, bool exception = false);
    Task<ShopCart> FindByUserIdAndGroupBuyIdAsync(Guid userId, Guid groupBuyId, bool includeDetails = false);
    Task<long> GetCountAsync(string? filter, Guid? userId, Guid? groupBuyId, bool includeDetails = false);
    Task<List<ShopCart>> GetListAsync(int skipCount, int maxResultCount, string sorting, string? filter, Guid? userId, Guid? groupBuyId, bool includeDetails = false);
    Task<IQueryable<ShopCart>> GetFilteredQueryableAsync(string? filter, Guid? userId, Guid? groupBuyId, bool includeDetails = false);
    Task<ShopCart> FindByCartItemIdAsync(Guid cartItemId, bool includeDetails = false, bool exception = false);
    Task<CartItem> FindCartItemAsync(Guid userId, Guid itemId, bool includeDetails = false);
}
