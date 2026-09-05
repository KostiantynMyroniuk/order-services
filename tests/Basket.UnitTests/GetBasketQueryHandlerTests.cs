using Basket.API.Features.GetBasket;
using Basket.API.Infrastructure.Repositories;
using Basket.API.Models;
using Moq;

namespace Basket.UnitTests;

public class GetBasketQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenBasketIsMissing_ReturnsEmptyBasketForUser()
    {
        var basketRepository = new Mock<IBasketRepository>();
        basketRepository
            .Setup(x => x.GetBasketAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((BasketModel?)null);

        var handler = new GetBasketQueryHandler(basketRepository.Object);

        var result = await handler.Handle(new GetBasketQuery("user-1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("user-1", result.Value!.UserId);
        Assert.Empty(result.Value.Items);
    }
}
