using Basket.API.Infrastructure.Repositories;
using Basket.API.Infrastructure.Services;
using Basket.API.Models;
using Shared.Models;

namespace Basket.API.Services
{
    public interface IBasketService
    {
        Task<Result<BasketModel>> AddItemAsync(string userId, Guid productId, int quantity, CancellationToken ct = default);
    }

    public class BasketService(
        IBasketRepository basketRepository,
        ICatalogClientService catalogClient,
        ILogger<BasketService> logger) : IBasketService
    {
        public async Task<Result<BasketModel>> AddItemAsync(string userId, Guid productId, int quantity, CancellationToken ct = default)
        {
            var product = await catalogClient.GetProductAsync(productId, ct);

            if (product is null)
                return Result<BasketModel>.Fail(StatusCodes.Status404NotFound, $"Product '{productId}' not found");

            var basket = await basketRepository.GetBasketAsync(userId, ct);

            if (basket is null)
                basket = new BasketModel(userId);

            var basketItem = new BasketItem()
            {
                ProductId = productId,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            };

            basket.AddItem(basketItem);

            await basketRepository.SaveBasketAsync(basket, ct);

            return Result<BasketModel>.Success(basket);
        }
    }
}
