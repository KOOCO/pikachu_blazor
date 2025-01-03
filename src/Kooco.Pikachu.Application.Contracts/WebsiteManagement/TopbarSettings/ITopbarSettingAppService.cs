using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

public interface ITopbarSettingAppService : IApplicationService
{
    Task<TopbarSettingDto?> FirstOrDefaultAsync();
    Task<TopbarSettingDto> UpdateAsync(UpdateTopbarSettingDto input);
}