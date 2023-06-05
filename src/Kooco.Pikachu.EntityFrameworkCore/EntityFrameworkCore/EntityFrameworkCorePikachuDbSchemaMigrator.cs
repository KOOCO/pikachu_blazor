using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Kooco.Pikachu.Data;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.EntityFrameworkCore;

public class EntityFrameworkCorePikachuDbSchemaMigrator
    : IPikachuDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCorePikachuDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the PikachuDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<PikachuDbContext>()
            .Database
            .MigrateAsync();
    }
}
