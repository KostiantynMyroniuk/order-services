using Orders.API.Models;

namespace Orders.API.Infrastructure.Services
{
    public interface ICatalogClientService
    {
        Task<IEnumerable<CatalogItem>> GetProductsByIds(IEnumerable<Guid> productIds, CancellationToken ct = default);
    }
}
