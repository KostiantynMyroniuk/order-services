namespace Basket.API.Models
{
    public sealed record BasketItem
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = default!;
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }

        public decimal LineTotal => UnitPrice * Quantity;

        public static BasketItem Create(Guid productId, string name, decimal price, int quantity)
        {
            return new BasketItem
            {
                ProductId = productId,
                ProductName = name,
                UnitPrice = price,
                Quantity = quantity
            };
        }
    }
}