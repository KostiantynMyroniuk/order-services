using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure;
using Shared.Extensions;
using Shared.Services;

namespace Orders.API.Extensions
{
    public static class Extensions
    {
        public static void AddPersistenceConfigurations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));
        }

        public static void AddIdentityConfigurations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            builder.AddDefaultAuthentication();

            builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
        }
    }
}
