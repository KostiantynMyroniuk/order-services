using Catalog.API.Infrastructure;
using Catalog.API.Protos;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Grpc
{
    public class CatalogGrpcService(CatalogDbContext dbContext) : CatalogService.CatalogServiceBase
    {
        public override async Task<ProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(request.ProductId));

            if (product is null)
                throw new RpcException(
                    new Status(StatusCode.NotFound, 
                    $"Product {request.ProductId} not found"));

            return new ProductResponse
            {
                ProductId = request.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price.ToString()
            };
        }
    }
}
