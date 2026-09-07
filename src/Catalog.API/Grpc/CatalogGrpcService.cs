using Catalog.API.Infrastructure;
using Catalog.API.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Catalog.API.Grpc
{
    public class CatalogGrpcService(
        CatalogDbContext dbContext,
        ILogger<CatalogGrpcService> logger) : CatalogService.CatalogServiceBase
    {
        public override async Task<ProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.ProductId));

            if (product is null)
            {
                logger.LogWarning("Product {ProductId} is missing in catalog", request.ProductId);

                throw new RpcException(
                    new Status(StatusCode.NotFound,
                    $"Product {request.ProductId} not found"));
            }

            return new ProductResponse
            {
                ProductId = request.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.ToString()
            };
        }

        public override async Task<GetProductsByIdsResponse> GetProducts(GetProductsByIdsRequest request, ServerCallContext context)
        {
            var productIds = request.ProductIds.Distinct().Select(Guid.Parse).ToList();
                
            var products = await dbContext.Products
                .AsNoTracking()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(context.CancellationToken);

            var foundProductIds = products.Select(p => p.Id);
            var missingProductIds = productIds.Except(foundProductIds).ToList();

            if (missingProductIds.Any())
            {
                logger.LogWarning("Products {MissingProducts} are missing in catalog", missingProductIds);

                throw new RpcException(
                    new Status(StatusCode.NotFound,
                    $"Products missing: {string.Join(", ", missingProductIds)}"));
            }

            var response = new GetProductsByIdsResponse();

            var catalogProducts = products.Select(p => new CatalogProduct
            {
                ProductId = p.Id.ToString(),
                Name = p.Name,
                Description = p.Description,
                Price = p.Price.ToString(CultureInfo.InvariantCulture)
            });

            response.Products.AddRange(catalogProducts);

            return response;
        }
    }
}
