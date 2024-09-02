using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Kooco.Pikachu.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class PikachuDbContextFactory : IDesignTimeDbContextFactory<PikachuDbContext>
{
    public PikachuDbContext CreateDbContext(string[] args)
    {
        PikachuEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<PikachuDbContext>()
            .UseSqlServer(
                configuration.GetConnectionString("Default"),
                sqlServerOptionsAction =>
                {
                    sqlServerOptionsAction.EnableRetryOnFailure();
                    sqlServerOptionsAction.CommandTimeout(120);
                }
            );

        return new PikachuDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Kooco.Pikachu.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
