using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Kooco.Pikachu.MultiTenancy;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.Configuration;
using Volo.Abp.BlobStoring;
using Kooco.Pikachu.AzureStorage.Image;
using Kooco.Pikachu.AzureStorage;
using Volo.Abp.BlobStoring.Azure;

namespace Kooco.Pikachu;

[DependsOn(
    typeof(PikachuDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpFeatureManagementDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpOpenIddictDomainModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpSettingManagementDomainModule),
    typeof(AbpTenantManagementDomainModule),
    typeof(AbpEmailingModule),
    typeof(AbpBlobStoringAzureModule)
)]
public class PikachuDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        ConfigureAzureStorageAccountOptions(context, configuration);
        ConfigureAbpBlobStoringOptions(configuration);

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية", "ae"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English", "gb"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("hr", "hr", "Croatian"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish", "fi"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français", "fr"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi", "in"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano", "it"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский", "ru"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak", "sk"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe", "tr"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
        });

        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });


#if DEBUG
        //context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
    }

    private void ConfigureAzureStorageAccountOptions(ServiceConfigurationContext context, IConfiguration configuration)
    {
        Configure<AzureStorageAccountOptions>(options => {
            var azureStorageConnectionString = configuration["AzureStorageAccountSettings:ConnectionString"];
            var azureStorageAccountUrl = configuration["AzureStorageAccountSettings:AccountUrl"];
            options.ConnectionString = azureStorageConnectionString; options.AccountUrl = azureStorageAccountUrl;
        });
    }

    private void ConfigureAbpBlobStoringOptions(IConfiguration configuration)
    {
        Configure<AbpBlobStoringOptions>(options => {
            var azureStorageConnectionString = configuration["AzureStorageAccountSettings:ConnectionString"];

            options.Containers.Configure<ImageContainer>(container => {
                container.UseAzure(azure => {
                    azure.ConnectionString = azureStorageConnectionString;
                    azure.CreateContainerIfNotExists = true;
                });
            });
        });
    }
}
