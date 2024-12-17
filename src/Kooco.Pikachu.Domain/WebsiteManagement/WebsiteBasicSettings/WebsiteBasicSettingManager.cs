using Kooco.Pikachu.EnumValues;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.WebsiteManagement.WebsiteBasicSettings;

public class WebsiteBasicSettingManager(IRepository<WebsiteBasicSetting, Guid> websiteBasicSettingRepository) : DomainService
{
    public async Task<WebsiteBasicSetting> UpdateAsync(bool isEnabled, WebsiteTitleDisplayOptions titleDisplayOption, string storeTitle,
        string description, string logoName, string logoUrl, GroupBuyTemplateType templateType, ColorScheme colorScheme,
        string primaryColor, string secondaryColor, string backgroundColor, string secondaryBackgroundColor, string alertColor)
    {
        var websiteBasicSettings = await websiteBasicSettingRepository.FirstOrDefaultAsync();

        if (websiteBasicSettings == null)
        {
            websiteBasicSettings = new(GuidGenerator.Create(), isEnabled, titleDisplayOption, storeTitle, description,
            logoName, logoUrl, templateType, colorScheme, primaryColor, secondaryColor, backgroundColor,
            secondaryBackgroundColor, alertColor);

            await websiteBasicSettingRepository.InsertAsync(websiteBasicSettings);
        }
        else
        {
            websiteBasicSettings.SetIsEnabled(isEnabled);
            websiteBasicSettings.SetStoreTitle(storeTitle);
            websiteBasicSettings.SetDescription(description);
            websiteBasicSettings.SetLogo(logoName, logoUrl);
            websiteBasicSettings.SetColorScheme(colorScheme, primaryColor, secondaryColor, backgroundColor, secondaryBackgroundColor, alertColor);
            websiteBasicSettings.TitleDisplayOption = titleDisplayOption;
            websiteBasicSettings.TemplateType = templateType;

            await websiteBasicSettingRepository.UpdateAsync(websiteBasicSettings);
        }

        return websiteBasicSettings;
    }

    public async Task<WebsiteBasicSetting> SetIsEnabledAsync(bool isEnabled)
    {
        var websiteBasicSettings = await websiteBasicSettingRepository.FirstOrDefaultAsync()
            ?? throw new EntityNotFoundException(nameof(WebsiteBasicSetting));

        websiteBasicSettings.SetIsEnabled(isEnabled);
        await websiteBasicSettingRepository.UpdateAsync(websiteBasicSettings);
        return websiteBasicSettings;
    }
}
