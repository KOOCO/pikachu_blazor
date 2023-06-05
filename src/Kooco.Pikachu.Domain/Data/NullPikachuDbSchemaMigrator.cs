using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.Data;

/* This is used if database provider does't define
 * IPikachuDbSchemaMigrator implementation.
 */
public class NullPikachuDbSchemaMigrator : IPikachuDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
