using Basket.API.Infrastructure.Repositories;
using Basket.API.Infrastructure.Services;
using Basket.API.Models;
using MediatR;
using Shared.Models;

namespace Basket.API.Features.AddItem
{
    public sealed record AddItemCommand(
        string UserId,
        Guid ProductId,
        int Quantity) : IRequest<Result<BasketModel>>;

    public class AddItemCommandHandler(
        IBasketRepository basketRepository,
        ICatalogClientService catalogClient,
        ILogger<AddItemCommandHandler> logger) : IRequestHandler<AddItemCommand, Result<BasketModel>>
    {
        public async Task<Result<BasketModel>> Handle(AddItemCommand request, CancellationToken cancellationToken)
        {
            var product = await catalogClient.GetProductAsync(request.ProductId, cancellationToken);

            if (product is null)
            {
                logger.LogWarning("Product {ProductId} not found while adding to basket for user {UserId}", request.ProductId, request.UserId);
                return Result<BasketModel>.Fail(StatusCodes.Status404NotFound, $"Product '{request.ProductId}' not found");
            }

            var existingBasket = await basketRepository.GetBasketAsync(request.UserId, cancellationToken);
            var basket = existingBasket ?? new BasketModel(request.UserId);

            basket.AddItem(new BasketItem()
            {
                ProductId = request.ProductId,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = request.Quantity
            });

            await basketRepository.SaveBasketAsync(basket, cancellationToken);

            return Result<BasketModel>.Success(basket);
        }
    }
}
