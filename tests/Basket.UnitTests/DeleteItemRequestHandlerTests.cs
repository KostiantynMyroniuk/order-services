using Basket.API.Features.AddItem;
using Basket.API.Features.DeleteItem;
using Basket.API.Infrastructure.Repositories;
using Basket.API.Infrastructure.Services;
using Basket.API.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Basket.UnitTests;

public class DeleteItemRequestHandlerTests
{
    private readonly Mock<IBasketRepository> _basketRepoMock;
    private readonly Mock<ILogger<DeleteItemCommandHandler>> _loggerMock;

    public DeleteItemRequestHandlerTests()
    {
        _basketRepoMock = new Mock<IBasketRepository>();
        _loggerMock = new Mock<ILogger<DeleteItemCommandHandler>>();
    }

    private DeleteItemCommandHandler CreateDeleteItemCommandHandler() => new DeleteItemCommandHandler(_basketRepoMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_WhenProductExists_DeletesItemAndSavesBasket()
    {
        var productId = Guid.NewGuid();
        var basket = new BasketModel("user-1");
        basket.AddItem(BasketItem.Create(productId, "Coffee", 12.5m, 1));

        _basketRepoMock
            .Setup(x => x.GetBasketAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);
        _basketRepoMock
            .Setup(x => x.SaveBasketAsync(It.IsAny<BasketModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BasketModel savedBasket, CancellationToken _) => savedBasket);

        var handler = CreateDeleteItemCommandHandler();

        var result = await handler.Handle(new DeleteItemCommand("user-1", productId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        _basketRepoMock.Verify(x => x.SaveBasketAsync(It.Is<BasketModel>(b => b.Items.Count == 0), It.IsAny<CancellationToken>()), Times.Once);
    }
}
