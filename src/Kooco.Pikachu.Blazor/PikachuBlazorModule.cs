using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Kooco.Pikachu.Blazor.Menus;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.MultiTenancy;
using OpenIddict.Validation.AspNetCore;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Blazor.Server;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.Blazor.Server;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Blazor.Server;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.OpenIddict;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Lsw.Abp.AspnetCore.Components.Server.AntDesignTheme.Bundling;
using Lsw.Abp.IdentityManagement.Blazor.Server.AntDesignUI;
using Lsw.Abp.SettingManagement.Blazor.Server.AntDesignUI;
using Lsw.Abp.TenantManagement.Blazor.Server.AntDesignUI;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;

namespace Kooco.Pikachu.Blazor;

[DependsOn(
    typeof(PikachuApplicationModule),
    typeof(PikachuEntityFrameworkCoreModule),
    typeof(PikachuHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpIdentityBlazorServerAntDesignModule),
    typeof(AbpTenantManagementBlazorServerAntDesignModule),
    typeof(AbpSettingManagementBlazorServerAntDesignModule),
       typeof(AbpAspNetCoreMvcUiBasicThemeModule)
   )]
public class PikachuBlazorModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(PikachuResource),
                typeof(PikachuDomainModule).Assembly,
                typeof(PikachuDomainSharedModule).Assembly,
                typeof(PikachuApplicationModule).Assembly,
                typeof(PikachuApplicationContractsModule).Assembly,
                typeof(PikachuBlazorModule).Assembly
            );
        });

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Pikachu");
                options.UseLocalServer();
                options.UseAspNetCore();
            });

            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment()) return;

            PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
            {
                options.AddDevelopmentEncryptionAndSigningCertificate = false;
            });

            PreConfigure<OpenIddictServerBuilder>(builder =>
            {
                builder.AddEncryptionCertificate(GetEncryptionCertificate(hostingEnvironment,
                    context.Services.GetConfiguration()));
                builder.AddSigningCertificate(
                    GetSigningCertificate(hostingEnvironment, context.Services.GetConfiguration()));
            });
        });

        var hostingEnvironment = context.Services.GetHostingEnvironment();

        if (hostingEnvironment.IsDevelopment()) return;

        PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
        {
            options.AddDevelopmentEncryptionAndSigningCertificate = false;
        });

        PreConfigure<OpenIddictServerBuilder>(builder =>
        {
            builder.AddEncryptionCertificate(GetEncryptionCertificate(hostingEnvironment,
                context.Services.GetConfiguration()));
            builder.AddSigningCertificate(
                GetSigningCertificate(hostingEnvironment, context.Services.GetConfiguration()));
        });



    }

    private X509Certificate2 GetEncryptionCertificate(IWebHostEnvironment environment, IConfiguration config)
    {
        var fileName = "encryption-certificate.pfx";
        var password = config["MyAppCertificate:X590:Password"];

        var file = Path.Combine(environment.ContentRootPath, fileName);
        if (File.Exists(file))
        {
            var created = File.GetCreationTime(file);
            var days = (DateTime.Now - created).TotalDays;
            if (days > 180)
            {
                File.Delete(file);
            }
            else
            {
                return new X509Certificate2(file, password, X509KeyStorageFlags.MachineKeySet);
            }
        }


        using var algorithm = RSA.Create(keySizeInBits: 2048);
        var subject = new X500DistinguishedName("CN=Fabrikam Encryption Certificate");
        var request = new CertificateRequest(subject, algorithm,
            HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(
            X509KeyUsageFlags.KeyEncipherment, critical: true));
        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddYears(2));
        File.WriteAllBytes(file, certificate.Export(X509ContentType.Pfx, password));
        return new X509Certificate2(file, password, X509KeyStorageFlags.MachineKeySet);
    }

    private X509Certificate2 GetSigningCertificate(IWebHostEnvironment environment, IConfiguration config)
    {
        var fileName = "signing-certificate.pfx";
        var password = config["MyAppCertificate:X590:Password"];
        var file = Path.Combine(environment.ContentRootPath, fileName);

        if (File.Exists(file))
        {
            var created = File.GetCreationTime(file);
            var days = (DateTime.Now - created).TotalDays;
            if (days > 180)
            {
                File.Delete(file);
            }
            else
            {
                return new X509Certificate2(file, password, X509KeyStorageFlags.MachineKeySet);
            }
        }

        using var algorithm = RSA.Create(keySizeInBits: 2048);
        var subject = new X500DistinguishedName("CN=Fabrikam Signing Certificate");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature,
            critical: true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

        File.WriteAllBytes(file, certificate.Export(X509ContentType.Pfx, password));
        return new X509Certificate2(file, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureAuthentication(context);
        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureSwaggerServices(context.Services);
        ConfigureAutoApiControllers();
        ConfigureRouter(context);
        ConfigureMenu(context);
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
            options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"]?.Split(',') ?? Array.Empty<string>());
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            // MVC UI
            options.StyleBundles.Configure(
                BasicThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            //BLAZOR UI
            options.StyleBundles.Configure(
            BlazorAntDesignThemeBundles.Styles.Global,
            bundle =>
            {
                bundle.AddFiles("/blazor-global-styles.css");
                //You can remove the following line if you don't use Blazor CSS isolation for components
                bundle.AddFiles("/Kooco.Pikachu.Blazor.styles.css");
            }
            );
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<PikachuDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Kooco.Pikachu.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<PikachuDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Kooco.Pikachu.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<PikachuApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Kooco.Pikachu.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<PikachuApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Kooco.Pikachu.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<PikachuBlazorModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pikachu API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }



    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new PikachuMenuContributor());
        });
    }

    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<Lsw.Abp.AspnetCore.Components.Web.AntDesignTheme.Routing.AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(PikachuBlazorModule).Assembly;
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(PikachuApplicationModule).Assembly);
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<PikachuBlazorModule>();
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        app.UseAbpRequestLocalization();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pikachu API");
        });
        app.UseConfiguredEndpoints();
    }
}
