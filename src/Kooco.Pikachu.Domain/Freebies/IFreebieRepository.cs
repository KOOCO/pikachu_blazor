using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.Freebies
{
    public interface IFreebieRepository : IRepository<Freebie, Guid>
    {
        Task<Freebie> FindByNameAsync(string itemName);

    }
}
