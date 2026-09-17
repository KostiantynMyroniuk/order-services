using MediatR;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure;
using Orders.API.Models;
using Shared.Models;

namespace Orders.API.Features.GetOrder
{
    public record GetOrderQuery(
        Guid OrderId,
        string UserId) : IRequest<Result<GetOrderResponse>>;

    public class GetOrderQueryHandler(
        OrdersDbContext context,
        ILogger<GetOrderQueryHandler> logger) : IRequestHandler<GetOrderQuery, Result<GetOrderResponse>>
    {
        public async Task<Result<GetOrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await context.Orders
                .AsNoTracking()
                .Where(o => o.Id == request.OrderId && o.UserId == request.UserId)
                .Select(o => new GetOrderResponse(
                    o.Id,
                    o.Status,
                    o.Address,
                    o.CreatedAtUtc,
                    o.OrderItems
                        .Select(i => new OrderItemResponse(
                            i.ProductId, 
                            i.ProductName, 
                            i.UnitPrice, 
                            i.Quantity, 
                            i.LineTotal)).ToList()))
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
            {
                logger.LogWarning("Order {OrderId} was not found for User {UserId}.", request.OrderId, request.UserId);
                return Result<GetOrderResponse>.Fail(StatusCodes.Status404NotFound, $"Order: {request.OrderId} not found");
            }

            return Result<GetOrderResponse>.Success(order);
        }
    }
}
