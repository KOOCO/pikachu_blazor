using AutoMapper.Internal.Mappers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;

namespace Kooco.Pikachu;

[DependsOn(
    typeof(PikachuApplicationModule),
    typeof(PikachuDomainTestModule)
    )]
public class PikachuApplicationTestModule : AbpModule
{

}
