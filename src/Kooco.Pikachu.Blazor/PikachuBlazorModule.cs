using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.FluentValidation;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using DinkToPdf;
using DinkToPdf.Contracts;
using FluentValidation;
using Hangfire;
using Hangfire.SqlServer;
using Kooco.Pikachu.BackgroundWorkers;
using Kooco.Pikachu.Blazor.Helpers;
using Kooco.Pikachu.Blazor.Menus;
using Kooco.Pikachu.Blazor.OpenIddict;
using Kooco.Pikachu.Blazor.Pages.InboxManagement.Toolbar;
using Kooco.Pikachu.Blazor.Pages.TenantManagement;
using Kooco.Pikachu.CodTradeInfos;
using Kooco.Pikachu.EntityFrameworkCore;
using Kooco.Pikachu.ImageCompressors;
using Kooco.Pikachu.InboxManagement;
using Kooco.Pikachu.Interfaces;
using Kooco.Pikachu.Localization;
using Kooco.Pikachu.MultiTenancy;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.PaymentGateways.LinePay;
using Kooco.Pikachu.Reconciliations;
using Kooco.Pikachu.Services;
using Kooco.Pikachu.UserShoppingCredits;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using OpenIddict.Validation.AspNetCore;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Components.Server.LeptonXLiteTheme;
using Volo.Abp.AspNetCore.Components.Server.LeptonXLiteTheme.Bundling;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite.Bundling;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Identity.Blazor.Server;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.ExtensionGrantTypes;
using Volo.Abp.SettingManagement.Blazor.Server;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Blazor.Server;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace Kooco.Pikachu.Blazor;

