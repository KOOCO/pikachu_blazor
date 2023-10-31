using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public interface IItemRepository : IRepository<Item, Guid>
{
    Task<Item> FindByNameAsync(string itemName);
    Task<Item> FindBySKUAsync(string SKU);
    Task<IQueryable<Item>> GetWithImagesAsync(int? maxResultCount = null);
    Task DeleteManyAsync(List<Guid> ids);
    Task<List<ItemWithItemType>> GetItemsLookupAsync();
}
