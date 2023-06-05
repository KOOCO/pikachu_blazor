using Kooco.Pikachu.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Kooco.Pikachu;

[DependsOn(
    typeof(PikachuEntityFrameworkCoreTestModule)
    )]
public class PikachuDomainTestModule : AbpModule
{

}
