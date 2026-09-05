using System.Text.Json.Serialization;

namespace Basket.API.Models
{
    public class BasketModel
    {
        [JsonInclude]
        public string UserId { get; private set; } = default!;

        [JsonInclude]
        [JsonPropertyName("items")]
        private List<BasketItem> _items = [];
        public IReadOnlyList<BasketItem> Items => _items.AsReadOnly();

        public BasketModel() { }

        public BasketModel(string userId)
        {
            UserId = userId;
        }

        public void AddItem(BasketItem item)
        {
            var index = _items.FindIndex(i => i.ProductId == item.ProductId);
            _items = index >= 0 
                ? _items.Select((i, n) => n == index ? i with { Quantity = i.Quantity + item.Quantity } : i).ToList() 
                : [.. _items, item];
        }

        public void DeleteItem(Guid productId)
        {
            var index = _items.FindIndex(i => i.ProductId == productId);

            if (index < 0) return;

            _items.RemoveAt(index);
        }
    }

}
