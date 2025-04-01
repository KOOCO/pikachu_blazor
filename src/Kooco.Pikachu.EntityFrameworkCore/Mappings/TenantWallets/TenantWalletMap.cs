using Kooco.Pikachu.TenantWallets.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Kooco.Pikachu.Mappings.TenantWallets;
public class TenantWalletMap : IEntityTypeConfiguration<TenantWallet>
{
    public void Configure(EntityTypeBuilder<TenantWallet> builder)
    {
        builder.ToTable(typeof(TenantWallet).ToDatabaseName());
        builder.ConfigureByConvention();

        builder.Property(x => x.TenantId);

        builder.Property(x => x.WalletBalance)
            .IsRequired();

        builder.HasIndex(x => x.TenantId)
            .IsUnique();
    }
}