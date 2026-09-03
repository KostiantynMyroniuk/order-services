using Basket.API.Infrastructure.Repositories;
using Basket.API.Infrastructure.Services;
using Basket.API.Models;
using Shared.Models;

namespace Basket.API.Services
{
    public interface IBasketService
    {
        Task<Result<BasketModel>> AddItemAsync(string userId, Guid productId, int quantity, CancellationToken ct = default);
        Task<Result<BasketModel>> GetUserBasketAsync(string userId, CancellationToken ct = default);
        Task<Result<BasketModel>> DeleteItemAsync(string userId, Guid productId, CancellationToken ct = default);
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
            {
                logger.LogWarning("Product {ProductId} not found while adding to basket for user {UserId}", productId, userId);
                return Result<BasketModel>.Fail(StatusCodes.Status404NotFound, $"Product '{productId}' not found");
            }

            var existingBasket = await basketRepository.GetBasketAsync(userId, ct);
            var basket = existingBasket ?? new BasketModel(userId);

            basket.AddItem(new BasketItem()
            {
                ProductId = productId,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = quantity
            });

            await basketRepository.SaveBasketAsync(basket, ct);

            return Result<BasketModel>.Success(basket);
        }

        public async Task<Result<BasketModel>> GetUserBasketAsync(string userId, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(userId, ct);

            return Result<BasketModel>.Success(basket ?? new BasketModel(userId));
        }

        public async Task<Result<BasketModel>> DeleteItemAsync(string userId, Guid productId, CancellationToken ct = default)
        {
            var existingBasket = await basketRepository.GetBasketAsync(userId, ct);
            var basket = existingBasket ?? new BasketModel(userId);

            basket.DeleteItem(productId);

            await basketRepository.SaveBasketAsync(basket, ct);

            return Result<BasketModel>.Success(basket);
        }
    }
}
