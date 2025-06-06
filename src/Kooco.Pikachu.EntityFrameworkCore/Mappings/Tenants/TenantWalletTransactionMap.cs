﻿using Kooco.Pikachu.TenantManagement;
using Kooco.Pikachu.Tenants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Kooco.Pikachu.Mappings.Tenants;
public sealed class TenantWalletTransactionMap : IEntityTypeConfiguration<TenantWalletTransaction>
{
    public void Configure(EntityTypeBuilder<TenantWalletTransaction> builder)
    {
        builder.ToTable(typeof(TenantWalletTransaction).ToDatabaseName());
        builder.ConfigureByConvention();

        builder.Property(x => x.DeductionStatus)
            .IsRequired();

        builder.Property(x => x.TradingMethods)
            .IsRequired();

        builder.Property(x => x.TransactionType)
            .IsRequired();

        builder.Property(x => x.TransactionAmount)
            .IsRequired();

        builder.Property(x => x.TransactionNotes)
            .HasMaxLength(TenantWalletConsts.MaxTransactionRemarkLength);

        builder.HasOne(x => x.TenantWallet)
            .WithMany(tw => tw.TenantWalletTransactions)
            .HasForeignKey(x => x.TenantWalletId);
    }
}