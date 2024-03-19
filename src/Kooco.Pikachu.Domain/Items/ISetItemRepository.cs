using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public interface ISetItemRepository : IRepository<SetItem, Guid>
{
    Task<SetItem> GetWithDetailsAsync(Guid id);
    Task DeleteManyAsync(List<Guid> ids);
    Task<List<ItemWithItemType>> GetItemsLookupAsync();
    Task<List<ItemWithItemType>> GetAvailableItemsLookupAsync();
}
