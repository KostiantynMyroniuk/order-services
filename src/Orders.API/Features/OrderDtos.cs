using Orders.API.Models;

namespace Orders.API.Features
{
    public record GetMyOrdersResponse(
        Guid OrderId,
        DateTime CreatedAt,
        OrderStatus Status,
        Address Address);

    public record GetOrderResponse(
    Guid OrderId,
    OrderStatus Status,
    Address Address,
    DateTime CreatedAt,
    List<OrderItemResponse> OrderItems);

    public record OrderItemResponse(
        Guid ProductId,
        string ProductName,
        decimal UnitPrice,
        int Quantity,
        decimal LineTotal);
}
