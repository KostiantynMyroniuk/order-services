using Basket.API.Models;

namespace Basket.API.Infrastructure.Repositories
{
    public interface IBasketRepository
    {
        Task<BasketModel?> GetBasketAsync(string userId, CancellationToken ct = default);
        Task<BasketModel> SaveBasketAsync(BasketModel basket, CancellationToken ct = default);
    }
}
