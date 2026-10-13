using Basket.API.Features.GetBasket;
using Basket.API.Infrastructure.Repositories;
using Basket.API.Models;
using Moq;

namespace Basket.UnitTests;

public class GetBasketQueryHandlerTests
{
    private readonly Mock<IBasketRepository> _basketRepoMock;

    public GetBasketQueryHandlerTests()
    {
        _basketRepoMock = new Mock<IBasketRepository>();
    }

    private GetBasketQueryHandler CreateGetBasketQueryHandler() => new GetBasketQueryHandler(_basketRepoMock.Object);

    [Fact]
    public async Task Handle_WhenBasketIsMissing_ReturnsEmptyBasketForUser()
    {
        _basketRepoMock
            .Setup(x => x.GetBasketAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((BasketModel?)null);

        var handler = CreateGetBasketQueryHandler();

        var result = await handler.Handle(new GetBasketQuery("user-1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("user-1", result.Value!.UserId);
        Assert.Empty(result.Value.Items);
    }
}
