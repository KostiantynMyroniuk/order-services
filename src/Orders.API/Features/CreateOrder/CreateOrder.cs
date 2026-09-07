using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Orders.API.Infrastructure;
using Orders.API.Infrastructure.Services;
using Orders.API.Models;
using Shared.Models;

namespace Orders.API.Features.CreateOrder
{
    public record CreateOrderCommand(
        Guid RequestId,
        string UserId,
        Address Address,
        List<OrderItemRequest> Items) : IRequest<Result<Guid>>;

    public sealed record OrderItemRequest(Guid ProductId, int Quantity);

    public class CreateOrderCommandHandler(
        OrdersDbContext context,
        ICatalogClientService catalogClientService,
        ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var existingOrder = await context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.RequestId == request.RequestId, cancellationToken);

            if (existingOrder is not null)
            {
                logger.LogInformation(
                        "Order {OrderId} already exists for RequestId {RequestId}.",
                        existingOrder.Id,
                        request.RequestId);

                return Result<Guid>.Success(existingOrder.Id);
            }

            var confirmedProducts = await catalogClientService.GetProductsByIds(request.Items.Select(i => i.ProductId), cancellationToken);
            var confirmedProductsDictionary = confirmedProducts.ToDictionary(p => p.ProductId, p => p);

            var order = new Order(request.RequestId, request.UserId, request.Address);

            foreach (var i in request.Items)
            {
                var confirmedProduct = confirmedProductsDictionary[i.ProductId];
                order.AddOrderItem(i.ProductId, order.Id, confirmedProduct.ProductName, confirmedProduct.Price, i.Quantity);
            }

            try
            {
                context.Orders.Add(order);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (IsRequestIdConflict(ex))
            {
                context.ChangeTracker.Clear();

                var existingOrderCatched = await context.Orders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.RequestId == request.RequestId, cancellationToken);

                if (existingOrderCatched is null)
                {
                    logger.LogError("Unique constraint violation for requestId {RequestId} but no matching order was found.", request.RequestId);

                    return Result<Guid>.Fail(
                        StatusCodes.Status404NotFound, 
                        "Duplicate request detected but original order could not be located.");
                }

                logger.LogInformation(
                        "Order {OrderId} already exists for RequestId {RequestId}.",
                        existingOrderCatched.Id,
                        request.RequestId);

                return Result<Guid>.Success(existingOrderCatched.Id);
            }

            return Result<Guid>.Success(order.Id);
        }

        private static bool IsRequestIdConflict(Exception exception) =>
            exception is DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 } sqlEx }
                && sqlEx.Message.Contains("IX_Orders_RequestId", StringComparison.OrdinalIgnoreCase);
    }
}
