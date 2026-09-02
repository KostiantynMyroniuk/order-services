namespace Basket.API.Models
{
    public class BasketModel
    {
        public string UserId { get; private set; } = default!;
        public List<BasketItem> Items { get; private set; } = [];

        public BasketModel() { }

        public BasketModel(string userId)
        {
            UserId = userId;
        }

        public void AddItem(BasketItem item)
        {
            var index = Items.FindIndex(i => i.ProductId == item.ProductId);
            Items = index >= 0 
                ? Items.Select((i, n) => n == index ? i with { Quantity = i.Quantity + item.Quantity } : i).ToList() 
                : [.. Items, item];
        }
    }

}
