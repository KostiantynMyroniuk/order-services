using Basket.API.Features.DeleteItem;
using Basket.API.Infrastructure.Repositories;
using Basket.API.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Basket.UnitTests;

public class DeleteItemRequestHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductExists_DeletesItemAndSavesBasket()
    {
        var basketRepository = new Mock<IBasketRepository>();
        var logger = new Mock<ILogger<DeleteItemRequestHandler>>();

        var productId = Guid.NewGuid();
        var basket = new BasketModel("user-1");
        basket.AddItem(BasketItem.Create(productId, "Coffee", 12.5m, 1));

        basketRepository
            .Setup(x => x.GetBasketAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(basket);
        basketRepository
            .Setup(x => x.SaveBasketAsync(It.IsAny<BasketModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BasketModel savedBasket, CancellationToken _) => savedBasket);

        var handler = new DeleteItemRequestHandler(basketRepository.Object, logger.Object);

        var result = await handler.Handle(new DeleteItemRequest("user-1", productId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!.Items);
        basketRepository.Verify(x => x.SaveBasketAsync(It.Is<BasketModel>(b => b.Items.Count == 0), It.IsAny<CancellationToken>()), Times.Once);
    }
}
