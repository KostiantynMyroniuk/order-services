using Catalog.API.Infrastructure;
using Catalog.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Catalog.API.Extensions
{
    public static class ProductSeeder
    {
        public static async Task SeedProducts(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var filePath = Path.Combine(environment.ContentRootPath, "Data", "catalog.json");
            var json = File.ReadAllText(filePath);

            var products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (products != null && !context.Products.Any())
            {
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }

        public static async Task MigrateDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
            await context.Database.MigrateAsync();
        }
    }
}
