using Basket.API.Infrastructure.Repositories;
using Basket.API.Models;
using MediatR;
using Shared.Models;

namespace Basket.API.Features.DeleteItem
{
    public record DeleteItemCommand(
        string UserId,
        Guid ProductId) : IRequest<Result<BasketModel>>;

    public class DeleteItemCommandHandler(
        IBasketRepository basketRepository,
        ILogger<DeleteItemCommandHandler> logger) : IRequestHandler<DeleteItemCommand, Result<BasketModel>>
    {
        public async Task<Result<BasketModel>> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
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
