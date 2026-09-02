using Catalog.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace Catalog.API.Extensions
{
    public static class Extension
    {
        public static void AddPersistence(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<CatalogDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));
        }

        public static void AddGrpcServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddGrpc();
        }
    }
}
