﻿using Microsoft.Extensions.DependencyInjection;
using OfficeOpenXml;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.SignalR;
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
    typeof(ECPayModule),
    typeof(AbpAspNetCoreSignalRModule)
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

      
    }
}