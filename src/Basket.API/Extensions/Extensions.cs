using Basket.API.Infrastructure;
using Basket.API.Infrastructure.Repositories;
using Basket.API.Infrastructure.Services;
using Basket.API.Models.Options;
using Basket.API.Services;
using Catalog.API.Protos;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using Shared.Services;
using StackExchange.Redis;

namespace Basket.API.Extensions
{
    public static class Extensions
    {
        public static void AddServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IBasketService, BasketService>();
            builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();
            builder.Services.AddScoped<ICatalogClientService, CatalogClientService>();
        }

        public static void AddPersistence(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Basket"));
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Cache") 
                    ?? throw new InvalidOperationException("Connection string 'Cache' is not configured.");
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Cache")!) ??
                    throw new InvalidOperationException("Connection string 'Cache' is not configured."));
        }

        public static void AddGrpcServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddGrpcClient<CatalogService.CatalogServiceClient>(options =>
            {
                options.Address = new Uri(builder.Configuration["ServicesDiscovery:Grpc:CatalogApi"] ?? 
                    throw new InvalidOperationException("Address string 'CatalogApi' is not configured."));
            });
        }

        public static void AddIdentity(this IHostApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            builder.AddDefaultAuthentication();

            builder.Services.AddScoped<IIdentityProvider, IdentityProvider>();
        }
    }
}
