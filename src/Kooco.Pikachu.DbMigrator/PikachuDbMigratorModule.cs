using Kooco.Pikachu.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Kooco.Pikachu.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(PikachuEntityFrameworkCoreModule),
    typeof(PikachuApplicationContractsModule)
    )]
public class PikachuDbMigratorModule : AbpModule
{

}
