using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Kooco.Pikachu.Orders;
using Kooco.Pikachu.Orders.Interfaces;
using Kooco.Pikachu.GroupBuys;
using Kooco.Pikachu.GroupBuys.Interfaces;
using Kooco.Pikachu.PaymentStrategies;
using Kooco.Pikachu.Application.PaymentStrategies;
using Kooco.Pikachu.ShippingStrategies;
using Kooco.Pikachu.Application.ShippingStrategies;
using Kooco.Pikachu.Performance;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FluentValidation;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Kooco.Pikachu;

[DependsOn(
    typeof(PikachuDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(PikachuApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpFluentValidationModule),
    typeof(ECPayModule)
)]
public class PikachuApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<ECPayOptions>(configuration.GetSection(ECPayConstants.Name));
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = true; //Disables job execution in the same process
        });
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<PikachuApplicationModule>();
        });
       
        context.Services.AddAllHostedServices();

        // Register new Order services following SRP - optimized for performance
        // Use Scoped lifetime for services that maintain state within a request
        context.Services.AddScoped<IOrderPaymentService, OrderPaymentService>();
        context.Services.AddScoped<IOrderInventoryService, OrderInventoryService>();
        context.Services.AddScoped<IOrderNotificationService, OrderNotificationService>();
        context.Services.AddScoped<IOrderLogisticsService, OrderLogisticsService>();
        context.Services.AddScoped<IOrderInvoiceService, OrderInvoiceService>();
        context.Services.AddScoped<IOrderStatusService, OrderStatusService>();
        context.Services.AddScoped<IOrderCommentService, OrderCommentService>();
        context.Services.AddScoped<IOrderReportingService, OrderReportingService>();

        // Register new GroupBuy services following SRP - optimized for performance
        // Use Scoped lifetime for services that maintain state within a request
        context.Services.AddScoped<IGroupBuyPricingService, GroupBuyPricingService>();
        context.Services.AddScoped<IGroupBuyReportingService, GroupBuyReportingService>();
        context.Services.AddScoped<IGroupBuyImageService, GroupBuyImageService>();

        // Register PaymentStrategy services following Strategy Pattern - optimized for performance
        // Use Singleton lifetime for stateless factories to reduce allocation overhead
        context.Services.AddSingleton<IPaymentStrategyFactory, PaymentStrategyFactory>();

        // Register ShippingStrategy services following Strategy Pattern - optimized for performance
        // Use Singleton lifetime for stateless factories to reduce allocation overhead
        context.Services.AddSingleton<IShippingStrategyFactory, ShippingStrategyFactory>();

        // Register performance optimization services
        context.Services.AddSingleton<IServiceCachingService, ServiceCachingService>();
        context.Services.AddSingleton<IPerformanceMetrics, PerformanceMetrics>();
        context.Services.AddSingleton<ILazyServiceFactory, LazyServiceFactory>();
    }
}