namespace Orders.API.Models
{
    public class Order
    {
        public Guid Id { get; private set; }
        public Guid RequestId { get; private set; }
        public string UserId { get; private set; } = null!;

        public Address Address { get; private set; } = default!;

        public OrderStatus Status { get; private set; }

        public DateTime CreatedAtUtc { get; private set;  }

        private List<OrderItem> _orderItems = [];
        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

        private Order() { }

        public Order(Guid requestId, string userId, Address address)
        {
            Id = Guid.CreateVersion7();
            RequestId = requestId;
            UserId = userId;
            Address = address;
            Status = OrderStatus.Submitted;
            CreatedAtUtc = DateTime.UtcNow;
        }

        public void AddOrderItem(Guid productId, string productName, decimal unitPrice, int quantity = 1)
        {
            var orderItem = new OrderItem(productId, productName, unitPrice, quantity);
            _orderItems.Add(orderItem);
        }

        public void SetStatusToShipped()
        {
            if (Status != OrderStatus.Paid)
                throw new InvalidOperationException(); // set to custom exception

            Status = OrderStatus.Shipped;
        }
    }

    public record Address(
        string City, 
        string Street, 
        string Country, 
        string ZipCode);

    public enum OrderStatus
    {
        Submitted = 1, 
        Paid = 2, 
        Shipped = 3, 
        Cancelled = 4
    }
}
