using System.Threading.Tasks;

namespace Kooco.Pikachu.WebsiteManagement.FooterSettings
{
    public interface IFooterSettingAppService
    {
        Task<FooterSettingDto?> FirstOrDefaultAsync();
        Task<FooterSettingDto> UpdateAsync(UpdateFooterSettingDto input);
    }
}