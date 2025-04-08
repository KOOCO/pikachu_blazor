using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Kooco.Pikachu.Data;
using Volo.Abp.DependencyInjection;

namespace Kooco.Pikachu.EntityFrameworkCore;
public class EntityFrameworkCorePikachuDbSchemaMigrator(IServiceProvider serviceProvider) : IPikachuDbSchemaMigrator, ITransientDependency
{
    public async Task MigrateAsync()
    {
        await serviceProvider
            .GetRequiredService<PikachuDbContext>()
            .Database
            .MigrateAsync();
    }
}