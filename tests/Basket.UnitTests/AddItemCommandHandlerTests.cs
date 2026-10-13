using Basket.API.Features.AddItem;
using Basket.API.Infrastructure.Repositories;
using Basket.API.Infrastructure.Services;
using Basket.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Basket.UnitTests;

public class AddItemCommandHandlerTests
{
    private readonly Mock<IBasketRepository> _basketRepoMock;
    private readonly Mock<ICatalogClientService> _catalogServiceMock;
    private readonly Mock<ILogger<AddItemCommandHandler>> _loggerMock;

    public AddItemCommandHandlerTests()
    {
        _basketRepoMock = new Mock<IBasketRepository>();
        _catalogServiceMock = new Mock<ICatalogClientService>();
        _loggerMock = new Mock<ILogger<AddItemCommandHandler>>();
    }

    private AddItemCommandHandler CreateAddItemCommandHandler() => new AddItemCommandHandler(_basketRepoMock.Object, _catalogServiceMock.Object, _loggerMock.Object);
    
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsFailure()
    {
        var productId = Guid.NewGuid();
        _catalogServiceMock
            .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductSnapshot?)null);

        var handler = CreateAddItemCommandHandler();

        var result = await handler.Handle(new AddItemCommand("user-1", productId, 2), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal($"Product '{productId}' not found", result.ErrorMessage);
        _basketRepoMock.Verify(x => x.GetBasketAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _basketRepoMock.Verify(x => x.SaveBasketAsync(It.IsAny<BasketModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductExists_SavesBasketWithItem()
    {
        var productId = Guid.NewGuid();
        var basket = new BasketModel("user-1");

        _catalogServiceMock
            .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductSnapshot(productId, "Coffee", "Fresh coffee", 12.5m));
        _basketRepoMock
            .Setup(x => x.GetBasketAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);
        _basketRepoMock
            .Setup(x => x.SaveBasketAsync(It.IsAny<BasketModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BasketModel savedBasket, CancellationToken _) => savedBasket);

        var handler = CreateAddItemCommandHandler();

        var result = await handler.Handle(new AddItemCommand("user-1", productId, 2), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(productId, result.Value.Items[0].ProductId);
        Assert.Equal(2, result.Value.Items[0].Quantity);
        _basketRepoMock.Verify(x => x.SaveBasketAsync(It.Is<BasketModel>(b => b.Items.Count == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
