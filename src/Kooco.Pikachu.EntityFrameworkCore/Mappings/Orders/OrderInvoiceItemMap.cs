using Kooco.Pikachu.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Kooco.Pikachu.Mappings.Orders;
public sealed class OrderInvoiceItemMap : IEntityTypeConfiguration<OrderInvoiceItem>
{
    public void Configure(EntityTypeBuilder<OrderInvoiceItem> builder)
    {
        builder.ToTable(typeof(OrderInvoiceItem).ToDatabaseName());
        builder.ConfigureByConvention();

        builder.Property(x => x.ProductName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ProductQty)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .IsRequired();

        builder.HasOne(x => x.OrderInvoice)
            .WithMany(o => o.OrderInvoiceItems)
            .HasForeignKey(x => x.OrderInvoiceId);
    }
}