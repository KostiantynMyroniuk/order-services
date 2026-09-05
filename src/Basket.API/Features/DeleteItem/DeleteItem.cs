using Basket.API.Infrastructure.Repositories;
using Basket.API.Models;
using MediatR;
using Shared.Models;

namespace Basket.API.Features.DeleteItem
{
    public sealed record DeleteItemRequest(
        string UserId,
        Guid ProductId) : IRequest<Result<BasketModel>>;

    public class DeleteItemRequestHandler(
        IBasketRepository basketRepository,
        ILogger<DeleteItemRequestHandler> logger) : IRequestHandler<DeleteItemRequest, Result<BasketModel>>
    {
        public async Task<Result<BasketModel>> Handle(DeleteItemRequest request, CancellationToken cancellationToken)
        {
            var existingBasket = await basketRepository.GetBasketAsync(request.UserId, cancellationToken);
            var basket = existingBasket ?? new BasketModel(request.UserId);

            basket.DeleteItem(request.ProductId);

            await basketRepository.SaveBasketAsync(basket, cancellationToken);

            logger.LogInformation("User {UserId} has deleted product {ProductId} from his basket", request.UserId, request.ProductId);

            return Result<BasketModel>.Success(basket);
        }
    }
}
