using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public interface ITopbarSettingRepository : IRepository<TopbarSetting, Guid>
{
    Task<TopbarSetting?> FirstOrDefaultAsync();
}
