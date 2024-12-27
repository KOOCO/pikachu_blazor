using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings;

public interface IFooterSettingAppService : IApplicationService
{
    Task<FooterSettingDto?> FirstOrDefaultAsync();
    Task<FooterSettingDto> UpdateAsync(UpdateFooterSettingDto input);
}