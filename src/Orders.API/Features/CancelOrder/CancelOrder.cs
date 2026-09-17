using MediatR;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure;
using Shared.Models;

namespace Orders.API.Features.CancelOrder
{
    public record CancelOrderCommand(
        Guid OrderId,
        string UserId) : IRequest<Result>;

    public class CancelOrderCommandHandler(
        OrdersDbContext context,
        ILogger<CancelOrderCommandHandler> logger) : IRequestHandler<CancelOrderCommand, Result>
    {
        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId || o.UserId == request.UserId, cancellationToken);

            if (order is null)
            {
                logger.LogWarning("Order {OrderId} was not found for User {UserId}", request.OrderId, request.UserId);
                return Result.Fail(StatusCodes.Status404NotFound, "Order not found");
            }

            order.SetStatusToCanceled();
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
