using Kooco.Pikachu.ProductCategories;
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
    Task<List<ItemWithItemType>> GetItemsAllLookupAsync();
    Task<Item> GetFirstImage(Guid id);
    Task<long> LongCountAsync(
                        Guid? itemId = null,
                        DateTime? minAvailableTime = null,
                        DateTime? maxAvailableTime = null,
                        bool? isFreeShipping = null,
                        bool? isAvailable = null
                        );
    Task<List<ItemListViewModel>> GetItemsListAsync(
                                int skipCount,
                                int maxResultCount,
                                string sorting,
                                Guid? itemId = null,
                                DateTime? minAvailableTime = null,
                                DateTime? maxAvailableTime = null,
                                bool? isFreeShipping = null,
                                bool? isAvailable = null
                                );

    Task<Item> GetSKUAndItemAsync(Guid itemId, Guid itemDetailId);
    Task<List<Item>> GetManyAsync(List<Guid> itemIds);
    Task<ItemDetails?> FindItemDetailAsync(Guid itemDetailId);
    Task<List<CategoryProduct>> GetItemCategoriesAsync(Guid id);
}
