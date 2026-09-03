using Basket.API.Models;
using Catalog.API.Protos;
using Grpc.Core;
using StackExchange.Redis;
using System.Text.Json;

namespace Basket.API.Infrastructure.Services
{
    public class CatalogClientService(
        IConnectionMultiplexer multiplexer,
        CatalogService.CatalogServiceClient catalogClient) : ICatalogClientService
    {
        private readonly IDatabase _db = multiplexer.GetDatabase();

        public async Task<ProductSnapshot?> GetProductAsync(Guid productId, CancellationToken ct)
        {
            var key = $"product:{productId}";

            var productCached = await _db.StringGetAsync(key);

            if (productCached.HasValue)
                return JsonSerializer.Deserialize<ProductSnapshot>(productCached.ToString());

            try
            {
                var productResponse = await catalogClient.GetProductAsync(
                    new GetProductRequest { ProductId = productId.ToString() },
                    cancellationToken: ct);

                var product = new ProductSnapshot(
                    Guid.Parse(productResponse.ProductId),
                    productResponse.Name,
                    productResponse.Description,
                    decimal.Parse(productResponse.Price));

                await _db.StringSetAsync(key, JsonSerializer.Serialize(product));

                return product;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
            
        }
    }
}
