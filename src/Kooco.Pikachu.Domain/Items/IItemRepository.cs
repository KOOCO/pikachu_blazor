using System;
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
}
