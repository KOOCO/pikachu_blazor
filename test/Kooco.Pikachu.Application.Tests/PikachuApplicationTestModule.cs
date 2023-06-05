using Volo.Abp.Modularity;

namespace Kooco.Pikachu;

[DependsOn(
    typeof(PikachuApplicationModule),
    typeof(PikachuDomainTestModule)
    )]
public class PikachuApplicationTestModule : AbpModule
{

}
