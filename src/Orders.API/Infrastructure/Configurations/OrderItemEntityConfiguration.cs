using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.API.Models;

namespace Orders.API.Infrastructure.Configurations
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasIndex(i => new { i.OrderId, i.ProductId })
                .IsUnique();

            builder.Property(i => i.ProductName)
                .HasMaxLength(200);

            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}
