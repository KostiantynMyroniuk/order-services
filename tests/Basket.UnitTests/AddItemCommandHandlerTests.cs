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
    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsFailure()
    {
        var basketRepository = new Mock<IBasketRepository>();
        var catalogClient = new Mock<ICatalogClientService>();
        var logger = new Mock<ILogger<AddItemCommandHandler>>();

        var productId = Guid.NewGuid();
        catalogClient
            .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductSnapshot?)null);

        var handler = new AddItemCommandHandler(basketRepository.Object, catalogClient.Object, logger.Object);

        var result = await handler.Handle(new AddItemCommand("user-1", productId, 2), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal($"Product '{productId}' not found", result.ErrorMessage);
        basketRepository.Verify(x => x.GetBasketAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        basketRepository.Verify(x => x.SaveBasketAsync(It.IsAny<BasketModel>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenProductExists_SavesBasketWithItem()
    {
        var basketRepository = new Mock<IBasketRepository>();
        var catalogClient = new Mock<ICatalogClientService>();
        var logger = new Mock<ILogger<AddItemCommandHandler>>();

        var productId = Guid.NewGuid();
        var basket = new BasketModel("user-1");

        catalogClient
            .Setup(x => x.GetProductAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProductSnapshot(productId, "Coffee", "Fresh coffee", 12.5m));
        basketRepository
            .Setup(x => x.GetBasketAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);
        basketRepository
            .Setup(x => x.SaveBasketAsync(It.IsAny<BasketModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BasketModel savedBasket, CancellationToken _) => savedBasket);

        var handler = new AddItemCommandHandler(basketRepository.Object, catalogClient.Object, logger.Object);

        var result = await handler.Handle(new AddItemCommand("user-1", productId, 2), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.Equal(productId, result.Value.Items[0].ProductId);
        Assert.Equal(2, result.Value.Items[0].Quantity);
        basketRepository.Verify(x => x.SaveBasketAsync(It.Is<BasketModel>(b => b.Items.Count == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
