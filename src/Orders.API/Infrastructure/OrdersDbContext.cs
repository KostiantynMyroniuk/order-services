using Microsoft.EntityFrameworkCore;
using Orders.API.Models;

namespace Orders.API.Infrastructure
{
    public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("orders");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        }
    }
}