[DependsOn(
    typeof(PikachuApplicationModule),
    typeof(PikachuEntityFrameworkCoreModule),
    typeof(PikachuHttpApiModule),
    typeof(AbpAutofacModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAspNetCoreComponentsServerLeptonXLiteThemeModule),
    typeof(AbpAspNetCoreMvcUiLeptonXLiteThemeModule),
    typeof(AbpIdentityBlazorServerModule),
    typeof(AbpTenantManagementBlazorServerModule),
    typeof(AbpSettingManagementBlazorServerModule),
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpAspNetCoreSignalRModule)
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

        context.Services.AddBlazoriseRichTextEdit();
        context.Services.AddAntDesign();
        context.Services.AddTransient<OrderDeliveryBackgroundJob>();

        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("Pikachu");
                options.UseLocalServer();
                options.UseAspNetCore();
            });

            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (!hostingEnvironment.IsDevelopment())
            {
                PreConfigure<AbpOpenIddictAspNetCoreOptions>(options =>
                {
                    options.AddDevelopmentEncryptionAndSigningCertificate = false;
                });

                PreConfigure<OpenIddictServerBuilder>(serverBuilder =>
                {
                    var configuration = context.Services.GetConfiguration();
                    serverBuilder.AddProductionEncryptionAndSigningCertificate("openiddict.pfx", configuration["MyAppCertificate:X590:PassWord"]);
                    //serverBuilder.AddEncryptionCertificate(GetEncryptionCertificate(hostingEnvironment, context.Services.GetConfiguration()));
                    //serverBuilder.AddSigningCertificate(GetSigningCertificate(hostingEnvironment, context.Services.GetConfiguration()));
                });
            }
        });

        PreConfigure<OpenIddictServerBuilder>(builder =>
        {
            builder.Configure(openIddictServerOptions =>
            {
                openIddictServerOptions.GrantTypes.Add(UserIdTokenExtensionGrant.ExtensionGrantName);
            });
            builder.SetAccessTokenLifetime(TimeSpan.FromDays(7));
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
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureMenu(context);
        ConfigureSignalRHubOptions();
        ConfigureHangfire(context, configuration);
        ConfigureLoggerService(context, configuration);
        ConfigureOptions(context);

        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new InboxToolbarContributor());
        });

        if (!hostingEnvironment.IsDevelopment())
        {
            Configure<AbpTenantResolveOptions>(options =>
            {
                options.TenantResolvers.Insert(0, new MyDomainTenantResolveContributor());
            });
        }
        Configure<AbpOpenIddictExtensionGrantsOptions>(options =>
        {
            options.Grants.Add(UserIdTokenExtensionGrant.ExtensionGrantName, new UserIdTokenExtensionGrant());
        });
        context.Services.AddScoped<CustomTenantManagement>();
        context.Services.AddScoped<ExcelDownloadHelper>();
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = true;
        });

        ConfigurePaymentGateways(context);

        context.Services.AddScoped<OrderStatusCheckerWorker>();
        context.Services.AddScoped<PassiveUserBirthdayCheckerWorker>();
        context.Services.AddSingleton<OrderExpirationWorker>();

        context.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
    }

    private void ConfigurePaymentGateways(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IConfigureOptions<LinePayConfiguration>, ConfigureLinePayOptions>();
    }

    private void ConfigureOptions(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IConfigureOptions<ImageCompressorConfiguration>, ConfigureImageCompressorOptions>();
    }

    private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(configuration.GetConnectionString("HangFire"), new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true
            });

        });

        context.Services.AddHangfireServer(options =>
        {
            options.Queues = ["automatic-emails-job", "automatic-issue-invoice"];
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context)
    {
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }
    private static void ConfigureLoggerService(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var env = Environment.GetEnvironmentVariable("Environment");
        if (env.IsNullOrWhiteSpace())
        {
            env = "Local";
        }

        var logConfig = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
            .MinimumLevel.Override("Volo.Abp.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", $"{Globals.PikachuConsts.ApplicationName}-{env}")
            .WriteTo.Async(c => c.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                fileSizeLimitBytes: 100 * 1024 * 1024,
                shared: true))

#if RELEASE
            .WriteTo.Seq(configuration["SeqLog:ServerUrl"] ?? "", apiKey: configuration["SeqLog:ApiKey"])
#endif
        .WriteTo.Async(c => c.Console());

        Serilog.Debugging.SelfLog.Enable(Console.Error);
        Log.Logger = logConfig.CreateLogger();

        context.Services.AddSingleton(Log.Logger);
        context.Services.AddTransient(typeof(ILoggerService<>), typeof(SerilogService<>));
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
                       LeptonXLiteThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            //BLAZOR UI
            options.StyleBundles.Configure(
               BlazorLeptonXLiteThemeBundles.Styles.Global,
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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pikachu API", Version = "v1", Description = "Application API" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);

            }
        );
    }

    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            .AddBlazoriseFluentValidation()
            .AddBlazorise(options =>
            {
                options.Debounce = true;
                options.DebounceInterval = 1000;
            })
            .AddMudServices();

        context.Services.AddValidatorsFromAssembly(typeof(PikachuApplicationModule).Assembly);
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
        Configure<AbpRouterOptions>(options =>
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

    public override async void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        app.UseAbpRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("zh-Hant");
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider()
            };
        });

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
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAbpOpenIddictValidation();
        app.UseCors();

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

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new HangfireAuthorizationFilter() }
        });

        app.UseConfiguredEndpoints();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHub<OrderNotificationHub>("/signalr-order-notifications");
            endpoints.MapHub<NotificationHub>("/signalr-notifications");
        });
        _ = context.ServiceProvider
            .GetRequiredService<IBackgroundWorkerManager>()
            .AddAsync(
             context
                 .ServiceProvider
                 .GetRequiredService<PassiveUserBirthdayCheckerWorker>()

            );
        _ = context.ServiceProvider
            .GetRequiredService<IBackgroundWorkerManager>()
            .AddAsync(
             context
                 .ServiceProvider
                 .GetRequiredService<OrderExpirationWorker>()

            );
        var backgroundJobManager = context.ServiceProvider.GetRequiredService<IBackgroundJobManager>();
        var config = context.GetConfiguration();

        RecurringJob.AddOrUpdate<OrderDeliveryBackgroundJob>(
            "DailyOrderStatusUpdate",         // Job identifier
            job => job.ExecuteAsync(0),      // Method to call
            "0 1 * * *"                     // Cron expression for 1 AM daily
        );

        RecurringJob.AddOrUpdate<UserShoppingCreditExpireBackgroundJob>(
           "UserShoppingCreditExpire",
           job => job.ExecuteAsync(0),
           Cron.Daily()
        );

        RecurringJob.AddOrUpdate<CloseOrderBackgroundJob>(
            "CloseOrderBackgroundJob",
            job => job.ExecuteAsync(new CloseOrderBackgroundJobArgs()),
            Cron.Daily(1)
        );

        if (config["App:Environment"] != "Development")
        {
            RecurringJob.AddOrUpdate<EcPayReconciliationJob>(
                "EcPayReconciliationDailyJob",
                job => job.ExecuteAsync(0),
                "0 9 * * *",
                new RecurringJobOptions
                {
                    TimeZone = TaipeiTime.TaipeiTimeZone
                }
            );

            RecurringJob.AddOrUpdate<EcPayCodTradeInfoJob>(
                "EcPayCodTradeInfoJob",
                job => job.ExecuteAsync(0),
                "0 9 * * *",
                new RecurringJobOptions
                {
                    TimeZone = TaipeiTime.TaipeiTimeZone
                }
            );
        }
    }

    // This method is required for the Image Upload in blazor
    // To avoid Did not receive data in allotted time
    private void ConfigureSignalRHubOptions()
    {
        Configure<HubOptions>(options =>
        {
            options.DisableImplicitFromServicesParameters = true;
            options.MaximumReceiveMessageSize = 100 * 1024 * 1024;
        });
    }
}