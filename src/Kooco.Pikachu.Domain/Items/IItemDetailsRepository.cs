using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public interface IItemDetailsRepository : IRepository<ItemDetails, Guid>
{
    Task<long> CountAsync(string? filter);
    Task<List<ItemDetails>> GetListAsync(int skipCount, int maxResultCount, string? sorting, string? filter);
    Task<List<ItemDetails>> GetInventroyListAsync(int skipCount, int maxResultCount, string? sorting, string? filter);
    Task<List<ItemDetails>> GetWithItemId(Guid itemId);
}
