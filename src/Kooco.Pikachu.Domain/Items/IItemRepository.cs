using System;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Items;

/// <summary>
/// 
/// </summary>
public interface IItemRepository : IRepository<Item, Guid>
{
}
