using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public interface IFooterSettingRepository : IRepository<FooterSetting, Guid>
{
    Task<FooterSetting?> FirstOrDefaultAsync();
}
