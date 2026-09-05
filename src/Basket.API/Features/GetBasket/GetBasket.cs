using Basket.API.Infrastructure.Repositories;
using Basket.API.Models;
using MediatR;
using Shared.Models;

namespace Basket.API.Features.GetBasket
{
    public sealed record GetBasketQuery(string UserId) : IRequest<Result<BasketModel>>;

    public class GetBasketQueryHandler(
        IBasketRepository basketRepository) : IRequestHandler<GetBasketQuery, Result<BasketModel>>
    {
        public async Task<Result<BasketModel>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await basketRepository.GetBasketAsync(request.UserId, cancellationToken);

            return Result<BasketModel>.Success(basket ?? new BasketModel(request.UserId));
        }
    }
}
