using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.AutomaticEmails
{
    public interface IAutomaticEmailRepository : IRepository<AutomaticEmail, Guid>
    {
        Task<long> GetCountAsync();
        Task<List<AutomaticEmail>> GetListAsync(int skipCount, int maxResultCount, string sorting);
    }
}
