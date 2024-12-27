using Kooco.Pikachu.Permissions;
using Kooco.Pikachu.Validators;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Volo.Abp;

namespace Kooco.Pikachu.WebsiteManagement.TopbarSettings;

[RemoteService(IsEnabled = false)]
[Authorize(PikachuPermissions.WebsiteManagement.TopbarSettings)]
public class TopbarSettingAppService(ITopbarSettingRepository topbarSettingRepository) : PikachuAppService, ITopbarSettingAppService
{
    public async Task<TopbarSettingDto?> FirstOrDefaultAsync()
    {
        var topbarSettings = await topbarSettingRepository.FirstOrDefaultAsync();
        return ObjectMapper.Map<TopbarSetting, TopbarSettingDto>(topbarSettings);
    }

    public async Task<TopbarSettingDto> UpdateAsync(UpdateTopbarSettingDto input)
    {
        MyCheck.NotUndefinedOrNull<TopbarStyleSettings>(input.TopbarStyleSettings, nameof(input.TopbarStyleSettings));

        var topbarSettings = await topbarSettingRepository.FirstOrDefaultAsync();

        if (topbarSettings is null)
        {
            topbarSettings ??= new TopbarSetting(GuidGenerator.Create(), input.TopbarStyleSettings.Value);
            await topbarSettingRepository.InsertAsync(topbarSettings);
        }

        topbarSettings.Links.Clear();

        foreach (var link in input.Links)
        {
            var newLink = topbarSettings.AddLink(GuidGenerator.Create(), link.TopbarLinkSettings, link.Index, link.Title, link.Url);
            foreach (var categoryOption in link.CategoryOptions)
            {
                newLink.AddCategoryOption(GuidGenerator.Create(), categoryOption.TopbarCategoryLinkOption, categoryOption.Index, categoryOption.Title, categoryOption.Link);
            }
        }

        await topbarSettingRepository.UpdateAsync(topbarSettings);
        return ObjectMapper.Map<TopbarSetting, TopbarSettingDto>(topbarSettings);
    }
}
