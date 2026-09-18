using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Features;
using Orders.API.Features.CancelOrder;
using Orders.API.Features.CreateOrder;
using Orders.API.Features.GetMyOrders;
using Orders.API.Features.GetOrder;
using Orders.API.Models;
using Shared.Models;
using Shared.Services;

namespace Orders.API.Apis
{
    public static class OrdersApi
    {
        public static void MapOrdersApi(this IEndpointRouteBuilder app)
        {
            var orderGroup = app.MapGroup("api/orders")
                .RequireAuthorization()
                .WithTags("Orders");

            orderGroup.MapPost("/", CreateOrder);
            orderGroup.MapPost("/{orderId:guid}/cancel", CancelOrder);
            orderGroup.MapGet("/{orderId:guid}", GetOrder);
            orderGroup.MapGet("/", GetMyOrders);
        }

        public sealed record CreateOrderRequest(Address Address, ICollection<CreateOrderItem> Items);
        public sealed record CreateOrderItem(Guid ProductId, int Quantity);
        public static async Task<Results<Ok<Guid>, NotFound<string>, BadRequest>> CreateOrder(
            [FromHeader(Name = "X-Request-Id")] Guid requestId,
            CreateOrderRequest request,
            ISender sender,
            IIdentityProvider identityProvider,
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return TypedResults.BadRequest();

            var result = await sender.Send(
                new CreateOrderCommand(
                    requestId,
                    userId!,
                    request.Address,
                    request.Items
                        .Select(i => new OrderItemRequest(
                            i.ProductId, 
                            i.Quantity)).ToList()), 
                ct);

            if (result.IsSuccess)
                return TypedResults.Ok(result.Value);

            return result.StatusCode switch
            {
                StatusCodes.Status404NotFound => TypedResults.NotFound(result.ErrorMessage),
                _ => TypedResults.BadRequest()
            };
        }

        public static async Task<Results<Ok<GetOrderResponse>, NotFound<string>, BadRequest>> GetOrder(
            [FromRoute] Guid orderId,
            ISender sender,
            IIdentityProvider identityProvider,
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return TypedResults.BadRequest();

            var result = await sender.Send(new GetOrderQuery(orderId, userId), ct);

            if (result.IsSuccess)
                return TypedResults.Ok(result.Value);

            return result.StatusCode switch
            {
                StatusCodes.Status404NotFound => TypedResults.NotFound(result.ErrorMessage),
                _ => TypedResults.BadRequest()
            };
        }

        public static async Task<Results<Ok<PaginatedList<GetMyOrdersResponse>>, BadRequest>> GetMyOrders(
            ISender sender,
            IIdentityProvider identityProvider,
            CancellationToken ct,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = identityProvider.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return TypedResults.BadRequest();

            var result = await sender.Send(new GetMyOrdersQuery(
                userId,
                pageNumber,
                pageSize), ct);

            if (result.IsSuccess)
                return TypedResults.Ok(result.Value);

            return TypedResults.BadRequest();
        }

        public static async Task<Results<NoContent, NotFound<string>, BadRequest>> CancelOrder(
            [FromRoute] Guid orderId,
            ISender sender,
            IIdentityProvider identityProvider,
            CancellationToken ct)
        {
            var userId = identityProvider.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return TypedResults.BadRequest();

            var result = await sender.Send(new CancelOrderCommand(orderId, userId), ct);

            if (result.IsSuccess)
                return TypedResults.NoContent();

            return result.StatusCode switch
            {
                StatusCodes.Status404NotFound => TypedResults.NotFound(result.ErrorMessage),
                _ => TypedResults.BadRequest()
            };
        }
    }
}
