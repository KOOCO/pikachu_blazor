using Kooco.Pikachu.Tenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Kooco.Pikachu.Mappings.Tenants;
public sealed class TenantTripartiteMap : IEntityTypeConfiguration<TenantTripartite>
{
    public void Configure(EntityTypeBuilder<TenantTripartite> builder)
    {
        builder.ToTable(typeof(TenantTripartite).ToDatabaseName());
        builder.ConfigureByConvention();

        builder.HasIndex(x => x.TenantId)
            .IsUnique();
    }
}