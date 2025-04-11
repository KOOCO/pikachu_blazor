using Kooco.Pikachu.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Kooco.Pikachu.Mappings.Orders;
public sealed class OrderInvoiceMap : IEntityTypeConfiguration<OrderInvoice>
{
    public void Configure(EntityTypeBuilder<OrderInvoice> builder)
    {
        builder.ToTable(typeof(OrderInvoice).ToDatabaseName());
        builder.ConfigureByConvention();

        builder.Property(x => x.InvoiceNo)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.UnifiedBusinessNo)
            .HasMaxLength(8);

        builder.Property(x => x.InvoiceType)
            .IsRequired();

        builder.Property(x => x.IssuanceMethod)
            .IsRequired();

        builder.Property(x => x.SubtotalAmount)
            .IsRequired();

        builder.Property(x => x.ShippingCost)
            .IsRequired();

        builder.Property(x => x.TaxAmount)
            .IsRequired();

        builder.Property(x => x.TaxType)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .IsRequired();

        builder.Property(x => x.SerialNo)
            .IsRequired();

        builder.Property(x => x.RelateNo)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.IsVoided)
            .IsRequired();

        builder.HasIndex(x => new { x.SerialNo, x.RelateNo });

        builder.HasOne(x => x.Order)
            .WithMany(o => o.OrderInvoices)
            .HasForeignKey(x => x.OrderId);
    }
}