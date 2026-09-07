namespace Orders.API.Models
{
    public class OrderItem
    {
        public Guid ProductId { get; private set; }
        public Guid OrderId { get; set; }
        public string ProductName { get; private set; } = default!;
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }
        public decimal LineTotal => Quantity * UnitPrice;

        private OrderItem() { }

        public OrderItem(
            Guid productId,
            Guid orderId,
            string productName,
            decimal unitPrice,
            int quantity)
        {
            ProductId = productId;
            OrderId = orderId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}
