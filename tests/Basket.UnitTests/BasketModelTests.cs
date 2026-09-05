using Basket.API.Models;

namespace Basket.UnitTests;

public class BasketModelTests
{
    [Fact]
    public void AddItem_WhenProductAlreadyExists_IncreasesQuantity()
    {
        var basket = new BasketModel("user-1");
        var productId = Guid.NewGuid();

        basket.AddItem(BasketItem.Create(productId, "Coffee", 12.5m, 1));
        basket.AddItem(BasketItem.Create(productId, "Coffee", 12.5m, 2));

        Assert.Single(basket.Items);
        Assert.Equal(3, basket.Items[0].Quantity);
    }

    [Fact]
    public void DeleteItem_WhenProductExists_RemovesIt()
    {
        var basket = new BasketModel("user-1");
        var productId = Guid.NewGuid();

        basket.AddItem(BasketItem.Create(productId, "Coffee", 12.5m, 1));

        basket.DeleteItem(productId);

        Assert.Empty(basket.Items);
    }
}
