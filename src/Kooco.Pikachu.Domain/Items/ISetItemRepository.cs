using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public interface ISetItemRepository : IRepository<SetItem, Guid>
{
    Task<SetItem> GetWithDetailsAsync(Guid id);
}
