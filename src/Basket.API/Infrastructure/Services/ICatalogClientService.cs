using Basket.API.Models;

namespace Basket.API.Infrastructure.Services
{
    public interface ICatalogClientService
    {
        Task<ProductSnapshot?> GetProductAsync(Guid productId, CancellationToken ct); 
    }
}
