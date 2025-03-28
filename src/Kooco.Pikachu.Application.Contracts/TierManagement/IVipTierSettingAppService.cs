using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.TierManagement;

public interface IVipTierSettingAppService : IApplicationService
{
    Task<VipTierSettingDto> FirstOrDefaultAsync();
    Task<VipTierSettingDto> UpdateAsync(UpdateVipTierSettingDto input);
}
