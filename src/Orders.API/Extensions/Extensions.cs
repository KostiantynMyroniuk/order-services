using Catalog.API.Protos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure;
using Orders.API.Infrastructure.ExceptionHandlers;
using Orders.API.Infrastructure.Services;
using Shared.Extensions;
using Shared.Services;

namespace Orders.API.Extensions
{
    public static class Extensions
    {
        public static void AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddValidatorsFromAssembly(typeof(Extensions).Assembly);

            builder.Services.AddMediatR(options =>
            {
                options.RegisterServicesFromAssembly(typeof(Extensions).Assembly);
            });
        }

        public static void AddPersistenceConfigurations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));
        }

        public static void AddGrpcServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddGrpcClient<CatalogService.CatalogServiceClient>(options =>
            {
                options.Address = new Uri(builder.Configuration["ServicesDiscovery:Grpc:CatalogApi"]
                    ?? throw new InvalidOperationException("Address string 'CatalogApi' is not configured."));
            });

            builder.Services.AddScoped<ICatalogClientService, CatalogGrpcClientService>();
        }

        public static void AddIdentityConfigurations(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            builder.AddDefaultAuthentication();

            builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
        }
    }
}
