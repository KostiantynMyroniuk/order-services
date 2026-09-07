using Catalog.API.Protos;
using Orders.API.Models;
using System.Globalization;

namespace Orders.API.Infrastructure.Services
{
    public sealed record CatalogItem(Guid ProductId, string ProductName, decimal Price);
    public class CatalogGrpcClientService(
        CatalogService.CatalogServiceClient catalogServiceClient) : ICatalogClientService
    {
        public async Task<IEnumerable<CatalogItem>> GetProductsByIds(IEnumerable<Guid> productIds, CancellationToken ct = default)
        {
            var request = new GetProductsByIdsRequest();

            request.ProductIds.AddRange(productIds.Select(i => i.ToString()));

            var products = await catalogServiceClient.GetProductsAsync(request, cancellationToken: ct);

            return products.Products
                .Select(p => new CatalogItem(
                    Guid.Parse(p.ProductId),
                    p.Name,
                    decimal.Parse(p.Price, CultureInfo.InvariantCulture)));
        }
    }
}
