using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.API.Models;

namespace Orders.API.Infrastructure.Configurations
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(i => i.ProductId);

            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2);
        }
    }
}
