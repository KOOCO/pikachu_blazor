using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Kooco.Pikachu.WebsiteManagement;

public class WebsiteSettingsManager(IWebsiteSettingsRepository websiteSettingsRepository) : DomainService
{
    public async Task<WebsiteSettings> CreateAsync(string notificationBar, string logoName, string logoUrl, string storeTitle,
        WebsiteTitleDisplayOptions titleDisplayOption, string facebook, string instagram, string line, string returnExchangePolicy)
    {
        ValidateInputs(notificationBar, logoName, logoUrl, storeTitle, titleDisplayOption, facebook, instagram, line, returnExchangePolicy);

        var websiteSettings = new WebsiteSettings(GuidGenerator.Create(), notificationBar, logoName, logoUrl, storeTitle,
            titleDisplayOption, facebook, instagram, line, returnExchangePolicy);

        await websiteSettingsRepository.InsertAsync(websiteSettings);

        return websiteSettings;
    }

    public async Task<WebsiteSettings> UpdateAsync(WebsiteSettings websiteSettings, string notificationBar, string logoName, string logoUrl, string storeTitle,
       WebsiteTitleDisplayOptions titleDisplayOption, string facebook, string instagram, string line, string returnExchangePolicy)
    {
        Check.NotNull(websiteSettings, nameof(WebsiteSettings));
        ValidateInputs(notificationBar, logoName, logoUrl, storeTitle, titleDisplayOption, facebook, instagram, line, returnExchangePolicy);

        websiteSettings.SetNotificationBar(notificationBar);
        websiteSettings.SetLogoName(logoName);
        websiteSettings.SetLogoUrl(logoUrl);
        websiteSettings.SetStoreTitle(storeTitle);
        websiteSettings.SetTitleDisplayOption(titleDisplayOption);
        websiteSettings.SetFacebook(facebook);
        websiteSettings.SetInstagram(instagram);
        websiteSettings.SetLine(line);
        websiteSettings.SetReturnExchangePolicy(returnExchangePolicy);

        await websiteSettingsRepository.UpdateAsync(websiteSettings);

        return websiteSettings;
    }

    private static void ValidateInputs(string notificationBar, string logoName, string logoUrl, string storeTitle,
        WebsiteTitleDisplayOptions titleDisplayOption, string facebook, string instagram, string line, string returnExchangePolicy)
    {
        Check.NotNullOrWhiteSpace(notificationBar, nameof(notificationBar), maxLength: WebsiteSettingsConsts.MaxNotificationBarLength);
        Check.NotNullOrWhiteSpace(logoName, nameof(logoName), maxLength: WebsiteSettingsConsts.MaxLogoNameLength);
        Check.NotNullOrWhiteSpace(logoUrl, nameof(logoUrl), maxLength: WebsiteSettingsConsts.MaxLogoUrlLength);
        Check.NotNullOrWhiteSpace(storeTitle, nameof(storeTitle), maxLength: WebsiteSettingsConsts.MaxStoreTitleLength);
        Check.NotNullOrWhiteSpace(facebook, nameof(facebook), maxLength: WebsiteSettingsConsts.MaxFacebookLength);
        Check.NotNullOrWhiteSpace(instagram, nameof(instagram), maxLength: WebsiteSettingsConsts.MaxInstagramLength);
        Check.NotNullOrWhiteSpace(line, nameof(line), maxLength: WebsiteSettingsConsts.MaxLineLength);
        Check.NotNullOrWhiteSpace(returnExchangePolicy, nameof(returnExchangePolicy), maxLength: WebsiteSettingsConsts.MaxReturnExchangePolicyLength);
    }
}
